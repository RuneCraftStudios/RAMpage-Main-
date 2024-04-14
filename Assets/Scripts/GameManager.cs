using UnityEngine;
using System.Collections.Generic; // Needed for using List

public class GameManager : MonoBehaviour
{
    public static GameManager instance; // Singleton instance

    [Header("ClearLevelSettings")]
    public AudioSource AudioSource;
    public AudioClip levelclearClip;
    public List<GameObject> LevelDoors; // Changed to a list of GameObjects

    [Header("GameOverSettings")]
    public GameObject gameOverUI; // UI element displayed when the player dies
    public AudioClip gameoverClip; // Sound clip played when the game is over

    private bool GameOver = false;
    private bool isLevelClear = false;

    private void Awake()
    {
        // Singleton implementation
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject); // Destroy duplicates
            return;
        }

        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false);
        }

        // Initialize all Level Doors as inactive
        if (LevelDoors != null)
        {
            foreach (GameObject door in LevelDoors)
            {
                if (door != null)
                {
                    door.SetActive(false);
                }
            }
        }

    }

    public void PlayerDied()
    {
        if (GameOver) return; // Prevent multiple calls

        GameOver = true;

        // Pause the game by stopping time
        Time.timeScale = 0f;

        // Play game over sound
        if (gameoverClip != null)
        {
            AudioSource.PlayOneShot(gameoverClip);
        }

        // Show game over UI
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }
    }

    public void LevelCleared()
    {
        if (isLevelClear) return;
        isLevelClear = true;

        // Activate all doors in the LevelDoors list
        foreach (GameObject door in LevelDoors)
        {
            if (door != null)
            {
                door.SetActive(true);
            }
        }

        AudioSource.PlayOneShot(levelclearClip);
    }
}
