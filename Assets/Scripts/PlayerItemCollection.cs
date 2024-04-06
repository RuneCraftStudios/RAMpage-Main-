using TMPro;
using UnityEngine;

public class PlayerItemCollection : MonoBehaviour
{
    public TextMeshProUGUI itemCollectedText; // Reference to the TextMeshPro component for displaying item collected message
    public LayerMask itemLayer; // Layer mask for detecting items
    public float itemSpeed = 10f; // Speed of items being pulled towards the Collection collider
    public SphereCollider detectionCollider; // Reference to the detection collider
    private PlayerInventory playerInventory; // Reference to the PlayerInventory script
    private void Start()
    {
        playerInventory = GetComponent<PlayerInventory>(); // Get reference to PlayerInventory script
    }

    private void Update()
    {
        // Check for items within detection range
        Collider[] detectedItems = Physics.OverlapSphere(detectionCollider.transform.position, detectionCollider.radius, itemLayer);
        foreach (Collider itemCollider in detectedItems)
        {
            // Check if the collider belongs to a code fragment
            CodeFragment codeFragment = itemCollider.GetComponent<CodeFragment>();
            if (codeFragment != null)
            {
                // Collect the code fragment and add it to the player's inventory
                if (playerInventory != null)
                {
                    playerInventory.AddCodeFragments(codeFragment.value);
                }

                // Destroy the collected code fragment
                Destroy(itemCollider.gameObject);
            }

            // Check if the collider belongs to a power-up
            PowerUp powerUp = itemCollider.GetComponent<PowerUp>();
            if (powerUp != null)
            {
                // Activate the power-up
                powerUp.Activate(gameObject);
            }
        }
    }

    // Rest of the script remains unchanged
}
