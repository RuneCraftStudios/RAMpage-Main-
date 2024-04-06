using UnityEngine;

public class ElementalEffect : MonoBehaviour
{
    public enum ElementType { None, Fire, Electricity }

    public ElementType elementType = ElementType.None;
    public float effectDuration = 3f; // Duration of elemental effect
    public int damageOverTime = 5; // Base damage per second
    [Range(0f, 1f)]
    public float effectChance = 1f; // Chance of elemental effect (100%)

    private Health targetHealth; // Reference to target's Health component

    // Method to simulate OnTriggerEnter for raycasts
    public void RaycastCollision(Vector3 hitPoint)
    {
        // Perform a sphere cast to find colliders at the hit point
        Collider[] colliders = Physics.OverlapSphere(hitPoint, 0.1f);
        foreach (Collider collider in colliders)
        {
            // Get the Health component of the collided GameObject
            targetHealth = collider.GetComponent<Health>();
            if (targetHealth != null)
            {
                // Check if elemental effect can be applied
                if (Random.value <= effectChance)
                {
                    if (elementType == ElementType.Fire && targetHealth.CurrentShield == 0)
                    {
                        // Apply Ignite effect
                        ApplyElementalEffect();
                    }
                    else if (elementType == ElementType.Electricity)
                    {
                        // Apply Electrify effect
                        ApplyElementalEffect();
                    }
                }
            }
        }
    }

    // Called when the projectile collides with another collider
    void OnTriggerEnter(Collider other)
    {
        // Get the Health component of the collided GameObject
        targetHealth = other.GetComponent<Health>();

        if (targetHealth != null)
        {
            // Check if elemental effect can be applied
            if (Random.value <= effectChance)
            {
                if (elementType == ElementType.Fire && targetHealth.CurrentShield == 0)
                {
                    // Apply Ignite effect
                    ApplyElementalEffect();
                }
                else if (elementType == ElementType.Electricity)
                {
                    // Apply Electrify effect
                    ApplyElementalEffect();
                }
            }
        }
    }

    // Method to apply elemental effect
    public void ApplyElementalEffect()
    {
        if (elementType == ElementType.Fire)
        {
            targetHealth.IsIgnited = true; // Set the IsIgnited boolean to true
            targetHealth.StartCoroutine(targetHealth.DamageOverTimeCoroutine(damageOverTime, Mathf.RoundToInt(effectDuration))); // Cast effectDuration to int
            targetHealth.StartCoroutine(targetHealth.DisableIgnitedEffect(Mathf.RoundToInt(effectDuration))); // Cast effectDuration to int
        }
        else if (elementType == ElementType.Electricity)
        {
            targetHealth.IsElectrified = true; // Set the IsElectrified boolean to true
            targetHealth.StartCoroutine(targetHealth.DamageOverTimeCoroutine(damageOverTime, Mathf.RoundToInt(effectDuration))); // Cast effectDuration to int
            targetHealth.StartCoroutine(targetHealth.DisableElectrifiedEffect(Mathf.RoundToInt(effectDuration))); // Cast effectDuration to int
        }
    }
}