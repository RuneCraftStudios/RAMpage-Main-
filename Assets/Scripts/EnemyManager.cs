using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public EnemyWave[] waves; // Array of waves

    private int currentWaveIndex = 0;
    private float cooldownTimer = 0f;
    private bool isCooldown = false;
    private GameManager gameManager;

    private void Start()
    {
        if (waves.Length > 0)
        {
            StartWave(currentWaveIndex);  // Start the first wave only if there are any waves
        }
    }

    private void Update()
    {
        if (isCooldown)
        {
            cooldownTimer += Time.deltaTime;
            if (cooldownTimer >= waves[currentWaveIndex].nextWaveCooldown)
            {
                cooldownTimer = 0f;
                isCooldown = false;
                currentWaveIndex++;
                if (currentWaveIndex < waves.Length) // Check if the new index is within bounds
                {
                    StartWave(currentWaveIndex);
                }
                else
                {
                    Debug.Log("All waves completed!");
                }
            }
        }
        else
        {
            CheckWaveCompletion();
        }
    }

    private void StartWave(int index)
    {
        foreach (var enemy in waves[index].enemies)
        {
            enemy.ChangeState(EnemyState.Patrol); // Assume this enables necessary components
        }
    }

    private void CheckWaveCompletion()
    {
        bool allDead = true;
        foreach (var enemy in waves[currentWaveIndex].enemies)
        {
            if (enemy != null && enemy.gameObject != null)
            {
                EnemyHealth health = enemy.GetComponent<EnemyHealth>();
                if (health != null && health.CurrentHealth > 0)
                {
                    allDead = false;
                    break;
                }
            }
        }

        if (allDead)
        {
            isCooldown = true;
        }
    }
}
