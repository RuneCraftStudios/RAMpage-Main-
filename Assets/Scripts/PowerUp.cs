using UnityEngine;

public class PowerUp : MonoBehaviour
{
    // Properties for different power-up effects
    public string itemName; // Name of the item
    public int healthRestoreAmount = 0; // Amount to restore player's health
    public int shieldRestoreAmount = 0; // Amount to restore player's shield
    public int energyRestoreAmount = 0; // Amount to restore player's energy

    // Method to activate the power-up
    public void Activate(GameObject target)
    {
        // Apply power-up effects based on the properties
        Health playerHealth = target.GetComponent<Health>();
        if (playerHealth != null)
        {
            if (healthRestoreAmount > 0)
            {
                playerHealth.RestoreHealth(healthRestoreAmount);
                Debug.Log("Health Restored: " + healthRestoreAmount);
            }

            if (shieldRestoreAmount > 0)
            {
                playerHealth.RestoreShield(shieldRestoreAmount);
                Debug.Log("Shield Restored: " + shieldRestoreAmount);
            }

            if (energyRestoreAmount > 0)
            {
                playerHealth.RestoreEnergy(energyRestoreAmount);
                Debug.Log("Energy Restored: " + energyRestoreAmount);
            }
        }

        // Destroy the power-up object
        Destroy(gameObject);
    }
}
