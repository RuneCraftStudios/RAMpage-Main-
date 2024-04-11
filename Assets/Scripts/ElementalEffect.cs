using UnityEngine;

public class ElementalEffect : MonoBehaviour
{
    public enum ElementType { None, Fire, Electricity }

    public ElementType elementType = ElementType.None;
    public float effectDuration = 3f; // Duration of elemental effect
    public int damageOverTime = 5; // Base damage per second
    [Range(0f, 1f)]
    public float effectChance = 1f; // Chance of elemental effect (100%)

    // References to Health components
    private Health playerHealth;
    private EnemyHealth enemyHealth; // Assume you have an EnemyHealth script similar to Health

    // Method to simulate OnTriggerEnter for raycasts
    public void RaycastCollision(Vector3 hitPoint)
    {
        // Perform a sphere cast to find colliders at the hit point
        Collider[] colliders = Physics.OverlapSphere(hitPoint, 0.1f);
        foreach (Collider collider in colliders)
        {
            ApplyEffectBasedOnTag(collider);
        }
    }

    // Called when the projectile collides with another collider
    void OnTriggerEnter(Collider other)
    {
        ApplyEffectBasedOnTag(other);
    }

    // Method to apply elemental effect based on tag
    private void ApplyEffectBasedOnTag(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            playerHealth = collider.GetComponent<Health>();
            ApplyEffectToHealth(playerHealth);
        }
        else if (collider.CompareTag("Enemy"))
        {
            enemyHealth = collider.GetComponent<EnemyHealth>();
            ApplyEffectToEnemyHealth(enemyHealth);
        }
    }

    // Method to apply elemental effect
    private void ApplyElementalEffect()
    {
        if (elementType == ElementType.Fire || elementType == ElementType.Electricity)
        {
            // This method is kept generic for both player and enemy
            // Directly start DamageOverTime
            if (playerHealth != null)
            {
                playerHealth.StartDamageOverTime(damageOverTime, effectDuration);
            }
            else if (enemyHealth != null)
            {
                enemyHealth.StartDamageOverTime(damageOverTime, effectDuration);
            }
        }
    }

    // Apply the effect if conditions are met and clear health references
    private void ApplyEffectToHealth(Health healthComponent)
    {
        if (healthComponent != null && Random.value <= effectChance)
        {
            if ((elementType == ElementType.Fire && healthComponent.CurrentShield == 0) || elementType == ElementType.Electricity)
            {
                ApplyElementalEffect();
            }
        }
        playerHealth = null; // Clear reference
    }

    // Similar to ApplyEffectToHealth but for enemy health component
    private void ApplyEffectToEnemyHealth(EnemyHealth enemyHealthComponent)
    {
        if (enemyHealthComponent != null && Random.value <= effectChance)
        {
            // Assume enemy health has similar functionality or adapt as needed
            ApplyElementalEffect();
        }
        enemyHealth = null; // Clear reference
    }
}
