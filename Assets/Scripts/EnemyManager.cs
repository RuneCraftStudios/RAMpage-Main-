using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public EnemyAiTutorial[] groundEnemies; // Array to store references to all ground enemy AI scripts
    public float groundEnemySpawnInterval = 3f; // Time interval between activating each ground enemy

    private int groundEnemiesActivated = 0; // Counter to keep track of activated ground enemies
    private float groundEnemyTimer = 0f;

    private void Update()
    {
        // Increment the timer
        groundEnemyTimer += Time.deltaTime;

        // Check if it's time to activate the next ground enemy
        if (groundEnemyTimer >= groundEnemySpawnInterval)
        {
            groundEnemyTimer = 0f;

            if (groundEnemiesActivated < groundEnemies.Length)
            {
                ActivateNextGroundEnemy();
            }
            else
            {
                // All ground enemies have been activated, stop the manager
                enabled = false;
            }
        }
    }

    private void ActivateNextGroundEnemy()
    {
        groundEnemies[groundEnemiesActivated].ChangeState(EnemyState.Patrol);
        groundEnemiesActivated++;
    }
}
