using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RangedEnemy : EnemyAiTutorial
{
    public float timeBetweenAttacks;
    public float raycastCooldownDuration;
    private float lastRaycastTime; // Variable to store the time of the last raycast
    private int totalRaycasts; // Counter for total raycasts
    private int failedRaycasts; // Counter for failed raycasts
    public GameObject grenade;
    private float grenadeSpeed;
    bool isGrenadeThrown = false;


    public EnemyState RangedEnemyState { get; private set; }


    public void AttackPlayer()
    {
        if (!playerInAttackRange)
        {
            ChangeState(EnemyState.Decision);
            return;
        }

        // Check if the enemy is currently attacking
        

        // Check if enough time has passed since the last raycast
        if (Time.time - lastRaycastTime < raycastCooldownDuration)
            return;

        if (playerInAttackRange)
        {
            foreach (Transform muzzleTransform in muzzleTransforms)
            {
                // Check if the enemy is currently relocating or throwing grenade
                if (currentState == EnemyState.Relocating || currentState == EnemyState.ThrowingGrenade)
                    return; // Stop raycast check while relocating or throwing grenade

                // Calculate direction from muzzle to player center
                Vector3 directionToPlayer = playerCollider.bounds.center - muzzleTransform.position;

                // Rotate the muzzle towards the player's direction
                Aim(directionToPlayer, muzzleTransform);

                // Draw a debug ray to visualize the line of sight
                Debug.DrawRay(muzzleTransform.position, directionToPlayer, Color.green, 2.0f); // Adjust color and duration as needed

                // Perform raycast check using the RaycastCheck function
                RaycastHit hit;
                totalRaycasts++; // Increment total raycasts counter
                if (RaycastCheck(muzzleTransform.position, directionToPlayer, Mathf.Infinity, out hit, PlayerLayer))
                {
                    RotateTowardsPlayer();
                    // Attack Code: Instantiate projectile
                    InstantiateProjectile(muzzleTransform);
                    lastRaycastTime = Time.time; // Update the last raycast time
                    failedRaycasts = 0; // Reset failed raycasts counter since the target is player
                }
                else
                {
                    failedRaycasts++; // Increment failed raycasts counter

                    // Add behaviors based on the number of failed raycasts
                    if (failedRaycasts == 2)
                    {
                        ChangeState(EnemyState.Relocating);
                    }
                    else if (failedRaycasts == 3 || failedRaycasts == 4)
                    {
                        ChangeState(EnemyState.Relocating);
                    }
                    else if (failedRaycasts >= 5)
                    {
                        ChangeState(EnemyState.ThrowingGrenade);
                        failedRaycasts = 0; // Reset failed raycasts counter after throwing grenade
                    }
                }
            }
        }
    }

    private void InstantiateProjectile(Transform muzzleTransform)
    {
        // Instantiate projectile at muzzle position with proper rotation
        Instantiate(Projectile, muzzleTransform.position, muzzleTransform.rotation);
    }

    private void Aim(Vector3 directionToPlayer, Transform muzzleTransform)
    {
        // Rotate the muzzle towards the player's direction
        muzzleTransform.rotation = Quaternion.LookRotation(directionToPlayer);
    }

    private bool RaycastCheck(Vector3 origin, Vector3 direction, float maxDistance, out RaycastHit hitInfo, LayerMask targetLayer)
    
    {
        // Perform raycast
        if (Physics.Raycast(origin, direction, out hitInfo, maxDistance, targetLayer))
        {
            // Check if the hit object is tagged as the player
            if (hitInfo.collider.CompareTag("Player"))
            {
                // Check if there are obstacles between the origin and hit point
                if (!Physics.Linecast(origin, hitInfo.point, ObstacleLayers))
                {
                    return true;
                }
            }
        }
        return false;
    }
    public IEnumerator Relocate()
    {
        yield return new WaitForSeconds(2.0f);
        // Calculate random point in range
        Vector3 randomDirection = Random.insideUnitSphere * walkpointRange;
        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, walkpointRange, 1);
        walkpoint = hit.position;
        agent.SetDestination(walkpoint);
        Debug.Log("Relocating...");
        ChangeState(EnemyState.Decision);
    }

    public IEnumerator ThrowGrenade()
    {
        // Check if a grenade has already been thrown
        if (isGrenadeThrown)
        {
            yield break; // Exit the coroutine if a grenade has already been thrown
        }

        // Set the flag to true to indicate that a grenade is being thrown
        isGrenadeThrown = true;

        // Instantiate grenade using the grenade GameObject
        GameObject grenadeInstance = Instantiate(grenade, muzzleTransforms[0].position, Quaternion.identity);

        // Continue making decisions after throwing the grenade
        Debug.Log("ThrowingGrenade...");

        yield return new WaitForSeconds(3.0f);

        // Reset the flag after the delay to allow throwing another grenade
        isGrenadeThrown = false;
        ChangeState(EnemyState.Decision);
    }

}
