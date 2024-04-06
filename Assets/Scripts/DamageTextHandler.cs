using UnityEngine;
using TMPro;
using System.Collections;

public class DamageTextHandler : MonoBehaviour
{
    public TextMeshProUGUI damageReceivedText; // Reference to the TextMeshProUGUI component for displaying damage received message
    private bool isClearingText = false; // Flag to track if text is currently being cleared

    public void DisplayDamageReceived(int damage)
    {
        // Display damage received message in UI
        if (damageReceivedText != null)
        {
            damageReceivedText.text = "Damage Received: " + damage;

            if (!isClearingText)
            {
                StartCoroutine(ClearDamageReceivedText()); // Start the coroutine to clear the message after some time
            }
        }
        else
        {
            Debug.LogError("Damage Received TextMeshProUGUI component not assigned.");
        }
    }

    private IEnumerator ClearDamageReceivedText()
    {
        // Set flag to track text clearing
        isClearingText = true;

        // Wait for specified duration before clearing the text
        yield return new WaitForSeconds(3f);

        // Clear the damage received message
        if (damageReceivedText != null)
        {
            damageReceivedText.text = "";
            isClearingText = false; // Reset flag
        }
    }
}
