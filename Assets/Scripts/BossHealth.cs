using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.XR;

public class BossHealth : MonoBehaviour
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
    private Coroutine damageOverTimeCoroutine = null;
    [SerializeField] private float DeathAnimationTime;

    private Coroutine electrifiedCoroutine;
    private Coroutine ignitedCoroutine;
    private Queue<int> damageQueue = new Queue<int>();

    private EnemySoundManager enemySoundManager;
    public Animator animator;
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

    // Inside your Awake() method
    private void Awake()
    {
        currentHealth = maxHealth;
        currentShield = maxShield;
        currentEnergy = maxEnergy;
        lastDamageTime = Time.time;
        EnemySoundManager enemySoundManager = GetComponent<EnemySoundManager>();
        animator.SetBool("IsActive", true);
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
            }
        }

        // Process damage queue
        if (damageQueue.Count > 0)
        {
            int damage = damageQueue.Dequeue();
            TakeDamage(damage);
        }

        if (currentHealth < maxHealth * 0.5f)
        {
            animator.SetBool("isEnraged", true);
        }
    }
    public void QueueDamage(int damage)
    {
        // Add incoming damage to the queue
        damageQueue.Enqueue(damage);
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
                animator.SetBool("isDead", true);
            }
        }

        // Update lastDamageTime to current time
        lastDamageTime = Time.time;

        // Apply elemental effects

        enemySoundManager.PlayEnemyTakeDamageSound();
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
}