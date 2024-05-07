using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class Health : MonoBehaviour
{
    [Header("Main Parameters")]
    public int maxHealth = 100; // Maximum health of the GameObject
    public int maxShield = 100; // Maximum shield of the GameObject
    public int maxEnergy = 100; // Maximum energy of the player
    public float rechargeDelay = 2f; // Time delay before shield starts recharging after taking damage
    public float shieldRechargeRate = 5f; // Shield recharge rate per second

    [Header("Status")]
    [SerializeField] private int currentHealth; // Current health of the GameObject
    [SerializeField] private int currentShield; // Current shield of the GameObject
    [SerializeField] private int currentEnergy; // Current energy of the GameObject
    public bool ConsumeEnergy; // Bool to test energy depletion
    private float lastDamageTime; // Time when the last damage was taken
    private float currentRechargeAmount; // Accumulated recharge amount for the shield

    private Coroutine damageOverTimeCoroutine = null;
    private GameManager GameManager;
    private Queue<int> damageQueue = new Queue<int>();
    public EnemyParametersUI enemyParametersUI;


    private void Awake()
    {
        currentHealth = maxHealth; // Initialize current health to max health
        currentShield = maxShield; // Initialize current shield to max shield
        currentEnergy = maxEnergy; // Initialize current energy to max energy
        lastDamageTime = Time.time; // Initialize lastDamageTime to current time
        UpdateUI();
    }
    public int CurrentHealth
    {
        get { return currentHealth; }
    }

    public int CurrentShield
    {
        get { return currentShield; }
    }

    public int CurrentEnergy
    {
        get { return currentEnergy; }
    }

    private void Update()
    {
        // Modify the shield recharge condition to check for !isDamageOverTimeActive and healthUnchanged
        if (currentShield < maxShield && Time.time > lastDamageTime + rechargeDelay && damageQueue.Count == 0)
        {
            // Accumulate recharge amount
            currentRechargeAmount += shieldRechargeRate * Time.deltaTime;

            // Check if enough recharge amount has accumulated to add a point to the shield
            while (currentRechargeAmount >= 1f)
            {
                currentShield = Mathf.Min(currentShield + 1, maxShield);
                currentRechargeAmount -= 1f;
                UpdateUI();
            }
        }

        // Process damage queue
        if (damageQueue.Count > 0)
        {
            int damage = damageQueue.Dequeue();
            TakeDamage(damage);
        }

        // Test energy depletion
        if (ConsumeEnergy)
        {
            DepleteEnergy(10); // Deplete energy by 10 units
            ConsumeEnergy = false; // Reset testDepleteEnergy
            UpdateUI();
        }
    }
    public void QueueDamage(int damage)
    {
        // Add incoming damage to the queue
        damageQueue.Enqueue(damage);
    }

    private void UpdateUI()
    {
        enemyParametersUI.UpdateHealthBar(currentHealth, maxHealth);
        enemyParametersUI.UpdateShieldBar(currentShield, maxShield);
        enemyParametersUI.UpdateEnergyBar(currentEnergy, maxEnergy);
    }
    public void TakeDamage(int damage)
    {
        if (currentShield > 0)
        {
            // Apply damage to shield
            currentShield -= damage;
            if (currentShield < 0)
            {
                currentShield = 0;
            }
        }
        else
        {
            // Apply damage to health directly
            currentHealth -= damage;
            if (currentHealth <= 0)
            {
                Die();
            }
        }

        // Update lastDamageTime to current time
        lastDamageTime = Time.time;
        UpdateUI();
        // Apply elemental effects

    }
    public void StartDamageOverTime(int damagePerSecond, float duration)
    {
        // If there's an ongoing DoT effect, stop it first to reset
        damageOverTimeCoroutine = StartCoroutine(DamageOverTimeCoroutine(damagePerSecond, duration));
    }
    private IEnumerator DamageOverTimeCoroutine(int damageOverTime, float effectDuration)
    {
        float timer = 0f;

        while (timer < effectDuration)
        {
            // Apply damage over time
            TakeDamage(damageOverTime); // Using TakeDamage ensures shield/health logic is centralized
            timer += 1f; // Assuming damage is applied every second
            yield return new WaitForSeconds(1f);
        }

        // Reset coroutine reference when done
        damageOverTimeCoroutine = null;
    }
    public void StopDamageOverTime()
    {
        if (damageOverTimeCoroutine != null)
        {
            StopCoroutine(damageOverTimeCoroutine);
            damageOverTimeCoroutine = null;
        }
    }

    public void RestoreHealth(int amount)
    {
        // Increase current health by the specified amount
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        UpdateUI();
    }

    public void RestoreShield(int amount)
    {
        // Increase current shield by the specified amount
        currentShield = Mathf.Min(currentShield + amount, maxShield);
        UpdateUI();
    }

    public void RestoreEnergy(float amount)
    {
        // Increase current energy by the specified amount
        currentEnergy = Mathf.Min(currentEnergy + (int)amount, maxEnergy);
        UpdateUI();
    }

    void Die()
    {
       GameManager.instance.PlayerDied();
        StopDamageOverTime();
    }
    // Accessors for current health, shield, and energy

    // Method to deplete energy (for testing purposes)
    public void DepleteEnergy(int amount)
    {
        // Reduce current energy by the specified amount
        currentEnergy = Mathf.Max(currentEnergy - amount, 0);
    }
}