using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Hammer : MonoBehaviour
{
    private Rigidbody weaponRigidbody;
    public Transform forceApplyLocation; // Assign this in the Unity Inspector
    public float forceMagnitude = 10.0f; // You can adjust the magnitude in the Inspector
    public Vector3 forceDirection = Vector3.forward; // Default direction of the force

    // Start is called before the first frame update
    void Start()
    {
        XRGrabInteractable grabbable = GetComponent<XRGrabInteractable>();
        grabbable.activated.AddListener(Rocket);
        weaponRigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Rocket(ActivateEventArgs Arg)
    {
        // Apply a force at the specified location
        if (weaponRigidbody && forceApplyLocation)
        {
            Vector3 worldForceDirection = forceApplyLocation.TransformDirection(forceDirection.normalized);
            weaponRigidbody.AddForceAtPosition(worldForceDirection * forceMagnitude, forceApplyLocation.position);
        }
    }
}
