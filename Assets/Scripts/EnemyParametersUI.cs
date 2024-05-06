using UnityEngine;
using UnityEngine.UI;

public class EnemyParametersUI : MonoBehaviour
{
    public Image healthBar;
    public Image shieldBar;
    public Image energyBar;

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

    public void UpdateEnergyBar(float currentEnergy, float maxEnergy)
    {
        if (energyBar != null)
        {
            float energyPercentage = currentEnergy / maxEnergy;
            energyBar.fillAmount = energyPercentage;
        }
    }
}
