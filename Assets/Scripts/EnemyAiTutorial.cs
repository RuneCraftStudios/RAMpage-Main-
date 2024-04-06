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
    Die
    // Add more states as needed
}

public class EnemyAiTutorial : MonoBehaviour
{
    [Header("Set Layers")]
    public LayerMask GroundLayer;
    public LayerMask PlayerLayer;

    [Header("NavMesh Settings")]
    public NavMeshAgent agent;

    [Header("Set Target")]
    public Transform player;
    public Collider playerCollider; // Collider component of the player

    [Header("Open Visuals")]
    public MeshRenderer[] meshRenderers; // Array of mesh renderers for the enemy
    public SkinnedMeshRenderer[] skinnedMeshRenderers; // Array of skinned mesh renderers for the enemy

    [Header("Enemy Hitbox")]
    public Rigidbody enemyRigidbody; // Rigidbody component for physics simulation
    public Collider[] enemyColliders; // Array of collider components for collision detection
    public Animator animator; // Animator component for controlling animations
    public AnimationClip DieAnimationClip;

    [Header("Base Parameters")]
    private Health health; // Reference to the Health component
    private int maxHealth; // Maximum health of the enemy
    public LootSystem lootSystem; // Reference to the LootSystem script

    [Header("Patrol Parameters")]
    public float patrolSpeed = 2f; // Speed of the enemy while patrolling
    public Vector3 walkpoint;
    bool walkpointSet;
    public float walkpointRange;
    public float CheckPointFrequency;

    [Header("Chase Parameters")]
    public float chaseSpeed = 4f; // Speed of the enemy while chasing

    [Header("Attack Parameters")]
    public float sightRange, attackRange;
    public bool playerInSightRange = false;
    public bool playerInAttackRange = false;

    [Header("Projectile Parameters")]
    public GameObject Projectile;
    [SerializeField] public float ProjectileInitiateBuffer;
    public Transform[] muzzleTransforms; // Array of muzzle transforms for projectile spawn positions
    public float forwardForce = 32f; // Force applied to projectile in the forward direction
    public float upwardForce = 8f; // Force applied to projectile in the upward direction

    [Header("Stun Parameters")]
    private float StunDuration = 3.0f;
    public bool isStunned = false;


    //States
    private EnemyState currentState; // Current state of the enemy
    private float ResetTriggerTime = 1f;
    private Coroutine checkPlayerCoroutine;
    private IEnumerator searchWalkPointCoroutine;
    public float checkInterval = 0.1f;



    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        health = GetComponent<Health>();
        maxHealth = health.maxHealth;
        animator = GetComponent<Animator>();
        // Start the coroutine to periodically check player presence
        checkPlayerCoroutine = StartCoroutine(CheckPlayerPresence());
        // Make sure all animation clips are assigned in the inspector

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

        switch (currentState)
        {
            case EnemyState.WaitingToBeSpawned:
                Debug.Log("EnteredWaitingToBeSpawnedState");
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
                animator.SetBool("Patroling", true);
                agent.speed = patrolSpeed; // Set patrol speed
                Patroling();
                Debug.Log("EnteredPatrolState");
                break;
            case EnemyState.Chase:
                animator.SetBool("Chasing", true);
                agent.speed = chaseSpeed; // Set chase speed
                ChasePlayer(); // Chase the player
                Debug.Log("EnteredChaseState");
                break;
            case EnemyState.Attack:
                animator.Play("Attack");
                Debug.Log("EnteredAttackState");
                StartCoroutine(PrepareAttack());
                // Code for Attack state
                break;
            case EnemyState.Stun:
                isStunned = true;
                animator.SetBool("Stunned", true);
                agent.isStopped = true;
                StartCoroutine(RecoverFromStun(StunDuration));
                break;
            case EnemyState.KnockBack:
                animator.SetBool("IsKnockedBack", true);
                agent.isStopped = true;
                StartCoroutine(RecoverFromKnockBack());
                break;
            case EnemyState.Die:
                animator.SetBool("IsDied", true);
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

        animator.SetBool("Patroling", false);
        animator.SetBool("Chasing", false);
        animator.SetBool("Attacking", false);
        animator.SetTrigger("ChangeState");
        StartCoroutine(ResetTrigger(ResetTriggerTime));

        if (gameObject.activeInHierarchy) // Check if the enemy is still active
        {
            StartCoroutine(EnableMeshRenderersAfterDelay(1.0f)); // Adjust the delay as needed
        }
        
        if (currentState != EnemyState.WaitingToBeSpawned)
        {
            animator.SetBool("IsActive", true);
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
            MakeDecision();
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
            MakeDecision();
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
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
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
        animator.SetBool("Stunned", false);
        agent.isStopped = false;
        // Perform checks and transition to appropriate state after recovering from stun
        // For example, you can check if the player is nearby and transition to Chase state
        if (health.CurrentHealth <= 0)
        {
            ChangeState(EnemyState.Die);
        }
        MakeDecision();
    }
    private IEnumerator ResetTrigger(float ResetTriggerTime)
    {
        yield return new WaitForSeconds(ResetTriggerTime);

        animator.ResetTrigger("ChangeState");
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
        while (true)
        {
            // Check player presence within sight and attack ranges
            playerInSightRange = Physics.CheckSphere(transform.position, sightRange, PlayerLayer);
            playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, PlayerLayer);

            yield return new WaitForSeconds(checkInterval);
        }
    }


    private IEnumerator RecoverFromKnockBack()
    {
        yield return new WaitForSeconds(2.0f);
        agent.isStopped = false;
        animator.SetBool("IsKnockedBack", false);
        MakeDecision();
    }


}