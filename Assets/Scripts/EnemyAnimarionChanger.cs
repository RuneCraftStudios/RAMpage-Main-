using UnityEngine;

public class EnemyAnimationChanger : MonoBehaviour
{
    private EnemyAiTutorial enemyAi; // Reference to the EnemyAiTutorial script
    [SerializeField] private Animator animator;
    private float blendTime = 0.2f; // Adjust this value as needed

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
        // Manipulate animator states based on enemy states
        switch (state)
        {
            case EnemyState.WaitingToBeSpawned:
                // Play the "WaitingToBeSpawned" animation with blending
                animator.CrossFade("WaitingToBeSpawned", blendTime);
                break;
            case EnemyState.Patrol:
                // Play the "Patrol" animation with blending
                animator.CrossFade("Patrol", blendTime);
                break;
            case EnemyState.Chase:
                // Play the "Chase" animation with blending
                animator.CrossFade("Chase", blendTime);
                break;
            case EnemyState.Attack:
                // Play the "Attack" animation with blending
                animator.CrossFade("Attack", blendTime);
                break;
            case EnemyState.Stun:
                // Play the "Stun" animation with blending
                animator.CrossFade("Stun", blendTime);
                break;
            case EnemyState.KnockBack:
                // Play the "KnockBack" animation with blending
                animator.CrossFade("KnockBack", blendTime);
                break;
            case EnemyState.Die:
                // Play the "Die" animation with blending
                animator.CrossFade("Die", blendTime);
                break;
            default:
                // Handle any other states if needed
                break;
        }
    }
}
