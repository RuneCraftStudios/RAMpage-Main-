using UnityEngine;

public class EnemyGrenade : MonoBehaviour
{
    public string playerTag = "Player"; // Tag for the player GameObject
    public float throwSpeed = 10f;
    public float gravity = 9.8f;
    public int explosionDamage;
    public float explosionRadius;
    public LayerMask explosionLayers;
    public LayerMask targetLayer;
    public GameObject ExplosionEffect;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);

        if (player != null)
        {
            Transform playerTransform = player.transform;
            ThrowGrenade(playerTransform);
        }
        else
        {
            Debug.LogWarning("Player GameObject not found. Make sure to tag your player GameObject with the specified tag.");
        }
    }

    void ThrowGrenade(Transform targetTransform)
    {
        Vector3 startPos = transform.position;
        Vector3 targetPos = targetTransform.position;
        Vector3 direction = (targetPos - startPos).normalized;
        float distance = Vector3.Distance(startPos, targetPos);
        float timeToReach = distance / throwSpeed;
        Vector3 initialVelocity = direction * throwSpeed;
        float initialUpwardVelocity = (targetPos.y - startPos.y + 0.5f * gravity * timeToReach * timeToReach) / timeToReach;
        initialVelocity.y = initialUpwardVelocity;
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.velocity = initialVelocity;
        transform.LookAt(targetPos);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == explosionLayers)
        {
            Explode();
        }
        else if (other.gameObject.layer == targetLayer)
        {
            Health targetHealth = other.GetComponent<Health>();
            if (targetHealth != null)
            {
                targetHealth.TakeDamage(explosionDamage);
            }
            Explode();
        }
    }

    void Explode()
    {
        if (ExplosionEffect != null)
        {
            Instantiate(ExplosionEffect, transform.position, Quaternion.identity);
        }

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, targetLayer);
        foreach (Collider collider in colliders)
        {
            Health targetHealth = collider.GetComponent<Health>();
            if (targetHealth != null)
            {
                targetHealth.TakeDamage(explosionDamage);
            }
        }

        Destroy(gameObject);
    }
}
