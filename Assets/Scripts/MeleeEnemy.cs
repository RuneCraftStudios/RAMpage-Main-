using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : EnemyAiTutorial
{
    public Collider WeaponColliderRight;
    public Collider WeaponColliderLeft;
    public int damage = 10;
    public float damageCooldownTime = 1f; // Cooldown time between consecutive damage dealing
    public LayerMask targetLayers; // LayerMask to specify which layers to check for collisions
    private bool canDealDamage = true; // Flag to track if the weapon can deal damage
    [SerializeField] private bool IsHeavy;
    [SerializeField] private float knockbackForce;
    [SerializeField] private float knockbackbuffer;
    public void AttackPlayer()
    {
        if (!playerInAttackRange)
        {
            ChangeState(EnemyState.Decision);
            return;
        }
        StartCoroutine(ChangeStateAfterAttack());   
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!playerInAttackRange)
        {
            ChangeState(EnemyState.Decision);
        }

        if (((1 << other.gameObject.layer) & targetLayers) == 0) return; // Early exit if the object isn't in the target layer

        if (other.CompareTag("Player"))
        {
            // Logic for when hitting the player
            Health playerHealth = other.GetComponentInParent<Health>();
            if (playerHealth != null && canDealDamage)
            {
                playerHealth.TakeDamage(damage);
                // Apply any player-specific effects here
                ProcessAttackEnd();
            }
        }
    }

    private void ApplyKnockbackIfHeavy(Collider other)
    {
        if (!IsHeavy) return;

        // Knockback logic remains the same
        EnemyAiTutorial enemyAi = other.GetComponentInParent<EnemyAiTutorial>();
        if (enemyAi != null)
        {
            Vector3 knockbackDirection = (other.transform.position - transform.position).normalized;
            Rigidbody enemyRigidbody = other.GetComponentInParent<Rigidbody>();
            if (enemyRigidbody != null)
            {
                enemyRigidbody.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);
                enemyAi.KnockBack();
            }
        }
    }

    private void ProcessAttackEnd()
    {
        canDealDamage = false;
        WeaponColliderLeft.enabled = false;
        WeaponColliderRight.enabled = false;
        Invoke("ResetDamageCooldown", damageCooldownTime);
    }

    private void ResetDamageCooldown()
    {
        canDealDamage = true;
        WeaponColliderLeft.enabled = true;
        WeaponColliderRight.enabled = true;
    }
    private IEnumerator ChangeStateAfterAttack()
    {
        yield return new WaitForSeconds(2.0f);
        ChangeState(EnemyState.Decision);
    }
}
