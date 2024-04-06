using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    // Dictionary to store the player's resources (e.g., code fragments)
    private Dictionary<string, int> resources = new Dictionary<string, int>();

    // Start is called before the first frame update
    void Start()
    {
        // Initialize the player's resources
        resources.Add("Code Fragments", 0);
    }

    // Method to add code fragments to the player's inventory
    public void AddCodeFragments(int amount)
    {
        resources["Code Fragments"] += amount;
        Debug.Log("Added " + amount + " code fragments to inventory. Total: " + resources["Code Fragments"]);
    }

    // Method to deduct code fragments from the player's inventory
    public void DeductCodeFragments(int amount)
    {
        if (resources["Code Fragments"] >= amount)
        {
            resources["Code Fragments"] -= amount;
            Debug.Log("Deducted " + amount + " code fragments from inventory. Remaining: " + resources["Code Fragments"]);
        }
        else
        {
            Debug.LogWarning("Insufficient code fragments in inventory.");
        }
    }

    // Method to check the quantity of code fragments in the player's inventory
    public int GetCodeFragmentCount()
    {
        return resources["Code Fragments"];
    }

    // You can add more methods here for managing other types of resources or upgrades
}
