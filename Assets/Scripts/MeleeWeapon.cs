using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    public int damage = 10;
    public float damageCooldownTime = 1f; 
    public LayerMask targetLayers; 
    private Collider weaponCollider;
    private bool canDealDamage = true;
    [SerializeField] private bool IsHeavy;
    [SerializeField] private float knockbackForce;
    [SerializeField] private float knockbackbuffer;
    public WeaponSoundManager weaponSoundManager;
    public int EnergyCost;
    public Health health;


    private void Start()
    {
        // Get all colliders attached to the object
        Collider[] colliders = GetComponentsInChildren<Collider>();

        // Check if there are any colliders
        if (colliders.Length > 0)
        {
            // Assign the first collider found to the weaponCollider variable
            weaponCollider = colliders[0];

            // If there are multiple colliders, enable them all
            for (int i = 1; i < colliders.Length; i++)
            {
                colliders[i].enabled = true;
            }
        }
        else
        {
            Debug.LogWarning("No collider found on MeleeWeapon object.");
        }

        GameObject playerGameObject = GameObject.Find("Player");
        if (playerGameObject != null)
        {
            // Get the Health component from the player GameObject
            health = playerGameObject.GetComponent<Health>();
        }
        else
        {
            Debug.LogError("Player GameObject not found!");
        }
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
                weaponSoundManager.PlayWeaponImpactClips();
                ApplyKnockbackIfHeavy(other);
                ProcessAttackEnd();
                health.DepleteEnergy(EnergyCost);
         
            }
        }
        else if (other.CompareTag("Player"))
        {
            // Logic for when hitting the player
            Health playerHealth = other.GetComponentInParent<Health>();
            if (playerHealth != null && canDealDamage)
            {
                playerHealth.TakeDamage(damage);
                weaponSoundManager.PlayWeaponImpactClips();
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