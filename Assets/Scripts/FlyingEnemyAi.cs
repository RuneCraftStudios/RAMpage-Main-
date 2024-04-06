using UnityEngine;
using System.Collections;

public class FlyingEnemyAi : MonoBehaviour
{
    public Transform player;
    public LayerMask WhatIsGround, WhatIsPlayer;
    public int maxHealth = 100; // Maximum health of the enemy
    private int currentHealth; // Current health of the enemy
    public MeshRenderer[] meshRenderers; // Array of mesh renderers for the enemy
    public SkinnedMeshRenderer[] skinnedMeshRenderers; // Array of skinned mesh renderers for the enemy
    public global::EnemyState currentState; // Fully qualify the EnemyState enum
    public Animator animator; // Animator component for controlling animations
    public Rigidbody enemyRigidbody; // Rigidbody component for physics simulation
    public Collider[] enemyColliders; // Array of collider components for collision detection

    // Animation parameters
    private static readonly int InSight = Animator.StringToHash("InSight");
    private static readonly int InRange = Animator.StringToHash("InRange");
    private static readonly int IsActive = Animator.StringToHash("IsActive");

    // Speed parameters
    public float patrolSpeed = 2f; // Speed of the enemy while patrolling
    public float chaseSpeed = 4f; // Speed of the enemy while chasing

    //Patrolling
    public Vector3 patrolCenter;
    public float patrolRadius;

    //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    public GameObject Projectile;
    public Transform[] muzzleTransforms; // Array of muzzle transforms for projectile spawn positions
    public float forwardForce = 32f; // Force applied to projectile in the forward direction
    public float upwardForce = 8f; // Force applied to projectile in the upward direction
    public float attackStoppingDistance = 5f; // Distance at which the enemy stops before attacking

    //States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        currentState = global::EnemyState.WaitingToBeSpawned; // Set initial state

        // Ensure Animator component is assigned
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        // Ensure Rigidbody component is assigned
        if (enemyRigidbody == null)
        {
            enemyRigidbody = GetComponent<Rigidbody>();
        }

        // Ensure Collider components are assigned
        if (enemyColliders == null || enemyColliders.Length == 0)
        {
            enemyColliders = GetComponentsInChildren<Collider>();
        }
    }

    private void Update()
    {
        //Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, WhatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, WhatIsPlayer);

        // Update animator parameters
        animator.SetBool(InSight, playerInSightRange);
        animator.SetBool(InRange, playerInAttackRange);
        animator.SetBool(IsActive, currentState != global::EnemyState.WaitingToBeSpawned);

        switch (currentState)
        {
            case global::EnemyState.WaitingToBeSpawned:
                // Code for WaitingToBeSpawned state
                break;
            case global::EnemyState.Patrol:
                if (!playerInSightRange && !playerInAttackRange) Patroling();
                if (playerInSightRange && !playerInAttackRange) ChangeState(global::EnemyState.Chase); // Transition to Chase state
                if (playerInAttackRange && playerInSightRange) AttackPlayer();
                break;
            case global::EnemyState.Chase:
                ChasePlayer(); // Chase the player
                if (playerInAttackRange && playerInSightRange) AttackPlayer(); // Transition to Attack state if the player is in attack range
                if (!playerInSightRange) ChangeState(global::EnemyState.Patrol); // Transition back to Patrol state if the player is out of sight
                break;
            case global::EnemyState.Attack:
                // Code for Attack state
                break;
                // Add more cases for other states as needed
        }
    }

    private void Patroling()
    {
        Vector3 randomPoint = Random.insideUnitSphere * patrolRadius + patrolCenter;
        randomPoint.y = patrolCenter.y;
        transform.position = Vector3.MoveTowards(transform.position, randomPoint, patrolSpeed * Time.deltaTime);
    }

    private void ChasePlayer()
    {
        // Calculate direction to the player
        Vector3 direction = (player.position - transform.position).normalized;

        // Calculate distance to the player
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Define minimum distance to maintain from the player
        float minDistance = 5f; // Adjust this value as needed

        // Move towards the player only if the distance is greater than the minimum
        if (distanceToPlayer > minDistance)
        {
            transform.position += direction * chaseSpeed * Time.deltaTime;
        }
        else
        {
            // Optional: Add logic to hover or maintain position if within minimum distance
            // For example: transform.position = transform.position;
        }
    }

    private void AttackPlayer()
    {
        // Calculate distance in 2D space, ignoring the height component
        Vector3 playerPosition = player.position;
        playerPosition.y = transform.position.y; // Ignore height component
        float distanceToPlayer = Vector3.Distance(transform.position, playerPosition);

        // Check if the enemy is within the attack stopping distance
        if (distanceToPlayer <= attackStoppingDistance)
        {
            transform.LookAt(player);

            if (!alreadyAttacked)
            {
                if (muzzleTransforms != null && muzzleTransforms.Length > 0)
                {
                    // Attack from each muzzle transform
                    foreach (Transform muzzleTransform in muzzleTransforms)
                    {
                        // Attack Code Thing that makes enemy attack and how it attacks
                        Rigidbody rb = Instantiate(Projectile, muzzleTransform.position, muzzleTransform.rotation).GetComponent<Rigidbody>();
                        rb.AddForce(muzzleTransform.forward * forwardForce, ForceMode.Impulse);
                        rb.AddForce(muzzleTransform.up * upwardForce, ForceMode.Impulse);
                    }

                    alreadyAttacked = true;
                    Invoke(nameof(ResetAttack), timeBetweenAttacks);
                }
                else
                {
                    Debug.LogWarning("Muzzle Transforms are not assigned for projectile spawn positions.");
                }
            }
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void TakeDamage(int damage)
    {
        // Reduce current health by the amount of damage
        currentHealth -= damage;

        // Check if the enemy is defeated
        if (currentHealth <= 0)
        {
            DestroyEnemy();
        }
    }

    private void DestroyEnemy()
    {
        // Perform any necessary cleanup and destroy the enemy GameObject
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }

    public void ChangeState(global::EnemyState newState)
    {
        currentState = newState;

        // Update behavior based on the new state
        switch (currentState)
        {
            case global::EnemyState.WaitingToBeSpawned:
                // Disable all mesh renderers in WaitingToBeSpawned state
                foreach (var renderer in meshRenderers)
                {
                    if (renderer != null)
                    {
                        renderer.enabled = false;
                    }
                }
                foreach (var skinnedRenderer in skinnedMeshRenderers)
                {
                    if (skinnedRenderer != null)
                    {
                        skinnedRenderer.enabled = false;
                    }
                }
                break;
            case global::EnemyState.Patrol:
                // Enable all mesh renderers in Patrol state after a delay
                StartCoroutine(EnableMeshRenderersAfterDelay(1.0f)); // Adjust the delay as needed
                break;
                // Add more cases for other states as needed
        }
    }

    private IEnumerator EnableMeshRenderersAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Enable all mesh renderers
        foreach (var renderer in meshRenderers)
        {
            if (renderer != null)
            {
                renderer.enabled = true;
            }
        }
        foreach (var skinnedRenderer in skinnedMeshRenderers)
        {
            if (skinnedRenderer != null)
            {
                skinnedRenderer.enabled = true;
            }
        }
    }
}
