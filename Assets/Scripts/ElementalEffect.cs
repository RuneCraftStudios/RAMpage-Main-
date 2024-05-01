using UnityEngine;

public class ElementalEffect : MonoBehaviour
{
    public enum ElementType { None, Fire, Electricity }

    public ElementType elementType = ElementType.None;
    public float effectDuration = 3f; // Duration of elemental effect
    public int damageOverTime = 5; // Base damage per second
    [Range(0f, 1f)]
    public float effectChance = 1f; // Chance of elemental effect


    private Health playerHealth;
    private EnemyHealth enemyHealth; // Assume you have an EnemyHealth script

    // Method to simulate OnTriggerEnter for raycasts
    public void RaycastCollision(Vector3 hitPoint)
    {
        Collider[] colliders = Physics.OverlapSphere(hitPoint, 0.1f);
        foreach (Collider collider in colliders)
        {
            ApplyEffectBasedOnTag(collider);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        ApplyEffectBasedOnTag(other);
    }

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

    private void ApplyElementalEffect()
    {
        if (elementType == ElementType.Fire || elementType == ElementType.Electricity)
        {
            if (playerHealth != null)
            {
                playerHealth.StartDamageOverTime(damageOverTime, effectDuration);
                // Assume player damage text management is handled separately
            }
            else if (enemyHealth != null)
            {
                enemyHealth.StartDamageOverTime(damageOverTime, effectDuration);
            }
        }
    }


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

    private void ApplyEffectToEnemyHealth(EnemyHealth enemyHealthComponent)
    {
        if (enemyHealthComponent != null && Random.value <= effectChance)
        {
            ApplyElementalEffect();
        }
        enemyHealth = null; // Clear reference
    }
}
