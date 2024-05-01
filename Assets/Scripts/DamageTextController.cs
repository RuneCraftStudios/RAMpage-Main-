using UnityEngine;
using TMPro;
using System.Collections;

public class DamageTextController : MonoBehaviour
{
    public TextMeshPro baseDamageText;  // Assign in the inspector for base damage
    public TextMeshPro fireDamageText;  // Assign in the inspector for fire damage
    public TextMeshPro electricityDamageText;  // Assign in the inspector for electricity damage

    private void Start()
    {
        ClearAllTexts(); // Ensure all texts are empty on start
    }

    public void DisplayDamage(int damage)
    {
        StopAllCoroutines(); // Stop any previous animations or clears
        TextMeshPro selectedText = baseDamageText;
        if (selectedText != null)
        {
            selectedText.text = damage.ToString();
            StartCoroutine(ClearTextAfterDelay(selectedText));
        }
    }

    private IEnumerator ClearTextAfterDelay(TextMeshPro textComponent)
    {
        yield return new WaitForSeconds(2f); // Wait for 2 seconds
        textComponent.text = ""; // Then clear the text
    }

    private void ClearAllTexts()
    {
        baseDamageText.text = "";
        fireDamageText.text = "";
        electricityDamageText.text = "";
    }
}
