using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class ChargeStation : MonoBehaviour
{
    [Header("Parameters")]
    public int maxCapacity = 100;
    public float regenRate = 10.0f;
    public Transform spawnLocation;
    public GameObject itemPrefab;
    public int spawnNumber = 1;
    public float spawnFrequency = 5.0f;
    public Animator animator;

    [Header("Regeneration")]
    public float regenInterval = 5.0f;

    [Header("Status")]
    [SerializeField]
    private int currentCapacity = 0;
    [SerializeField]
    private bool playerInside = false;
    [SerializeField]
    private bool playerOutside = true; // Initially, player is outside
    [SerializeField]
    private bool regenProcessActive = false;
    [SerializeField]
    private bool capacityFull = false;
    [SerializeField]
    private bool capacityEmpty = false;

    private bool continueRegeneration = true; // Flag to control the regeneration loop

    private void Update()
    {
        // Check if the regeneration process needs to start or stop
        if (!playerInside && playerOutside && currentCapacity < maxCapacity && !regenProcessActive)
        {
            // Start the regeneration process if player is outside, capacity is not full, and process is not active
            StartRegenProcess();
        }
        else if (playerInside && regenProcessActive)
        {
            // Stop the regeneration process if the player is inside
            StopRegenProcess();
        }

        // Update capacity full and empty booleans
        capacityFull = currentCapacity >= maxCapacity;
        capacityEmpty = currentCapacity == 0;

        // Activate animator bools based on capacity status
        animator.SetBool("CapacityFull", capacityFull);
        animator.SetBool("CapacityEmpty", capacityEmpty);
    }



    private void StopRegenProcess()
    {
        if (regenProcessActive)
        {
            regenProcessActive = false;
            animator.SetBool("RegenProcessActive", false);
            continueRegeneration = false; // Reset the flag to ensure the coroutine stops properly
            StopCoroutine(RegenerateCapacity());
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            animator.SetBool("PlayerInside", true);
            playerInside = true;
            playerOutside = false;
            StartSpawning(); // Start spawning items when player enters the collider
            //Debug.Log("PlayerInside bool set to true.");

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            playerInside = false;
            playerOutside = true;
            StopSpawning(); // Stop spawning items when player exits the collider
            animator.SetBool("PlayerInside", false);
        }
    }

    private void StartSpawning()
    {
        InvokeRepeating("SpawnItems", 0, spawnFrequency);
    }

    private void StopSpawning()
    {
        CancelInvoke("SpawnItems");
    }

    private void StartRegenProcess()
    {
        if (!regenProcessActive)
        {
            regenProcessActive = true;
            animator.SetBool("RegenProcessActive", true);
            continueRegeneration = true; // Reset the flag to ensure proper regeneration
            StartCoroutine(RegenerateCapacity()); // Start or restart the regeneration coroutine

        }
    }

    private IEnumerator RegenerateCapacity()
    {
        // Wait for the initial buffer time before starting the regeneration process
        yield return new WaitForSeconds(regenInterval);

        while (currentCapacity < maxCapacity && continueRegeneration) // Check continueRegeneration flag
        {
            //Debug.Log("Start of regeneration process. Current capacity: " + currentCapacity);

            // Update currentCapacity by the regenRate
            currentCapacity += (int)regenRate;

            // Ensure currentCapacity does not exceed maxCapacity
            currentCapacity = Mathf.Min(currentCapacity, maxCapacity);

            // If currentCapacity reaches maxCapacity, stop the regeneration process
            if (currentCapacity >= maxCapacity)
            {
                currentCapacity = maxCapacity;
                // Station is fully charged
                //Debug.Log("Station is fully charged. Current capacity: " + currentCapacity);
                animator.SetBool("RegenProcessActive", false);
                regenProcessActive = false;
                break; // Exit the loop
            }
            else
            {
                // Wait for the regenInterval before the next iteration
                yield return new WaitForSeconds(regenInterval);
            }
        }
    }

    private void SpawnItems()
    {
        // Spawn 'spawnNumber' of items if currentCapacity is greater than 0
        for (int i = 0; i < spawnNumber && currentCapacity > 0; i++)
        {
            Instantiate(itemPrefab, spawnLocation.position, spawnLocation.rotation);
            currentCapacity--;
        }
    }
}
