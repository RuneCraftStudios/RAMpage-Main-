using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance; // Singleton instance

    public GameObject gameOverUI; // UI element displayed when the player dies
    public GameObject gameOverSound; // Sound clip played when the game is over
    private bool GameOver = false;

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
        if (gameOverSound != null)
        {
            gameOverSound.SetActive(false);
        }
    }

    public void PlayerDied()
    {
        if (GameOver) return; // Prevent multiple calls

        GameOver = true;

        // Pause the game by stopping time
        Time.timeScale = 0f;

        // Play game over sound
        if (gameOverSound != null)
        {
            gameOverSound.SetActive(true);
        }

        // Show game over UI
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }
    }
}
