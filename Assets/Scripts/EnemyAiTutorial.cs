using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using UnityEngine.ProBuilder.MeshOperations;

// Declare the EnemyState enum outside of any class and mark it as public
public enum EnemyState
{
    WaitingToBeSpawned,
    Patrol,
    Chase,
    Attack,
    Stun,
    KnockBack,
    Relocating,
    ThrowingGrenade,
    Decision,
    Die
    // Add more states as needed
}

public class EnemyAiTutorial : MonoBehaviour
{
    [Header("Set Layers")]
    public LayerMask GroundLayer;
    public LayerMask PlayerLayer;
    public LayerMask ignoreRaycastLayer;
    public LayerMask ObstacleLayers;

    [Header("NavMesh Settings")]
    public NavMeshAgent agent;

    [Header("Set Target")]
    public Transform player;
    public Collider playerCollider; // Collider component of the player

    [Header("Determine Renderers")]
    public MeshRenderer[] meshRenderers; // Array of mesh renderers for the enemy
    public SkinnedMeshRenderer[] skinnedMeshRenderers; // Array of skinned mesh renderers for the enemy

    [Header("Enemy Hitbox")]
    public Rigidbody enemyRigidbody; // Rigidbody component for physics simulation
    public Collider[] enemyColliders; // Array of collider components for collision detection
 
    public AnimationClip DieAnimationClip;

    [Header("Base Parameters")]
    private EnemyHealth health; // Reference to the Health component
    private int maxHealth; // Maximum health of the enemy
    public LootSystem lootSystem; // Reference to the LootSystem script

    [Header("Patrol Parameters")]
    public float patrolSpeed = 2f; // Speed of the enemy while patrolling
    public Vector3 walkpoint;
    public bool walkpointSet;
    public float walkpointRange;
    public float CheckPointFrequency;

    [Header("Chase Parameters")]
    public float chaseSpeed = 4f; // Speed of the enemy while chasing

    [Header("Attack Parameters")]
    public float sightRange, attackRange;
    public float fieldOfViewAngle = 90f;
    public float awarenessRadius = 3f;
    public bool playerInSightRange = false;
    public bool playerInAttackRange = false;

    [Header("Projectile Parameters")]
    public GameObject Projectile;
    public Transform[] muzzleTransforms; // Array of muzzle transforms for projectile spawn positions
    
    [Header("Stun Parameters")]
    private float StunDuration = 3.0f;
    public bool isStunned = false;

    //States
    protected EnemyState currentState;
    private Coroutine checkPlayerCoroutine;
    private IEnumerator searchWalkPointCoroutine;
    public float checkInterval = 0.1f;
    private RangedEnemy rangedEnemy;
    public GameObject gazeObject;
    public float rotationSpeed;
    public EnemyState CurrentState
    {
        get { return currentState; }
        private set { currentState = value; }
    }
    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        health = GetComponent<EnemyHealth>();
        maxHealth = health.maxHealth;
        rangedEnemy = GetComponent<RangedEnemy>();
        // Start the coroutine to periodically check player presence
        checkPlayerCoroutine = StartCoroutine(CheckPlayerPresence());
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
        switch (currentState)
        {
            case EnemyState.WaitingToBeSpawned:
                //Debug.Log("EnteredWaitingToBeSpawnedState");
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

                foreach (var collider in enemyColliders)
                {
                    if (collider != null)
                    {
                        collider.enabled = false;
                    }
                }
                break;

            case EnemyState.Patrol:
                Patroling();
                break;

            case EnemyState.Chase:

                ChasePlayer();

                break;
            case EnemyState.Attack:
                SelectAttackMethod();
                break;
            case EnemyState.Stun:
                break;
            case EnemyState.KnockBack:
                break;
            case EnemyState.Relocating:
                break;
            case EnemyState.ThrowingGrenade:
                break;
            case EnemyState.Decision:
                break;

            case EnemyState.Die:
                Debug.Log("EnteredDieState");
                agent.isStopped = true;
                sightRange = 0;
                attackRange = 0;    
                StartCoroutine(DieAfterBufferTime());
                break;
        }
    }
    public void ChangeState(EnemyState newState)
    {
        currentState = newState;

        if (gameObject.activeInHierarchy) // Check if the enemy is still active
        {
            StartCoroutine(EnableMeshRenderersAfterDelay(1.0f)); // Adjust the delay as needed
        }
        if (currentState == EnemyState.Relocating)
        {
            agent.speed = patrolSpeed;
            StartCoroutine(rangedEnemy.Relocate());
            Debug.Log("EnteredRelocateState");
        }
        if (currentState == EnemyState.ThrowingGrenade)
        {
            RotateTowardsPlayer();
            StartCoroutine(rangedEnemy.ThrowGrenade());
            Debug.Log("EnteredThrowingGrenadeState");
        }
        if (currentState == EnemyState.Patrol)
        {
            agent.speed = patrolSpeed;
            Debug.Log("EnteredPatrolState");
        }
        if (currentState == EnemyState.Chase)
        {
            RotateTowardsPlayer();
            agent.speed = chaseSpeed;
            Debug.Log("EnteredChaseState");
        }
        if (currentState == EnemyState.Attack)
        {
            Debug.Log("EnteredAttackState");
            agent.speed = 0.1f;
        }
        if (currentState == EnemyState.Decision)
        {
            Debug.Log("EnteredDecisionState");
            MakeDecision();
        }
        if (currentState == EnemyState.Stun)
        {
            Debug.Log("EnteredStunState");
            isStunned = true;
            agent.isStopped = true;
            StartCoroutine(RecoverFromStun(StunDuration));
        }
        if (currentState == EnemyState.KnockBack)
        {
            RotateTowardsPlayer();
            Debug.Log("EnteredKnockBackState");
            agent.isStopped = true;
            StartCoroutine(RecoverFromKnockBack());
        }

    }

    private IEnumerator PrepareAttack()
    {
        yield return new WaitForSeconds(1.0f);
        SelectAttackMethod();
    }

    private void Patroling()
    {
        if (playerInSightRange == false)
        {
            if (!walkpointSet)
                SearchWalkPoint();

            if (walkpointSet)
                agent.SetDestination(walkpoint);

            Vector3 distanceToWalkPoint = transform.position - walkpoint;

            //Walkpoint reached
            if (distanceToWalkPoint.magnitude < 1f)
                walkpointSet = false;
        }
        if (playerInSightRange == true)
        {
            ChangeState(EnemyState.Decision);
        }
      
    }
    private void SearchWalkPoint()
    {
        if (searchWalkPointCoroutine == null)
        {
            searchWalkPointCoroutine = SearchWalkPointCoroutine(); // Call the coroutine directly
            StartCoroutine(searchWalkPointCoroutine); // Start the coroutine
        }
    }

    private void ChasePlayer()
    {
        // If the player is within sight range but not within attack range, continue chasing
        if (playerInSightRange && !playerInAttackRange)
        {
            agent.SetDestination(player.position);
        }
        // If the player is within attack range or the sight range is false, make a decision
        else
        {
            ChangeState(EnemyState.Decision);
        }
    }
    public void MakeDecision()
    {
        if (playerInAttackRange== true)
        {
            ChangeState(EnemyState.Attack);
            if (currentState == EnemyState.Attack)
            {
                SelectAttackMethod();
            }
        }
        if (playerInSightRange == true && playerInAttackRange == false)
        {
            ChangeState(EnemyState.Chase);
        }
        if (playerInSightRange == false)
        {
            ChangeState(EnemyState.Patrol);
        }

    }

    public void SelectAttackMethod()
    {
        if (playerInAttackRange)
        {
            RangedEnemy rangedEnemy = GetComponent<RangedEnemy>(); // Assuming RangedEnemy is a component on the same GameObject
            if (rangedEnemy != null)
            {
                rangedEnemy.AttackPlayer(); // Call the AttackPlayer() method on the instance of RangedEnemy
            }
            else
            {
                Debug.LogError("RangedEnemy component not found!");
            }
        }
    }

    public void Stun()
    {
        ChangeState(EnemyState.Stun);
    }
    
    public void KnockBack()
    {
        ChangeState(EnemyState.KnockBack);
    }

    public void TakeDamage(int damage)
    {
        health.TakeDamage(damage);
    }

    private void Die()
    {
        DropLoot();
        Destroy(gameObject);
    }

    IEnumerator DieAfterBufferTime()
    {
        yield return new WaitForSeconds(DieAnimationClip.length); // Wait for the buffer time
        Die(); // Call your die logic after the buffer time
    }

    void DropLoot()
    {
        // Call the DropItems method from the LootSystem script if the GameObject is an enemy
        if (lootSystem != null)
        {
            // Define the desired offset
            Vector3 offset = new Vector3(0f, 1f, 0f); // Example offset (replace with your desired values)

            // Call the DropItems method with the enemy GameObject and the offset
            lootSystem.DropItems(gameObject, offset);
        }
    }

    private void DropItems()
    {
        // Call the DropItems method from the LootSystem script
        if (lootSystem != null)
        {
            // Define the desired location as a GameObject
            GameObject desiredLocation = new GameObject("DesiredLocation");
            desiredLocation.transform.position = new Vector3(10f, 0f, 5f); // Example coordinates (replace with your desired values)

            // Call the DropItems method with the desired location as the reference object and zero offset
            lootSystem.DropItems(desiredLocation, Vector3.zero);

            // Destroy the GameObject used as the desired location after dropping items
            Destroy(desiredLocation);
        }
    }



    private void OnDrawGizmosSelected()
    {
        // Draw FOV
        float halfFOV = fieldOfViewAngle / 2f;
        float viewDistance = sightRange;
        Vector3 fovLine1 = Quaternion.AngleAxis(halfFOV, transform.up) * transform.forward * viewDistance;
        Vector3 fovLine2 = Quaternion.AngleAxis(-halfFOV, transform.up) * transform.forward * viewDistance;

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, fovLine1);
        Gizmos.DrawRay(transform.position, fovLine2);
        Gizmos.DrawLine(transform.position, transform.position + fovLine1);
        Gizmos.DrawLine(transform.position, transform.position + fovLine2);

        // Draw sight range sphere
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);

        // Draw attack range sphere
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Draw awareness radius sphere
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, awarenessRadius);
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
        foreach (var collider in enemyColliders)
        {
            if (collider != null)
            {
                collider.enabled = true;
            }
        }
    }

    private IEnumerator RecoverFromStun(float StunDuration)
    {
        yield return new WaitForSeconds(StunDuration);
        isStunned = false;
        agent.isStopped = false;
        // Perform checks and transition to appropriate state after recovering from stun
        // For example, you can check if the player is nearby and transition to Chase state
        if (health.CurrentHealth <= 0)
        {
            ChangeState(EnemyState.Die);
        }
        ChangeState(EnemyState.Decision);
    }

    private IEnumerator SearchWalkPointCoroutine()
    {
        while (true)
        {
            // Calculate random point in range
            float randomZ = Random.Range(-walkpointRange, walkpointRange);
            float randomX = Random.Range(-walkpointRange, walkpointRange);

            walkpoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

            NavMeshHit hit;
            if (NavMesh.SamplePosition(walkpoint, out hit, 2f, NavMesh.AllAreas))
            {
                // Check if the sampled position is on the NavMesh
                walkpoint = hit.position;
                walkpointSet = true;

                yield return new WaitUntil(() => Vector3.Distance(transform.position, walkpoint) <= 0.5f); // Wait until the enemy reaches the walk point

                walkpointSet = false; // Reset walkpointSet after reaching the walk point
            }

            yield return new WaitForSeconds(CheckPointFrequency); // Adjust this delay as needed
        }
    }

    private IEnumerator CheckPlayerPresence()
    {
        Debug.Log("CheckPlayerPresence coroutine started.");

        while (true)
        {
            // Check player presence within sight range
            playerInSightRange = PlayerInSightRange();

            // Check player presence within attack range
            playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, PlayerLayer);

            yield return new WaitForSeconds(checkInterval);
        }
    }

    private bool PlayerInSightRange()
    {
        bool playerSeen = false; // Initialize playerSeen flag

        // Combine the PlayerLayer and IgnoreRaycastLayer masks
        LayerMask combinedLayerMask = PlayerLayer | ignoreRaycastLayer;

        Collider[] targetsInViewRadius = Physics.OverlapSphere(gazeObject.transform.position, sightRange, combinedLayerMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;

            // Check if the target is the player
            if (targetsInViewRadius[i].CompareTag("Player"))
            {
                playerSeen = true; // Set playerSeen to true
                continue; // Skip obstacle checks and continue to the next target
            }

            Vector3 directionToTarget = (target.position - gazeObject.transform.position).normalized;

            // Check if the target is within the field of view angle
            if (Vector3.Angle(gazeObject.transform.forward, directionToTarget) < fieldOfViewAngle / 2)
            {
                float distanceToTarget = Vector3.Distance(gazeObject.transform.position, target.position);

                // Debug the raycast
                Debug.DrawRay(gazeObject.transform.position, directionToTarget * distanceToTarget, Color.green);

                // Perform the raycast
                RaycastHit hit;
                if (Physics.Raycast(gazeObject.transform.position, directionToTarget, out hit, distanceToTarget, ObstacleLayers))
                {
                    // If there's an obstacle, continue to the next target
                    continue;
                }
                else
                {
                    playerSeen = true; // Set playerSeen to true
                }
            }
        }

        // Check if player is within awareness radius
        Collider[] targetsInAwarenessRadius = Physics.OverlapSphere(gazeObject.transform.position, awarenessRadius, PlayerLayer);
        if (targetsInAwarenessRadius.Length > 0)
        {
            playerSeen = true; // Set playerSeen to true
        }

        return playerSeen; // Return the playerSeen flag
    }
    protected void RotateTowardsPlayer()
    {
        agent.SetDestination(player.position);
    }
    private IEnumerator RecoverFromKnockBack()
    {
        yield return new WaitForSeconds(2.0f);
        agent.isStopped = false;
        ChangeState(EnemyState.Decision);
    }
}