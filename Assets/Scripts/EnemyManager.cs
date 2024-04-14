using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public EnemyWave[] waves; // Array of waves

    private int currentWaveIndex = 0;
    private float cooldownTimer = 0f;
    private bool isCooldown = false;

    private void Start()
    {
        StartWave(currentWaveIndex);
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
                if (currentWaveIndex < waves.Length)
                {
                    StartWave(currentWaveIndex);
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
            enemy.ChangeState(EnemyState.Patrol); // Assumes this will enable the necessary components
        }
    }

    private void CheckWaveCompletion()
    {
        bool allDeactivated = true;
        foreach (var enemy in waves[currentWaveIndex].enemies)
        {
            if (enemy.gameObject.activeSelf) // Check if the GameObject is active in the hierarchy
            {
                allDeactivated = false;
                break;
            }
        }

        if (allDeactivated)
        {
            isCooldown = true;
        }
    }
}