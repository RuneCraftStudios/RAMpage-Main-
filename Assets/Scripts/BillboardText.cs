using UnityEngine;

public class BillboardText : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        // Find the main camera in the scene
        mainCamera = Camera.main;

        // Check if TMP component exists
        if (GetComponent<TMPro.TextMeshPro>() == null)
        {
            Debug.LogError("BillboardText script requires a TextMeshPro component.");
            enabled = false;
        }
    }

    void LateUpdate()
    {
        // Ensure camera is not null
        if (mainCamera == null)
            return;

        // Make the text face towards the camera
        transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
            mainCamera.transform.rotation * Vector3.up);
    }
}
