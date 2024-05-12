using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectile : Projectile
{
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collision Occurred");
        if (((1 << other.gameObject.layer) & targetLayer) != 0)
        {
            EnemyHealth enemyHealth = null;

            if (targetLayer == (targetLayer | (1 << other.gameObject.layer)) && other.gameObject.CompareTag("Enemy"))
            {
                enemyHealth = other.GetComponent<EnemyHealth>();
                enemyHealth.TakeDamage(damage);
                if (ImpactEffect != null)
                {
                    // Get the collision contact point
                    Vector3 collisionPoint = other.ClosestPointOnBounds(transform.position);
                    // Instantiate impact effect at the collision point
                    Instantiate(ImpactEffect, collisionPoint, Quaternion.identity);
                    DestroyProjectile();
                }
            }
        }
        else if (((1 << other.gameObject.layer) | CollusionLayers) != 0) 
        {
            Vector3 collisionPoint = other.ClosestPointOnBounds(transform.position);
            Instantiate(ImpactEffect, collisionPoint, Quaternion.identity);
            DestroyProjectile();
        }

        else
        {
            DestroyProjectile();
        }
    }
}

