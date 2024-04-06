using System.Collections;
using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class PlayerUI : MonoBehaviour
{
    public TextMeshProUGUI itemText; // Reference to the TextMeshPro component in the player's UI

    private Queue<string> incomingInputs = new Queue<string>(); // Queue to store incoming inputs
    private bool isClearingText = false; // Flag to track if text is currently being cleared

    public void UpdateItemText(string itemName)
    {
        itemText.text = "Item Collected: " + itemName;

        if (!isClearingText)
        {
            StartCoroutine(ClearItemCollectedText()); // Start the coroutine to clear the message after some time
        }
        else
        {
            // If text is currently being cleared, cache the input for later display
            CacheInput("Item Collected: " + itemName);
        }
    }

    private IEnumerator ClearItemCollectedText()
    {
        // Set flag to track text clearing
        isClearingText = true;

        // Wait for specified duration before clearing the text
        yield return new WaitForSeconds(3f); // Adjust the duration as needed

        // Clear the item collected message
        itemText.text = "";
        isClearingText = false; // Reset flag

        // Clear all queued inputs
        incomingInputs.Clear();
    }


    public void CacheInput(string input)
    {
        // Add incoming input to the cache queue
        incomingInputs.Enqueue(input);
    }
}
