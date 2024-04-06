using UnityEngine;

public class RaycastBullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    public float damage = 10f; // Damage dealt by the bullet
    public float maxDistance = 100f; // Maximum distance the bullet can travel
    public bool isExplosive = false; // Flag to indicate if the bullet has an explosive effect

    [Header("Explosion Settings")]
    public float explosionRadius = 5f; // Radius of the explosion
    public float explosionForce = 500f; // Force of the explosion

    [Header("Direction")]
    [SerializeField] private GameObject directionObject; // Object responsible for the direction of the raycast

    [Header("Target Layer")]
    [SerializeField] private LayerMask targetLayer; // Layer to be considered as targets

    private ElementalEffect elementalEffect; // Reference to the ElementalEffect component

    private void Start()
    {
        // Get the ElementalEffect component attached to the bullet
        elementalEffect = GetComponent<ElementalEffect>();
        if (elementalEffect == null)
        {
            Debug.LogError("ElementalEffect component not found on the bullet object.");
            return;
        }

        // Perform the raycast when the bullet is spawned
        ShootRaycast();
    }

    public void ShootRaycast()
    {
        // Ensure the directionObject is not null
        if (directionObject == null)
        {
            Debug.LogError("Direction object is null.");
            return;
        }

        // Get the direction from the specified GameObject's transform
        Vector3 direction = directionObject.transform.forward;

        // Perform a raycast in the specified direction
        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, maxDistance, targetLayer))
        {
            // Check if the raycast hits a collider with a Health component
            Health targetHealth = hit.collider.GetComponent<Health>();
            if (targetHealth != null)
            {
                // Check if the target has shields
                if (targetHealth.CurrentShield > 0)
                {
                    // Deal damage to shields
                    targetHealth.TakeDamage((int)damage, applyToShield: true);
                }
                else
                {
                    // If no shields, deal damage to health
                    targetHealth.TakeDamage((int)damage);
                }

                // Log a debug message indicating that the target was hit
                Debug.Log("Target hit: " + hit.collider.name);
            }

            // Check if the bullet has an explosive effect
            if (isExplosive)
            {
                // Apply explosive effect
                Explode(hit.point);
            }

            // Apply elemental effect
            elementalEffect.RaycastCollision(hit.point);
        }
    }

    private void Explode(Vector3 explosionPoint)
    {
        // Find all colliders in the explosion radius
        Collider[] colliders = Physics.OverlapSphere(explosionPoint, explosionRadius);

        // Apply explosion force to all rigidbodies within the explosion radius
        foreach (Collider col in colliders)
        {
            Rigidbody rb = col.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, explosionPoint, explosionRadius, 0f, ForceMode.Impulse);
            }

            // Check if the collider has a Health component to apply damage
            Health targetHealth = col.GetComponent<Health>();
            if (targetHealth != null)
            {
                // Apply damage to the target's health
                targetHealth.TakeDamage((int)damage);
            }
        }

        // Optional: Instantiate visual and sound effects for explosion
        // Instantiate(explosionEffectPrefab, explosionPoint, Quaternion.identity);
        // Instantiate(explosionSoundPrefab, explosionPoint, Quaternion.identity);
    }
}
