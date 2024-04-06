using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RangedEnemy : EnemyAiTutorial
{
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    public float raycastCooldownDuration = 1.0f;
    private float lastRaycastTime; // Variable to store the time of the last raycast
    public float relocateTime = 3.0f; // Time to wait before attempting to relocate
    private float lastRelocateTime; // Time when the enemy last attempted to relocate
    private bool isRelocating; // Flag to indicate if the enemy is currently relocating
    public void AttackPlayer()
    {
        if (!playerInAttackRange)
        {
            MakeDecision();
            return;
        }

        // Rotate towards the player
        RotateAgentTowardsPlayer();

        // Check if the enemy is currently attacking
        if (!alreadyAttacked)
        {
            // Set the destination of the NavMeshAgent to the current position to prevent movement
            agent.SetDestination(transform.position);
        }

        // Check if enough time has passed since the last raycast
        if (Time.time - lastRaycastTime < raycastCooldownDuration)
            return;

        if (playerInAttackRange)
        {
            foreach (Transform muzzleTransform in muzzleTransforms)
            {
                // Calculate direction to the player
                Vector3 directionToPlayer = playerCollider.bounds.center - muzzleTransform.position;

                // Draw a debug ray to visualize the line of sight
                Debug.DrawRay(muzzleTransform.position, directionToPlayer, Color.green, 2.0f); // Adjust color and duration as needed

                // Perform raycast to check if there's a clear line of sight to the player
                if (Physics.Raycast(muzzleTransform.position, directionToPlayer, out RaycastHit hit, Mathf.Infinity, PlayerLayer))
                {
                    // Check if the hit object is tagged as the player
                    if (hit.collider.CompareTag("Player"))
                    {
                        // Check if there are obstacles between the enemy and the player
                        if (!Physics.Linecast(muzzleTransform.position, playerCollider.bounds.center, GroundLayer))
                        {
                            // Rotate the muzzle towards the player's position
                            muzzleTransform.rotation = Quaternion.LookRotation(directionToPlayer);

                            // Attack Code: Instantiate projectile and apply force towards the player
                            StartCoroutine(ProjectileBuffer());
                            StartCoroutine(Firing(muzzleTransform));
                            // Set alreadyAttacked to true to prevent multiple projectiles
                            alreadyAttacked = true;
                            lastRaycastTime = Time.time;
                            Invoke(nameof(ResetAttack), timeBetweenAttacks);


                            // Exit the loop once we successfully attack
                            return;
                        }
                    }
                    else
                    {
                        // If the player is not visible within attack range, initiate relocation and attack
                        StartCoroutine(RelocateAndAttack());
                    }
                }
            }
        }

    }

    public void RotateAgentTowardsPlayer()
    {
        if (player != null)
        {
            // Calculate direction to the player
            Vector3 directionToPlayer = player.position - transform.position;

            // Create a rotation that looks at the player
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);

            // Apply the rotation to the agent's transform
            transform.rotation = targetRotation;
        }
        else
        {
            Debug.LogWarning("Player reference is null!");
        }
    }

    IEnumerator Firing(Transform muzzleTransform)
    {
        yield return new WaitForSeconds(1.0f);
        Rigidbody rb = Instantiate(Projectile, muzzleTransform.position, muzzleTransform.rotation).GetComponent<Rigidbody>();
        rb.AddForce(muzzleTransform.forward * forwardForce, ForceMode.Impulse);
        rb.AddForce(Vector3.up * upwardForce, ForceMode.Impulse);
    }

    IEnumerator ProjectileBuffer()
    {
        yield return new WaitForSeconds(ProjectileInitiateBuffer); // Wait for the buffer time
    }


    private IEnumerator RelocateAndAttack()
    {
        // Attempt to find a new position closer to the player
        Vector3 newPosition = FindNewPosition();
        agent.SetDestination(newPosition);

        // Wait for the enemy to reach the new position
        yield return new WaitUntil(() => Vector3.Distance(transform.position, newPosition) < agent.stoppingDistance);

        // Wait for a short duration before initiating the raycast
        yield return new WaitForSeconds(1.0f); // Adjust this delay as needed

        // Check if there's a clear line of sight to the player
        if (Physics.Raycast(transform.position, player.position - transform.position, out RaycastHit hit, Mathf.Infinity, PlayerLayer))
        {
            // Check if the hit object is tagged as the player
            if (hit.collider.CompareTag("Player"))
            {
                // Check if there are obstacles between the enemy and the player
                if (!Physics.Linecast(transform.position, playerCollider.bounds.center, GroundLayer))
                {
                    // Enemy has a clear line of sight to the player, initiate attack
                    AttackPlayer();
                    yield break; // Exit the coroutine
                }
            }
        }

        // If we reach here, the player is not in sight, wait for relocate time duration
        yield return new WaitForSeconds(relocateTime);

        // Pick another point and repeat the process
        StartCoroutine(RelocateAndAttack());
    }




    private void FixedUpdate()
    {
        if (isRelocating)
        {
            // Attempt to find a new position closer to the player
            Vector3 newPosition = FindNewPosition();
            agent.SetDestination(newPosition);

            // Check if the enemy has reached the new position
            if (Vector3.Distance(transform.position, newPosition) < agent.stoppingDistance)
            {
                isRelocating = false;
            }
        }
    }




    // Function to find a new position closer to the player
    private Vector3 FindNewPosition()
    {
        Vector3 newDestination = Vector3.zero;
        float closestDistanceToPlayer = Mathf.Infinity;

        // Attempt to find a valid position within a radius of 2
        for (int i = 0; i < 10; i++) // Try 10 times to find a valid position
        {
            // Calculate random point within a radius of 2
            Vector3 randomDirection = Random.insideUnitSphere * 2f;
            randomDirection += transform.position;
            NavMeshHit hit;

            if (NavMesh.SamplePosition(randomDirection, out hit, 2f, NavMesh.AllAreas))
            {
                // Check if the sampled position is on the NavMesh
                Vector3 newPosition = hit.position;

                // Calculate distance to the player from the new position
                float distanceToPlayer = Vector3.Distance(newPosition, player.position);

                // Check if the new position is closer to the player and on the navmesh
                if (distanceToPlayer < closestDistanceToPlayer)
                {
                    // Update closest distance and set new destination
                    closestDistanceToPlayer = distanceToPlayer;
                    newDestination = newPosition;
                }
            }
        }

        return newDestination;
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }
}
