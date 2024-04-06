using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class MeleeWeapon : MonoBehaviour
{
    public int damage = 10;
    public float damageCooldownTime = 1f; // Cooldown time between consecutive damage dealing

    private Collider weaponCollider;
    private bool canDealDamage = true; // Flag to track if the weapon can deal damage
    [SerializeField] private bool IsHeavy;
    [SerializeField] private float knockbackForce;
    [SerializeField] private float knockbackbuffer;
    public EnemyAiTutorial enemyAi;
    

    private void Start()
    {
        weaponCollider = GetComponent<Collider>();
    }

    // Called when the trigger collider attached to the weapon GameObject overlaps with another collider
    private void OnTriggerEnter(Collider other)
    {
        // Check if the collider or its parent GameObject has a Health component
        Health targetHealth = other.GetComponentInParent<Health>();

        if (targetHealth != null && canDealDamage)
        {
            // Apply damage to the collided GameObject's Health component
            targetHealth.TakeDamage(damage);
            if (IsHeavy)
            {
                // Find the EnemyAiTutorial component on the root GameObject of the collider
                EnemyAiTutorial enemyAi = other.GetComponentInParent<EnemyAiTutorial>();

                // If EnemyAiTutorial component is found, proceed with knockback
                if (enemyAi != null)
                {
                    // Calculate the knockback direction
                    Vector3 knockbackDirection = (other.transform.position - transform.position).normalized;

                    // Apply knockback force in the opposite direction
                    Rigidbody enemyRigidbody = other.GetComponentInParent<Rigidbody>();
                    if (enemyRigidbody != null)
                    {
                        enemyRigidbody.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);
                        enemyAi.KnockBack();
                    }
                }
            }
            canDealDamage = false; // Disable dealing damage temporarily
            weaponCollider.enabled = false; // Deactivate the collider
            Invoke("ResetDamageCooldown", damageCooldownTime); // Invoke the method to reset the damage cooldown
        }
    }

    

    // Method to reset the damage cooldown
    private void ResetDamageCooldown()
    {
        canDealDamage = true; // Enable dealing damage
        weaponCollider.enabled = true; // Reactivate the collider
    }
}
