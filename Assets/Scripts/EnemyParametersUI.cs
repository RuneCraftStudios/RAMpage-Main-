using UnityEngine;
using UnityEngine.UI;

public class EnemyParametersUI : MonoBehaviour
{
    public Image healthBar;
    public Image shieldBar;

    // Method to update health bar fill amount
    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        if (healthBar != null)
        {
            float healthPercentage = currentHealth / maxHealth;
            healthBar.fillAmount = healthPercentage;
        }
    }

    // Method to update shield bar fill amount
    public void UpdateShieldBar(float currentShield, float maxShield)
    {
        if (shieldBar != null)
        {
            float shieldPercentage = currentShield / maxShield;
            shieldBar.fillAmount = shieldPercentage;
        }
    }
}
