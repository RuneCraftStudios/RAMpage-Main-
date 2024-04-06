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
    [SerializeField] private bool isElectrified = false; // Indicates if the target is electrified
    [SerializeField] private bool isIgnited = false; // Indicates if the target is ignited
 
    private GameManager GameManager;
    private Queue<int> damageQueue = new Queue<int>();
    // Inside your Awake() method
    private void Awake()
    {
        currentHealth = maxHealth; // Initialize current health to max health
        currentShield = maxShield; // Initialize current shield to max shield
        currentEnergy = maxEnergy; // Initialize current energy to max energy
        lastDamageTime = Time.time; // Initialize lastDamageTime to current time
        isElectrified = false;
        isIgnited = false;
    }

    public bool IsElectrified
    {
        get { return isElectrified; }
        set { isElectrified = value; }
    }

    public bool IsIgnited
    {
        get { return isIgnited; }
        set { isIgnited = value; }
    }

    private void Update()
    {
        
        // Check if the shield needs to be recharged
        if (!isElectrified && !isIgnited && currentShield < maxShield && Time.time > lastDamageTime + rechargeDelay && damageQueue.Count == 0)
        {
            // Accumulate recharge amount
            currentRechargeAmount += shieldRechargeRate * Time.deltaTime;

            // Check if enough recharge amount has accumulated to add a point to the shield
            while (currentRechargeAmount >= 1f)
            {
                currentShield = Mathf.Min(currentShield + 1, maxShield);
                currentRechargeAmount -= 1f;
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
        }
    }


    public bool IsElementalEffectActive()
    {
        // Check if the enemy is electrified
        bool isElectrified = GetComponent<ElementalEffect>() != null && GetComponent<ElementalEffect>().elementType == ElementalEffect.ElementType.Electricity;

        // Check if the enemy is ignited
        bool isIgnited = GetComponent<ElementalEffect>() != null && GetComponent<ElementalEffect>().elementType == ElementalEffect.ElementType.Fire;

        // Return true if either the enemy is electrified or ignited
        return isElectrified || isIgnited;
    }



    public void QueueDamage(int damage)
    {
        // Add incoming damage to the queue
        damageQueue.Enqueue(damage);
    }

    public void TakeDamage(int damage, bool applyToShield = false, bool isElementalDamage = false, float effectDuration = 0f, int damageOverTime = 0, float effectChance = 0f)
    {
        if (applyToShield)
        {
            if (!isElementalDamage)
            {
                // Normal damage to shields
                currentShield -= damage;
                if (currentShield < 0)
                {
                    currentShield = 0;
                }
            }
        }
        else
        {
            // Apply normal damage to health
            currentHealth -= damage;
            if (currentHealth <= 0)
                {
                    Die();
                }
          
        }

        // Update lastDamageTime to current time
        lastDamageTime = Time.time;
    }


    public IEnumerator DamageOverTimeCoroutine(int damageOverTime, int effectDuration, bool isElementalDamage = false)
    {

        float timer = 0f;

        while (timer < effectDuration)
        {
            // Check if shields are present
            if (currentShield > 0)
            {
                // Apply damage over time to shields
                currentShield -= damageOverTime;

                if (currentShield < 0)
                {
                    currentShield = 0;
                }
            }
            else
            {
                // If no shields, apply damage over time to health
                currentHealth -= damageOverTime;
                if (currentHealth <= 0)
                {
                    
                    Die();
                    
                    yield break; // Exit the coroutine if health reaches zero
                }
            }
            timer += 1f; // Increment timer by 1 second
            yield return new WaitForSeconds(1f);
        }
    }
    

    public IEnumerator DisableElectrifiedEffect(int duration)
    {
        yield return new WaitForSeconds(duration);
        isElectrified = false; // Disable electrified effect after duration ends
    }
    public IEnumerator DisableIgnitedEffect(int duration)
    {
        yield return new WaitForSeconds(duration);
        isIgnited = false; // Disable ignited effect after duration ends
    }


    public void RestoreHealth(int amount)
    {
        // Increase current health by the specified amount
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
    }

    public void RestoreShield(int amount)
    {
        // Increase current shield by the specified amount
        currentShield = Mathf.Min(currentShield + amount, maxShield);
    }

    public void RestoreEnergy(float amount)
    {
        // Increase current energy by the specified amount
        currentEnergy = Mathf.Min(currentEnergy + (int)amount, maxEnergy);
    }

    void Die()
    {
            GameManager.instance.PlayerDied();
    }
    // Accessors for current health, shield, and energy
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

    // Method to deplete energy (for testing purposes)
    public void DepleteEnergy(int amount)
    {
        // Reduce current energy by the specified amount
        currentEnergy = Mathf.Max(currentEnergy - amount, 0);
    }
}