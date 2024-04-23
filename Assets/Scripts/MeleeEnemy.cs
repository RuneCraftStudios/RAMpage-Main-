using System.Collections;
using UnityEngine;

public class MeleeEnemy : EnemyAiTutorial
{
    [Header("Melee Enemy Settings")]
    public Collider WeaponCollider; // Single collider for melee attacks
    public int damage = 10;
    public LayerMask targetLayers;
    public bool IsHeavy;
    public float knockbackForce;
    public AnimationClip AttackAnimation;
    private bool canDealDamage = true; // Flag to control damage for the weapon

    public void AttackPlayer()
    {
        if (!playerInAttackRange)
        {
            return;
        }
        StartCoroutine(ChangeStateAfterAttack());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & targetLayers) == 0) return; // Exit if not the target layer
        if (!WeaponCollider.enabled || !canDealDamage) return; // Damage only if the collider is active and damage is allowed

        if (other.CompareTag("Player"))
        {
            Health playerHealth = other.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                ApplyKnockbackIfHeavy(other);
                canDealDamage = false; // Prevent further damage until reset
            }
        }
    }

    private void ApplyKnockbackIfHeavy(Collider other)
    {
        if (!IsHeavy) return;
        CharacterController playerController = other.GetComponentInParent<CharacterController>();
        if (playerController != null)
        {
            Vector3 knockbackDirection = (other.transform.position - transform.position).normalized;
            StartCoroutine(ApplyKnockback(playerController, knockbackDirection * knockbackForce, 0.5f));
        }
    }

    IEnumerator ApplyKnockback(CharacterController controller, Vector3 knockback, float duration)
    {
        float time = 0;
        while (time < duration)
        {
            controller.Move(knockback * Time.deltaTime);
            time += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator ChangeStateAfterAttack()
    {
        yield return new WaitForSeconds(AttackAnimation.length);
        ResetDamageCapability(); // Reset damage capability after the attack animation is complete
        ChangeState(EnemyState.Decision);
    }

    private void ResetDamageCapability()
    {
        canDealDamage = true; // Re-enable damage capability
    }
}
