using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    public int damage = 10;
    public float damageCooldownTime = 1f; // Cooldown time between consecutive damage dealing
    public LayerMask targetLayers; // LayerMask to specify which layers to check for collisions

    private Collider weaponCollider;
    private bool canDealDamage = true; // Flag to track if the weapon can deal damage
    [SerializeField] private bool IsHeavy;
    [SerializeField] private float knockbackForce;
    [SerializeField] private float knockbackbuffer;


    private void Start()
    {
        weaponCollider = GetComponent<Collider>();
    }

    // Called when the trigger collider attached to the weapon GameObject overlaps with another collider
    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & targetLayers) == 0) return; // Early exit if the object isn't in the target layer

        if (other.CompareTag("Enemy"))
        {
            // Logic for when hitting an enemy
            EnemyHealth targetHealth = other.GetComponentInParent<EnemyHealth>();
            if (targetHealth != null && canDealDamage)
            {
                targetHealth.TakeDamage(damage);
                ApplyKnockbackIfHeavy(other);
                ProcessAttackEnd();
            }
        }
        else if (other.CompareTag("Player"))
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
        weaponCollider.enabled = false;
        Invoke("ResetDamageCooldown", damageCooldownTime);
    }

    private void ResetDamageCooldown()
    {
        canDealDamage = true;
        weaponCollider.enabled = true;
    }
}