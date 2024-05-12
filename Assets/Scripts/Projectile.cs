using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float speed = 10f;
    public float accuracy = 5f;
    public float maxDistance = 100f;
    public ParticleSystem ImpactEffect;
    public float MaxLife;
    public LayerMask CollusionLayers;

    [Header("Damage Settings")]
    public int damage = 10;

    [Header("Explosion Settings")]
    [SerializeField] protected private bool explodeOnCollision = false;
    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private int explosionDamage = 20;
    [SerializeField] private LayerMask explosionLayers;
    [SerializeField] private GameObject explosionParticles;

    [Header("Target Layer")]
    public LayerMask targetLayer; // New variable for target layer

    private float distanceTraveled = 0f;
    private float LifeSpend = 0f;

    void Start()
    {

        Vector3 randomRotation = Random.insideUnitSphere * accuracy;
        Vector3 randomDirection = Quaternion.Euler(randomRotation) * transform.forward;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = randomDirection * speed;
        }

    }

    void Update()
    {
        distanceTraveled += speed * Time.deltaTime;
        LifeSpend += Time.deltaTime;

        if (distanceTraveled >= maxDistance)
        {
            DestroyProjectile();
        }

        if (LifeSpend >= MaxLife)
        {
            DestroyProjectile();
        }
    }


    public void Explode()
    {
        if (explodeOnCollision)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, explosionLayers);
            foreach (Collider collider in colliders)
            {
                if (((1 << collider.gameObject.layer) & targetLayer) != 0) // Check if the collider's layer is in targetLayer
                {
                    Health targetHealth = collider.GetComponent<Health>();
                    if (targetHealth != null)
                    {
                        targetHealth.TakeDamage(explosionDamage);
                    }
                }
            }
        }
        else
        {
            DestroyProjectile();
        }

        if (explosionParticles != null)
        {
            Instantiate(explosionParticles, transform.position, Quaternion.identity);
        }

        
    }

    public void OnDrawGizmosSelected()
    {
        if (explodeOnCollision)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, explosionRadius);
        }
    }

    public IEnumerator HandleDestroy()
    {
        yield return new WaitForSeconds(1.0f);
        DestroyProjectile();
    }

    public void DestroyProjectile()
    {
        Destroy(gameObject);
    }
}
