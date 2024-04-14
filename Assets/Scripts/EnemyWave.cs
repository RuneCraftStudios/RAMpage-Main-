using UnityEngine;

[System.Serializable]
public class EnemyWave
{
    public EnemyAiTutorial[] enemies; // Array of enemies in this wave
    public float nextWaveCooldown = 5f; // Cooldown time before the next wave starts
}