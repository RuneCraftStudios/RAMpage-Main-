using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    public List<Wave> waves;
    public float timeBetweenWaves = 5.0f; // Time between waves
    [SerializeField] private GameManager gameManager;

    private int currentWaveIndex = 0;
    private GameObject[] currentWave;
    private bool spawningWave = false;

    private void Awake()
    {
        StartWave();
    }

    private void StartWave()
    {
        if (currentWaveIndex < waves.Count)
        {
            currentWave = waves[currentWaveIndex].enemies;
            StartCoroutine(SpawnWave());
        }
        else
        {
            gameManager.LevelCleared();
        }
    }

    private IEnumerator SpawnWave()
    {
        spawningWave = true;

        foreach (var enemy in currentWave)
        {
            if (enemy != null)
            {
                enemy.SetActive(true);
                EnemyAiTutorial ai = enemy.GetComponent<EnemyAiTutorial>();
                if (ai != null)
                {
                    ai.ChangeState(EnemyState.WaitingToBeSpawned);
                }
            }
        }

        yield return new WaitUntil(() => AllEnemiesDeactivated());

        // Delay before starting the next wave (you can add animation and music here)
        yield return new WaitForSeconds(timeBetweenWaves);

        currentWaveIndex++;
        StartWave();

        spawningWave = false;
    }

    private bool AllEnemiesDeactivated()
    {
        foreach (var enemy in currentWave)
        {
            if (enemy != null && enemy.activeSelf)
            {
                return false;
            }
        }
        return true;
    }

    private void Update()
    {
        // Check if all enemies of the current wave are deactivated
        if (!spawningWave && AllEnemiesDeactivated())
        {
            gameManager.ManagerAudioSource.PlayOneShot(gameManager.WaveClear);
            // You can add any post-wave logic here
            Debug.Log("Wave " + (currentWaveIndex + 1) + " cleared!");
        }
    }
}
