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
    public LayerMask ObstacleLayers;
    public LayerMask ignoreRaycastLayer;

    [Header("Required Components")]
    public NavMeshAgent agent;
    public GameObject gazeObject;
    private bool ComponentsActive = true;
    public Animator animator;

    [Header("Set Target")]
    public Transform player;
    public Collider playerCollider;

    [Header("Determine Renderers")]
    public  MeshRenderer[] meshRenderers;
    public  SkinnedMeshRenderer[] skinnedMeshRenderers;

    [Header("Enemy Hitbox")]
    public  Rigidbody enemyRigidbody;
    public Collider[] enemyColliders;
    public AnimationClip DieAnimationClip;

    [Header("Base Parameters")]
    private EnemyHealth health;
    private int maxHealth;
    private LootSystem lootSystem;

    [Header("Patrol Parameters")]
    public float patrolSpeed = 2f;
    public Vector3 walkpoint;
    public bool walkpointSet;
    public float walkpointRange;
    public float CheckPointFrequency;

    [Header("Chase Parameters")]
    public float chaseSpeed = 4f;

    [Header("Attack Parameters")]
    public float sightRange, attackRange;
    public float timeBetweenAttacks;
    public float fieldOfViewAngle = 90f;
    public bool playerInSightRange = false;
    public bool playerInAttackRange = false;
    
    [Header("Stun Parameters")]
    public float StunDuration = 3.0f;
    public bool isStunned = false;

    [Header("Projectile Parameters")]
    public GameObject Projectile;
    public Transform[] muzzleTransforms;

    public EnemySoundManager enemySoundManager;

    //Numerator&CourotineReferences
    public Coroutine checkPlayerCoroutine;
    public IEnumerator searchWalkPointCoroutine;
    //EnemyReferences
    private RangedEnemy rangedEnemy;
    private ExplodingEnemy explodingEnemy;
    private MeleeEnemy meleeEnemy;
    private EvasiveEnemy evasiveEnemy;
    //StateReferences
    protected EnemyState currentState;
    //Constants
    private float checkInterval = 0.1f;
    private float rotationSpeed = 360;
    public EnemyState CurrentState
    {
        get { return currentState; }
        private set { currentState = value; }
    }
    private void Awake()
    {
        checkPlayerCoroutine = StartCoroutine(CheckPlayerPresence());
        GameObject playerGameObject = GameObject.Find("Player");
        agent = GetComponent<NavMeshAgent>();
        health = GetComponent<EnemyHealth>();
        EnemySoundManager enemySoundManager = GetComponent<EnemySoundManager>();

        if (playerGameObject != null)
        {
            player = playerGameObject.transform;
            playerCollider = playerGameObject.GetComponent<CapsuleCollider>();
            if (playerCollider == null) // Check if the CapsuleCollider component is present on the Player
            {
                Debug.LogError("CapsuleCollider not found on the Player GameObject!");
            }
        }
        else
        {
            Debug.LogError("Player GameObject not found!");
        }
        if (health != null)
        {
            maxHealth = health.maxHealth;
        }
        else
        {
            Debug.LogError("EnemyHealth component not found!");
        }

        if (enemyRigidbody == null)
        {
            enemyRigidbody = GetComponent<Rigidbody>();
            if (enemyRigidbody == null)
            {
                Debug.LogError("Rigidbody component not found!");
            }
        }

        // Ensure Collider components are assigned
        enemyColliders = GetComponentsInChildren<Collider>();
        if (enemyColliders == null || enemyColliders.Length == 0)
        {
            Debug.LogError("No Collider components found in children!");
        }

    }

    private void Update()
    {
        switch (currentState)
        {
            case EnemyState.WaitingToBeSpawned:
                break;
            case EnemyState.Patrol:
                break;
            case EnemyState.Chase:
                break;
            case EnemyState.Attack:
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
                break;
        }
    }
    public void ChangeState(EnemyState newState)
    {
        EnemySoundManager enemySoundManager = GetComponent<EnemySoundManager>();
        currentState = newState;
        if (currentState == EnemyState.WaitingToBeSpawned)
        {
            ChangeState(EnemyState.Decision);
        }
        if (currentState == EnemyState.Relocating)
        {
            EvasiveEnemy evasiveEnemy = GetComponent<EvasiveEnemy>();
            RangedEnemy rangedEnemy = GetComponent<RangedEnemy>();
            if (rangedEnemy != null)
            {
                agent.isStopped = false;
                agent.speed = 2f;
                rangedEnemy.Relocate();
            }
            if (evasiveEnemy != null)
            {
                agent.isStopped = false;
                agent.speed = 20f;
                evasiveEnemy.Relocate();
            }
            
        }

        if (currentState == EnemyState.ThrowingGrenade)
        {
            RangedEnemy rangedEnemy = GetComponent<RangedEnemy>();
            if (rangedEnemy != null)
            {
                rangedEnemy.ThrowGrenade();
            }
        }
        if (currentState == EnemyState.Patrol)
        {
            agent.isStopped = false;
            agent.speed = patrolSpeed;
            Patroling();
            enemySoundManager.PlayEnemyLocateSound();
        }
        if (currentState == EnemyState.Chase)
        {
            agent.isStopped = false;
            agent.speed = chaseSpeed;
            ChasePlayer();
            enemySoundManager.PlayEnemySearchSound();
        }
        if (currentState == EnemyState.Attack)
        {
            enemySoundManager.PlayEnemyTargetSightedSound();
            SelectAttackMethod();
            //Debug.Log("AttackStateCalled");
        }
        if (currentState == EnemyState.Decision)
        {
            agent.isStopped = true;
            MakeDecision();
        }
        if (currentState == EnemyState.Stun)
        {
            isStunned = true;
            agent.isStopped = true;
            StartCoroutine(RecoverFromStun(StunDuration));
        }
        if (currentState == EnemyState.KnockBack)
        {
            agent.isStopped = true;
            RotateTowardsPlayer();
            StartCoroutine(RecoverFromKnockBack());
        }

        if (currentState == EnemyState.Die)
        {
            StopAllCoroutines();
            agent.isStopped = true;
            sightRange = 0;
            attackRange = 0;
            StartCoroutine(DieAfterBufferTime());
            enemySoundManager.PlayEnemyDeathSound();
        }

    }

    private void Patroling()
    {
        if (!playerInSightRange)
        {
            if (!walkpointSet)
                SearchWalkPoint();

            if (walkpointSet)
                agent.SetDestination(walkpoint);

            Vector3 distanceToWalkPoint = transform.position - walkpoint;

            if (distanceToWalkPoint.magnitude < 1f)
                walkpointSet = false;
            StartCoroutine(ReturnToDecisionSlow());
        }
        
      
    }
    public void SearchWalkPoint()
    {
        if (searchWalkPointCoroutine == null)
        {
            searchWalkPointCoroutine = SearchWalkPointCoroutine();
            StartCoroutine(searchWalkPointCoroutine);
        }
    }
    public void MakeDecision()
    {
        EvasiveEnemy evasiveEnemy = GetComponent<EvasiveEnemy>();

        if (evasiveEnemy != null)
        {
            if (playerInAttackRange && !evasiveEnemy.playerInEvasionRange)
            {
                //Debug.Log("Transition to Attack State");
                ChangeState(EnemyState.Attack);
            }
            else if (playerInSightRange && !playerInAttackRange)
            {
                //Debug.Log("Transition to Chase State");
                ChangeState(EnemyState.Chase);
            }
            else if (!playerInSightRange)
            {
                //Debug.Log("Transition to Patrol State");
                ChangeState(EnemyState.Patrol);
            }
            else if (evasiveEnemy.playerInEvasionRange)
            {
                ChangeState(EnemyState.Relocating);
            }
        }

        else if (evasiveEnemy == null)
        {
            if (playerInAttackRange)
            {
                //Debug.Log("Transition to Attack State");
                ChangeState(EnemyState.Attack);
            }
            else if (playerInSightRange && !playerInAttackRange)
            {
                //Debug.Log("Transition to Chase State");
                ChangeState(EnemyState.Chase);
            }
            else if (!playerInSightRange)
            {
                //Debug.Log("Transition to Patrol State");
                ChangeState(EnemyState.Patrol);
            }
        }
      
    }

    public void SelectAttackMethod()
    {
        RangedEnemy rangedEnemy = GetComponent<RangedEnemy>();
        MeleeEnemy meleeEnemy = GetComponent<MeleeEnemy>();
        ExplodingEnemy explodingEnemy = GetComponent<ExplodingEnemy>();
        EvasiveEnemy evasiveEnemy = GetComponent<EvasiveEnemy>();

        if (!playerInAttackRange)
        {
            MakeDecision();
            return;
        }

        if (rangedEnemy != null)
        {
            //Debug.Log("RangedEnemyAttackCalled");
            rangedEnemy.AttackPlayer();
            return;
        }

        if (meleeEnemy != null)
        {
            //Debug.Log("MeleeEnemyAttackCalled");
            meleeEnemy.AttackPlayer();
            return;
        }

        if (explodingEnemy != null)
        {
            explodingEnemy.AttackPlayer();
            return;
        }

        if (evasiveEnemy != null)
        {
            evasiveEnemy.AttackPlayer();
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
        gameObject.SetActive(false);
    }

    public void ChasePlayer()
    {
        if (playerInSightRange && !playerInAttackRange)
        {
            agent.SetDestination(player.position);
            StartCoroutine(ReturnToDecision());
        }
    }
    IEnumerator DieAfterBufferTime()
    {

        ExplodingEnemy explodingEnemy = GetComponent<ExplodingEnemy>();
        if (explodingEnemy != null)
        {
            Die();
            enemySoundManager.PlayEnemyDeathSound();
        }
        else
        {
            yield return new WaitForSeconds(DieAnimationClip.length);
            enemySoundManager.PlayEnemyDeathSound();
            Die();
        }
    }
    void DropLoot()
    {
        if (lootSystem != null)
        {
            Vector3 offset = new Vector3(0f, 1f, 0f);
            lootSystem.DropItems(gameObject, offset);
        }
    }

    protected void RotateTowardsPlayer()
    {
        agent.isStopped = false;
        agent.speed = chaseSpeed;
        agent.SetDestination(player.position);
    }

    public IEnumerator ReturnToDecision()
    {
        yield return new WaitForSeconds(0.2f);
        ChangeState(EnemyState.Decision);
    }

    public IEnumerator ReturnToDecisionSlow()
    {
        yield return new WaitForSeconds(2f);
        ChangeState(EnemyState.Decision);
    }
    
    private void OnDrawGizmosSelected()
    {
        // Draw sight range sphere
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);

        // Draw attack range sphere
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Draw Evade Range
        EvasiveEnemy evasiveEnemy = GetComponent<EvasiveEnemy>();
        if (evasiveEnemy != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, evasiveEnemy.EvasionRange);
        }

    }
    
    private IEnumerator RecoverFromStun(float StunDuration)
    {
        yield return new WaitForSeconds(StunDuration);
        isStunned = false;
        agent.isStopped = false;
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
            float randomZ = Random.Range(-walkpointRange, walkpointRange);
            float randomX = Random.Range(-walkpointRange, walkpointRange);

            walkpoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

            NavMeshHit hit;
            if (NavMesh.SamplePosition(walkpoint, out hit, 2f, NavMesh.AllAreas))
            {
                
                walkpoint = hit.position;
                walkpointSet = true;

                yield return new WaitUntil(() => Vector3.Distance(transform.position, walkpoint) <= 0.5f);

                walkpointSet = false; 
            }

            yield return new WaitForSeconds(CheckPointFrequency);
        }
    }
    private IEnumerator CheckPlayerPresence()
    {
        //Debug.Log("CheckPlayerPresence coroutine started.");
        EvasiveEnemy evasiveEnemy = GetComponent<EvasiveEnemy>();

        while (true)
        {
            if (evasiveEnemy != null)
            {
                evasiveEnemy.playerInEvasionRange = Physics.CheckSphere(transform.position, evasiveEnemy.EvasionRange, PlayerLayer);
            }
            playerInSightRange = Physics.CheckSphere(transform.position, sightRange, PlayerLayer);
            playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, PlayerLayer);
            yield return new WaitForSeconds(checkInterval);
        }
    }
    
    private IEnumerator RecoverFromKnockBack()
    {
        yield return new WaitForSeconds(2.0f);
        agent.isStopped = false;
        ChangeState(EnemyState.Decision);
    }
}