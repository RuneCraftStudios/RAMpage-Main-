using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class EnemyHealth : MonoBehaviour
{
    [Header("Main Parameters")]
    public int maxHealth = 100;
    public int maxShield = 100;
    public int maxEnergy = 100;
    public float rechargeDelay = 2f;
    public float shieldRechargeRate = 5f;

    [Header("Status")]
    [SerializeField] private int currentHealth;
    [SerializeField] private int currentShield;
    [SerializeField] private int currentEnergy;
    private float lastDamageTime;
    private float currentRechargeAmount;
    [SerializeField] private bool isElectrified = false;
    [SerializeField] private bool isIgnited = false;

    [Header("EnemyUISettings")]
    public TextMeshPro baseDamageText; // Assign the TextMeshPro component for base damage
    public TextMeshPro electricityDamageText; // Assign the TextMeshPro component for electricity damage
    public TextMeshPro fireDamageText; // Assign the TextMeshPro component for fire damage
    [SerializeField] private float textDisappearTime = 2f;
    [SerializeField] private float DeathAnimationTime;

    // Queue to store incoming damage messages
    private Queue<int> damageQueue = new Queue<int>();

    public EnemyAiTutorial enemyAi;

    // Inside your Awake() method
    private void Awake()
    {
        currentHealth = maxHealth;
        currentShield = maxShield;
        currentEnergy = maxEnergy;
        lastDamageTime = Time.time;
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
        if (applyToShield && currentShield > 0)
        {
            if (!isElementalDamage)
            {
                currentShield -= damage;
                if (currentShield < 0)
                {
                    currentShield = 0;
                }
            }
        }
        else
        {
            currentHealth -= damage;
            if (currentHealth <= 0)
            {
                enemyAi.ChangeState(EnemyState.Die);
            }
        }

        lastDamageTime = Time.time;
        SpawnDamageNumber(damage, transform, !isElementalDamage);
    }




    public IEnumerator DamageOverTimeCoroutine(int damageOverTime, int effectDuration, bool isElementalDamage = false)
    {
        enemyAi.ChangeState(EnemyState.Stun);

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
                    
                        enemyAi.ChangeState(EnemyState.Die);
                    yield break; // Exit the coroutine if health reaches zero
                }
            }


            SpawnDamageNumber(damageOverTime, transform, isElementalDamage);
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
    void SpawnDamageNumber(int damage, Transform targetTransform, bool isElemental = false)
    {
        TextMeshPro damageTextPrefab;

        // Select the appropriate damage number prefab based on elemental type
        if (isElemental)
        {
            if (IsElectrified)
            {
                damageTextPrefab = electricityDamageText;
            }
            else if (IsIgnited)
            {
                damageTextPrefab = fireDamageText;
            }
            else
            {
                damageTextPrefab = baseDamageText;
            }
        }
        else
        {
            damageTextPrefab = baseDamageText;
        }

        if (damageTextPrefab != null)
        {
            // Instantiate the damage number prefab at the position of the target TextMeshPro component
            TextMeshPro damageText = Instantiate(damageTextPrefab, damageTextPrefab.transform.position, Quaternion.identity);

            // Set the parent of the instantiated damage text to maintain hierarchy
            damageText.transform.SetParent(damageTextPrefab.transform.parent);

            // Set the damage number text to display the damage value
            damageText.text = damage.ToString();

            // Remove the damage number text after a certain time
            Destroy(damageText.gameObject, textDisappearTime);
        }
        else
        {
            Debug.LogError("Damage number prefab is not assigned.");
        }
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
}