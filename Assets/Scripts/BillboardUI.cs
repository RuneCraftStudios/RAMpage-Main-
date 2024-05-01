using UnityEngine;

public class BillboardUI : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        // Find the main camera in the scene
        mainCamera = Camera.main;

        // Check if UI elements are present
        if (GetComponent<RectTransform>() == null)
        {
            Debug.LogError("BillboardUI script requires UI elements with RectTransform component.");
            enabled = false;
        }
    }

    void LateUpdate()
    {
        // Ensure camera and UI elements are not null
        if (mainCamera == null || GetComponent<RectTransform>() == null)
            return;

        // Make the UI elements face towards the camera
        transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
            mainCamera.transform.rotation * Vector3.up);
    }
}
