using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class PlayerToggleRun : MonoBehaviour
{
    // Reference to the DynamicMoveProvider component
    [SerializeField] private DynamicMoveProvider moveProvider;

    // Input action to toggle running
    [SerializeField] private InputActionReference toggleRunAction;

    // Input action for forward movement
    [SerializeField] private InputActionReference forwardMovementAction;

    // Original move speed value
    [SerializeField] private float originalMoveSpeed;

    // Move speed value to apply when running
    [SerializeField] private float runningMoveSpeed = 10f;

    // Flag to track whether the player is currently running
    private bool isRunning = false;

    // Flag to track whether the player was moving forward previously
    private bool wasMovingForward = false;

    private void Start()
    {
        // Get the DynamicMoveProvider component attached to the player if not set
        if (moveProvider == null)
        {
            moveProvider = GetComponent<DynamicMoveProvider>();
        }

        // Store the original move speed value
        originalMoveSpeed = moveProvider.moveSpeed;

        // Subscribe to the toggle run action if it's assigned
        if (toggleRunAction != null)
        {
            toggleRunAction.action.started += ToggleRunning;
            toggleRunAction.action.Enable();
        }
        else
        {
            Debug.LogWarning("Toggle run action is not assigned in the inspector.");
        }

        // Subscribe to forward movement action if it's assigned
        if (forwardMovementAction != null)
        {
            forwardMovementAction.action.performed += OnForwardMovement;
            forwardMovementAction.action.canceled += OnForwardMovementCanceled;
            forwardMovementAction.action.Enable();
        }
        else
        {
            Debug.LogWarning("Forward movement action is not assigned in the inspector.");
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from the toggle run action if it's assigned
        if (toggleRunAction != null)
        {
            toggleRunAction.action.started -= ToggleRunning;
            toggleRunAction.action.Disable();
        }

        // Unsubscribe from forward movement action if it's assigned
        if (forwardMovementAction != null)
        {
            forwardMovementAction.action.performed -= OnForwardMovement;
            forwardMovementAction.action.canceled -= OnForwardMovementCanceled;
            forwardMovementAction.action.Disable();
        }
    }

    // Callback method to toggle running
    private void ToggleRunning(InputAction.CallbackContext context)
    {
        isRunning = !isRunning;
        moveProvider.moveSpeed = isRunning ? runningMoveSpeed : originalMoveSpeed;
    }

    // Callback method to handle forward movement input
    private void OnForwardMovement(InputAction.CallbackContext context)
    {
        // Enable running if the toggle run action is pressed and the player is moving forward
        if (toggleRunAction.action.ReadValue<float>() > 0f && !wasMovingForward)
        {
            isRunning = true;
            moveProvider.moveSpeed = runningMoveSpeed;
        }
        wasMovingForward = true;
    }

    // Callback method to handle canceled forward movement input
    private void OnForwardMovementCanceled(InputAction.CallbackContext context)
    {
        // Disable running if the player stops moving forward
        if (wasMovingForward)
        {
            wasMovingForward = false;
            isRunning = false;
            moveProvider.moveSpeed = originalMoveSpeed;
        }
    }
}
