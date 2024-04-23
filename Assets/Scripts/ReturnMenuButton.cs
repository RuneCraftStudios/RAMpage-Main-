using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ReturnMenuButton : MonoBehaviour
{
    public InputActionProperty menuButtonAction;
    public float holdDuration = 2f;
    public Image radialFillImage;
    private float timer = 0f;
    private bool buttonHeld = false;

    private void OnEnable()
    {
        menuButtonAction.action.Enable();
    }

    private void OnDisable()
    {
        menuButtonAction.action.Disable();
    }

    void Update()
    {
        bool isPressed = menuButtonAction.action.ReadValue<float>() > 0.1f;  // Assumes button press is a float

        if (isPressed)
        {
            if (!buttonHeld)
            {
                buttonHeld = true;  // Start holding the button
            }
            timer += Time.deltaTime;
            radialFillImage.fillAmount = timer / holdDuration;
        }
        else if (buttonHeld)
        {
            ResetButton();
        }

        if (buttonHeld && timer >= holdDuration)
        {
            LoadMainMenu();
        }
    }

    void ResetButton()
    {
        buttonHeld = false;
        timer = 0f;
        radialFillImage.fillAmount = 0f;
    }

    void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
