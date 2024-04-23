using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class EvasiveEnemy : EnemyAiTutorial
{
    public bool playerInEvasionRange;
    public float EvasionRange;
    public float projectileSpeed = 30f;  // Speed at which the projectile will be fired
    public void AttackPlayer()
    {
        if (!playerInAttackRange)
        {
            ChangeState(EnemyState.Decision);
            return;
        }
        
            foreach (Transform muzzleTransform in muzzleTransforms)
            {
                Vector3 targetPosition = playerCollider.bounds.center;
                Vector3 projectilePosition = muzzleTransform.position;

                Vector3 toTarget = targetPosition - projectilePosition;

                // Fire the projectile
                FireProjectile(projectilePosition, toTarget);
            }
        StartCoroutine(ReturnToDecision());
        
    }

    private void FireProjectile(Vector3 startPosition, Vector3 targetPosition)
    {
        GameObject projectileInstance = Instantiate(Projectile, startPosition, Quaternion.identity);
        Rigidbody projectileRigidbody = projectileInstance.GetComponent<Rigidbody>();

        Vector3 velocity = CalculateVelocity(targetPosition, startPosition, Physics.gravity.y);
        projectileRigidbody.velocity = velocity;
    }

    private Vector3 CalculateVelocity(Vector3 target, Vector3 origin, float gravity)
    
    {
        // Distance to target
        float distance = Vector3.Distance(origin, target);

        // Direction to target
        Vector3 direction = (target - origin).normalized;

        // Calculate initial velocity required to hit the target using the formula for projectile motion
        float firingAngle = 45.0f;  // Angle in degrees, you might need to adjust or calculate this
        float radianAngle = firingAngle * Mathf.Deg2Rad;

        float v0 = Mathf.Sqrt(distance * Mathf.Abs(gravity) / Mathf.Sin(2 * radianAngle));

        return Quaternion.LookRotation(direction) * new Vector3(0, v0 * Mathf.Sin(radianAngle), v0 * Mathf.Cos(radianAngle));
    }


    public IEnumerator Relocate()
    {
        // Get the direction away from the player
        Vector3 directionAwayFromPlayer = transform.position - playerCollider.transform.position;
        directionAwayFromPlayer.Normalize();

        // Find a random direction that also trends away from the player
        Vector3 randomDirection = Random.insideUnitSphere + directionAwayFromPlayer;
        randomDirection.Normalize();

        // Calculate the target position
        Vector3 potentialTarget = transform.position + randomDirection * walkpointRange;

        // Use NavMesh to find the nearest valid point
        NavMeshHit hit;
        if (NavMesh.SamplePosition(potentialTarget, out hit, walkpointRange, 1))
        {
            walkpoint = hit.position;
        }
        else
        {
            // If no valid NavMesh point is found, default to moving directly away
            walkpoint = transform.position + directionAwayFromPlayer * walkpointRange;
            NavMesh.SamplePosition(walkpoint, out hit, walkpointRange, 1);  // Try one more time to find a close valid position
            walkpoint = hit.position;
        }

        agent.SetDestination(walkpoint);
        Debug.Log("Relocating...");

        // Wait until the agent has reached the destination
        while (!agent.pathPending && agent.remainingDistance > agent.stoppingDistance)
        {
            yield return null;  // Wait until next frame before re-checking the condition
        }

        // Optionally, add a small fixed delay to stabilize the state change if needed
        yield return new WaitForSeconds(0.5f);

        // Change state back to decision making
        ChangeState(EnemyState.Decision);
    }

}
