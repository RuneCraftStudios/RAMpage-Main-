using UnityEngine;

public class EnemyAnimationChanger : MonoBehaviour
{
    private EnemyAiTutorial enemyAi; // Reference to the EnemyAiTutorial script
    [SerializeField] private Animator animator;

    private EnemyState currentAnimationState;
    private EnemyState previousAnimationState;
    private float blendTimer = 0.25f;

    void Awake()
    {
        // Get the EnemyAiTutorial component
        enemyAi = GetComponent<EnemyAiTutorial>();

        // Ensure all components are assigned properly
        if (enemyAi == null)
            Debug.LogError("EnemyAiTutorial component not found!");
        if (animator == null)
            Debug.LogError("Animator component not found!");
    }

    void Update()
    {
        // Update the animation state based on the enemy's current state
        UpdateAnimationState(enemyAi.CurrentState);
    }

    void UpdateAnimationState(EnemyState state)
    {
        // Update previous state before changing current state
        previousAnimationState = currentAnimationState;

        // Update current state
        currentAnimationState = state;

        // Manipulate animator states based on enemy states
        switch (state)
        {
            case EnemyState.WaitingToBeSpawned:
                PlayAnimationWithBlend("Patrol");
                break;
            case EnemyState.Patrol:
                PlayAnimationWithBlend("Patrol");
                break;
            case EnemyState.Chase:
                PlayAnimationWithBlend("Chase");
                break;
            case EnemyState.Attack:
                PlayAnimationWithBlend("Attack");
                break;
            case EnemyState.Stun:
                PlayAnimationWithBlend("Stun");
                break;
            case EnemyState.KnockBack:
                PlayAnimationWithBlend("KnockBack");
                break;
            case EnemyState.ThrowingGrenade:
                PlayAnimationWithBlend("ThrowGrenade");
                break;
            case EnemyState.Relocating:
                PlayAnimationWithBlend("Patrol");
                break;
            case EnemyState.Die:
                PlayAnimationWithBlend("Die");
                break;
            default:
                // Handle any other states if needed
                break;
        }
    }

    void PlayAnimationWithBlend(string animationName)
    {
        if (currentAnimationState != previousAnimationState)
        {
            animator.CrossFadeInFixedTime(animationName, blendTimer);
        }
       
    }
}
