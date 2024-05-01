using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyLaser : MonoBehaviour
{
    private bool isRoutineRunning = true;

    void Awake()
    {
        StartCoroutine(LaserDamageRoutine());
        StartCoroutine(StopLaserDamageRoutine());
    }

    private IEnumerator LaserDamageRoutine()
    {
        while (isRoutineRunning)
        {
            yield return new WaitForSeconds(0.75f);

            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    Health playerHealth = hit.collider.GetComponent<Health>();
                    if (playerHealth != null)
                    {
                        playerHealth.TakeDamage(10); // Adjust damage amount as needed
                    }
                }
            }
        }
    }

    private IEnumerator StopLaserDamageRoutine()
    {
        yield return new WaitForSeconds(3f); // Adjust duration as needed
        isRoutineRunning = false;
    }
}
