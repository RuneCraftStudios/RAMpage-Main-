using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplerProjectile : MonoBehaviour
{
    [Header("Damage Settings")]
    public int damage = 10;

    [Header("Explosion Settings")]
    [SerializeField] private bool explodeOnCollision = false;
    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private int explosionDamage = 20;
    [SerializeField] private LayerMask explosionLayers;
    [SerializeField] private GameObject explosionParticles;

    [Header("Target Layer")]
    public LayerMask targetLayer; // New variable for target layer
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collision Occurred");
        if (((1 << other.gameObject.layer) & targetLayer) != 0)
        {
            Health playerHealth = null;
            EnemyHealth enemyHealth = null;

            // Check if the collided GameObject is the player
            if (targetLayer == (targetLayer | (1 << other.gameObject.layer)) && other.gameObject.CompareTag("Player"))
            {
                Debug.Log("Player Identified");
                playerHealth = other.GetComponent<Health>();
                playerHealth.TakeDamage(damage);
            }
            // Check if the collided GameObject is an enemy
            else if (targetLayer == (targetLayer | (1 << other.gameObject.layer)) && other.gameObject.CompareTag("Enemy"))
            {
                enemyHealth = other.GetComponent<EnemyHealth>();
                enemyHealth.TakeDamage(damage);
            }
            // Check if explosion is enabled, and destroy the projectile accordingly
            if (explodeOnCollision)
            {
                Explode();
            }
            else
            {
                DestroyProjectile();
            }
        }
        // Destroy the projectile if it collides with any other object
        else
        {
            DestroyProjectile();
        }
    }

    void DestroyProjectile()
    {
        Destroy(gameObject);
    }
    private void Explode()
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

        if (explosionParticles != null)
        {
            Instantiate(explosionParticles, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }

}
