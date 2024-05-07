using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EvasiveEnemy : EnemyAiTutorial
{
    [Header("Evasive Enemy Settings")]
    public bool playerInEvasionRange;
    public float EvasionRange;
    public float parabolaHeight = 3f; // Height of the parabolic path
    private bool isAttacking = false;
    private Transform playerTransform;
    public GameObject muzzleObject;
    public GameObject projectilePrefab;

    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform; // Assuming player has the "Player" tag
        StartCoroutine(UpdatePlayerPositionCoroutine(1.0f)); // Start coroutine to update player position with a delay
    }

    private IEnumerator UpdatePlayerPositionCoroutine(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            UpdatePlayerPosition();
        }
    }

    private void UpdatePlayerPosition()
    {
        if (playerTransform == null)
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform; // Re-find player if lost
        }
    }

    public void AttackPlayer()
    {
        if (playerTransform == null || muzzleObject == null)
        {
            Debug.LogError("Player transform or muzzle transform not assigned!");
            return;
        }

        if (isAttacking)
        {
            return;
        }

        isAttacking = true;
        agent.isStopped = true;
        RotateEnemyTowardsPlayer();

        Transform muzzleTransform = muzzleObject.transform;

        // Calculate the direction from muzzle point to player
        Vector3 direction = (playerTransform.position - muzzleTransform.position).normalized;

        // Calculate the distance between player and muzzle point
        float distance = Vector3.Distance(playerTransform.position, muzzleTransform.position);

        // Calculate the midpoint between muzzle and player
        Vector3 midPoint = (muzzleTransform.position + playerTransform.position) / 2f;
        midPoint += Vector3.up * parabolaHeight; // Adjust the midpoint to create a parabolic path

        // Calculate the time for the projectile to reach the midpoint horizontally
        float timeToMidpoint = distance / CalculateProjectileSpeed(parabolaHeight, distance);

        Vector3 initialVelocity = ((midPoint - muzzleTransform.position) / timeToMidpoint - 0.5f * Physics.gravity * timeToMidpoint) * 1.25f; // Increase the velocity by a factor of 1.2



        // Instantiate the projectile
        GameObject projectileInstance = Instantiate(projectilePrefab, muzzleTransform.position, Quaternion.identity);

        // Get the Rigidbody component of the projectile
        Rigidbody projectileRigidbody = projectileInstance.GetComponent<Rigidbody>();

        // Apply the calculated velocity to the projectile
        projectileRigidbody.velocity = initialVelocity;

        // Destroy the projectile after a certain duration
        Destroy(projectileInstance, timeToMidpoint * 2); // Multiply by 2 to ensure enough time for the projectile to reach the player

        StartCoroutine(ResetAttack(timeBetweenAttacks));
        StartCoroutine(ReturnToDecisionSlow());
    }

    private IEnumerator ResetAttack(float time)
    {
        yield return new WaitForSeconds(time);
        isAttacking = false;
        
    }


    private void RotateEnemyTowardsPlayer()
    {
        Vector3 directionToPlayer = playerTransform.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = targetRotation;
    }

    private float CalculateProjectileSpeed(float height, float distance)
    {
        float timeToPeak = Mathf.Sqrt(2 * height / Mathf.Abs(Physics.gravity.y));
        float timeToTarget = 2 * timeToPeak;
        return distance / timeToTarget;
    }

    public void Relocate()
    {
        // Check if the player transform is available
        if (playerTransform == null)
        {
            Debug.LogError("Player transform is not assigned.");
            return;
        }

        // If a walk point is not already set, find one
        if (!walkpointSet)
            SearchWalkPoint();

        // Check if a walk point is set after the search
        if (walkpointSet)
        {
            // Calculate the direction away from the player
            Vector3 directionAwayFromPlayer = (transform.position - playerTransform.position).normalized;

            // Calculate the distance to the player
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

            // Calculate the distance from the current walk point to the player
            float distanceToWalkPoint = Vector3.Distance(walkpoint, playerTransform.position);

            if (distanceToWalkPoint > distanceToPlayer)
            {
                agent.isStopped = false;
                agent.speed = 150f;
                Debug.Log("Relocating...");
                // After waiting, move to the walk point
                agent.SetDestination(walkpoint);
                StartCoroutine(RelocateRoutine()); // Start the relocation routine
            }
            else
            {
                Debug.Log("Next walk point is closer to the player.");

                // Search for another walk point biased away from the player
                StartCoroutine(SearchWalkPointBiasedCoroutine(directionAwayFromPlayer));
                StartCoroutine(ReturnToDecision());
            }
        }
        else
        {
            Debug.Log("No walk point found.");
            // If no walk point is found, return to decision state
            StartCoroutine(ReturnToDecision());
        }
    }

    private IEnumerator SearchWalkPointBiasedCoroutine(Vector3 directionAwayFromPlayer)
    {
        while (true)
        {
            // Bias the random point away from the player
            Vector3 biasedDirection = directionAwayFromPlayer * walkpointRange;
            Vector3 biasedPosition = transform.position + new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)) + biasedDirection;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(biasedPosition, out hit, 2f, NavMesh.AllAreas))
            {
                walkpoint = hit.position;
                walkpointSet = true;

                // Wait until the enemy reaches the walk point
                yield return new WaitUntil(() => Vector3.Distance(transform.position, walkpoint) <= 0.5f);

                walkpointSet = false;
            }

            yield return new WaitForSeconds(CheckPointFrequency);
        }
    }




    private IEnumerator RelocateRoutine()
    {
        // Wait for a brief period before moving to the walkpoint
        yield return new WaitForSeconds(3.0f);

        // Return to the decision state
        StartCoroutine(ReturnToDecision());
    }
}
