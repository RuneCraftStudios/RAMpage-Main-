using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : Projectile
{
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collision Occurred");
        if (((1 << other.gameObject.layer) & targetLayer) != 0)
        {
            Health playerHealth = null;

            if (targetLayer == (targetLayer | (1 << other.gameObject.layer)) && other.gameObject.CompareTag("Player"))
            {
                Debug.Log("Player Identified");
                playerHealth = other.GetComponent<Health>();
                playerHealth.TakeDamage(damage);
                DestroyProjectile();
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
