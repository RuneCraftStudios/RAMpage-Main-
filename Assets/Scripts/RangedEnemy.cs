using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RangedEnemy : EnemyAiTutorial
{
    public float raycastCooldownDuration;
    private float lastRaycastTime;
    private int totalRaycasts;
    private int failedRaycasts = 0;
    public GameObject grenade;
    private float grenadeSpeed;
    bool isGrenadeThrown = false;
    private bool isattacking = false;

    public EnemyState RangedEnemyState { get; private set; }


    public void AttackPlayer()
    {
        Debug.Log("Failed Raycasts: " + failedRaycasts); // Debug to print failed raycast counter
        if (!playerInAttackRange)
        {
            StartCoroutine(ReturnToDecision());
            return;
        }

        // Check if already attacking or within cooldown period
        if (isattacking || Time.time - lastRaycastTime < raycastCooldownDuration)
        {
            return;
        }

        isattacking = true;

        // Pause the NavMesh Agent
        agent.isStopped = true;

        // Rotate the enemy towards the player
        RotateEnemyTowardsPlayer();

        // Perform a single raycast checking for both player and obstacles
        RaycastHit hit;
        Vector3 directionToPlayer = playerCollider.bounds.center - muzzleTransforms[0].position;
        Debug.DrawRay(muzzleTransforms[0].position, directionToPlayer, Color.green, 2.0f);

        if (Physics.Raycast(muzzleTransforms[0].position, directionToPlayer, out hit, Mathf.Infinity, PlayerLayer | ObstacleLayers))
        {
            // Check if the hit object is the player
            if (hit.collider.CompareTag("Player"))
            {
                Debug.Log("PlayerIdentified");
                foreach (Transform muzzleTransform in muzzleTransforms)
                {
                    Aim(playerCollider.bounds.center - muzzleTransform.position, muzzleTransform);
                }

                foreach (Transform muzzleTransform in muzzleTransforms)
                {
                    InstantiateProjectile(muzzleTransform);
                }
                lastRaycastTime = Time.time;
                failedRaycasts = 0;
            }
            // Check if the hit object is tagged as an obstacle
            else if (hit.collider.CompareTag("Obstacle"))
            {
                Debug.Log("ObstacleIdentified");
                failedRaycasts++;
                HandleFailedRaycasts();
            }
        }
        else
        {
            StartCoroutine(ReturnToDecision());
            failedRaycasts = 0;
        }

        // Resume the NavMesh Agent
        StartCoroutine(ResetAttack());
    }

    // Rotate the enemy towards the player
    private void RotateEnemyTowardsPlayer()
    {
        Vector3 directionToPlayer = playerCollider.bounds.center - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = targetRotation;
    }

    // Coroutine to reset isattacking after cooldown period
    private IEnumerator ResetAttack()
    {
        yield return new WaitForSeconds(timeBetweenAttacks);
        isattacking = false;
        agent.isStopped = false; // Resume the NavMesh Agent
        StartCoroutine(ReturnToDecision());
    }


    // Coroutine to reset isattacking after cooldown period
    


    // Call this function outside of AttackPlayer based on failedRaycasts
    public void HandleFailedRaycasts()
    {
        if (failedRaycasts >= 1 && failedRaycasts <= 2)
        {
            ChangeState(EnemyState.Relocating);
        }
        else if (failedRaycasts >= 3)
        {
            ChangeState(EnemyState.ThrowingGrenade);
            failedRaycasts = 0; // Reset counter
        }
    }



    private void InstantiateProjectile(Transform muzzleTransform)
    { 
        Instantiate(Projectile, muzzleTransform.position, muzzleTransform.rotation);
        enemySoundManager.PlayEnemyAttackSound();
    }

    private void Aim(Vector3 directionToPlayer, Transform muzzleTransform)
    {
        muzzleTransform.rotation = Quaternion.LookRotation(directionToPlayer);
    }

    public void Relocate()
    {

        // If a walkpoint is not already set, find one
        if (!walkpointSet)
            SearchWalkPoint();

        // Check if a walkpoint is set after the search
        if (walkpointSet)
        {
            Debug.Log("Relocating...");
            // After waiting, move to the walkpoint
            agent.SetDestination(walkpoint);
            enemySoundManager.PlayEnemyRelocateSound();
            StartCoroutine(RelocateRoutine()); // Start the relocation routine
        }
        else
        {
            Debug.Log("No walkpoint found.");
            StartCoroutine(ReturnToDecision()); // If no walkpoint is found, return to decision state
        }

    }

    private IEnumerator RelocateRoutine()
    {
        // Wait for a brief period before moving to the walkpoint
        yield return new WaitForSeconds(3.0f);

        // Return to the decision state
        StartCoroutine(ReturnToDecision());
    }

    public void ThrowGrenade()
    {
        Debug.Log("Throwing Grenade..."); // Add this line to check if this method is being called
        GameObject grenadeInstance = Instantiate(grenade, muzzleTransforms[0].position, Quaternion.identity);
        StartCoroutine(ThrowGrenadeRoutine());
        enemySoundManager.PlayEnemyGrenadeSound();
    }

    public IEnumerator ThrowGrenadeRoutine()
    {
        Debug.Log("ThrowGrenadeRoutine started"); // Add this line to check if the coroutine starts
        yield return new WaitForSeconds(3.0f);
        Debug.Log("ThrowGrenadeRoutine finished"); // Add this line to check if the coroutine ends
        StartCoroutine(ReturnToDecision());
    }

}
