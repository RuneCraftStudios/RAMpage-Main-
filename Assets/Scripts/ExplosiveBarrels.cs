using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveBarrels : MonoBehaviour
{
    [SerializeField] private Collider explosionCollider;
    [SerializeField] private int explosionRadius;
    [SerializeField] private int explosionDamage;
    [SerializeField] GameObject ExplosionEffect;
    [SerializeField] private float ExplosionEffectDuration;
    [SerializeField] private AudioSource AudioSource;
    [SerializeField] private AudioClip ExplosionClip;
    [SerializeField] private MeshRenderer MeshRenderer;
    
    private void OnTriggerEnter(Collider other)
    {
       Explode();
       Debug.Log("ExplodeCalled");
    }

    private void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(explosionCollider.transform.position, explosionRadius);

        foreach (Collider collider in colliders)
        {
            Transform parentTransform = collider.transform.parent; // Get the parent transform
            if (parentTransform != null)
            {
                if (parentTransform.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                {
                    EnemyHealth enemyHealth = parentTransform.GetComponent<EnemyHealth>();
                    if (enemyHealth != null && collider.CompareTag("Explosion"))
                    {
                        // Deal damage to the enemy if it has the "Explosive" tag
                        enemyHealth.TakeDamage(explosionDamage);
                    }
                }
                else if (parentTransform.gameObject.layer == LayerMask.NameToLayer("Player"))
                {
                    Health playerHealth = parentTransform.GetComponent<Health>();
                    if (playerHealth != null && collider.CompareTag("Explosion"))
                    {
                        // Deal damage to the player if it has the "Explosive" tag
                        playerHealth.TakeDamage(explosionDamage);
                    }
                }
            }
        }

        MeshRenderer.enabled = false;
        InstantiateEffect();
        AudioSource.PlayOneShot(ExplosionClip);
        StartCoroutine(Delete());
    }

    private void OnDrawGizmosSelected()
    {
        // Set the color of the wireframe sphere
        Gizmos.color = Color.red;

        // Draw the wireframe sphere representing the explosion radius
        Gizmos.DrawWireSphere(explosionCollider.transform.position, explosionRadius);
    }

    private void InstantiateEffect()
    {
        Instantiate(ExplosionEffect, AudioSource.transform.position, AudioSource.transform.rotation);
    }
    private IEnumerator Delete()
    {
        yield return new WaitForSeconds(3.0f);
        Destroy(gameObject);
    }

}
