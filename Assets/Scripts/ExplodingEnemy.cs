using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ExplodingEnemy : EnemyAiTutorial
{
    [Header("Exploding Enemy Settings")]
    public ParticleSystem ExplosionEffect;
    public int ExplosionDamage;
    private bool canExplode = false;
    private EnemySoundManager soundManager;
    public float chargeSpeed;

    public void AttackPlayer()
    {
        agent.isStopped = true;
        agent.SetDestination(player.position);
        StartCoroutine(AttackAfterChargeDelay());
    }


    private void OnTriggerEnter(Collider other)
    {
        if (canExplode && other.gameObject.CompareTag("Player"))
        {
            Health playerHealth = other.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(ExplosionDamage);
                Instantiate(ExplosionEffect, transform.position, Quaternion.identity);
                enemySoundManager.PlayEnemyExplodeSound();
                ChangeState(EnemyState.Die);
            }
        }
    }
    public IEnumerator AttackAfterChargeDelay()
    {
        Debug.Log("ChargeActionCalled");
        yield return new WaitForSeconds(2.0f);
        agent.isStopped = false;
        agent.speed = chargeSpeed;
        canExplode = true;
        enemySoundManager.PlayEnemyChargeSound();



    }
}
