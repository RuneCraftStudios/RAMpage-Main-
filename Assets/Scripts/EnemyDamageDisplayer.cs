using System.Collections;
using UnityEngine;
using TMPro;

public class DamageTextManager : MonoBehaviour
{
    public static DamageTextManager Instance { get; private set; } // Singleton instance

    public TextMeshProUGUI baseDamageText; // Assign in the inspector
    public TextMeshProUGUI fireDamageText; // Assign in the inspector
    public TextMeshProUGUI electricityDamageText; // Assign in the inspector

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator ShowAndClearDamage(TextMeshProUGUI textComponent, int damage)
    {
        textComponent.text = damage.ToString();
        yield return new WaitForSeconds(2f); // Display the damage for 2 seconds
        textComponent.text = ""; // Clear the text
    }

    public void DisplayDamage(int damage, string damageType)
    {
        TextMeshProUGUI selectedTextComponent = baseDamageText; // Default to base damage text

        // The rest of your method remains unchanged...
    }
}
