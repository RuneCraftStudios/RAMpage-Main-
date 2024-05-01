using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMeleeWeapon : MonoBehaviour
{
    public int damage = 10;
    public bool IsHeavy;
    public float knockbackForce;
    private bool canDealDamage = true;

    private void Awake()
    {
        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            Debug.Log("ColliderFound");
        }
        if (collider == null)
        {
            Debug.Log("ColliderCannotBeIdentified");
        }
    }
    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            Debug.Log("PlayerIdentified");
            Health playerHealth = other.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                ApplyKnockbackIfHeavy(other);

            }
        }
    }
    private void ApplyKnockbackIfHeavy(Collider other)
    {
        if (!IsHeavy) return;
        CharacterController playerController = other.GetComponentInParent<CharacterController>();
        if (playerController != null)
        {
            Vector3 knockbackDirection = (other.transform.position - transform.position).normalized;
            StartCoroutine(ApplyKnockback(playerController, knockbackDirection * knockbackForce, 0.5f));
        }
    }

    IEnumerator ApplyKnockback(CharacterController controller, Vector3 knockback, float duration)
    {
        float time = 0;
        while (time < duration)
        {
            controller.Move(knockback * Time.deltaTime);
            time += Time.deltaTime;
            yield return null;
        }
    }
}
