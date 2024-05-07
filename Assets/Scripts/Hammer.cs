using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Hammer : MonoBehaviour
{
    public GameObject SkillCollider;
    public GameObject ImpactEffect;
    public Collider WeaponCollider;
    void Start()
    {
        XRGrabInteractable grabbable = GetComponent<XRGrabInteractable>();
        grabbable.activated.AddListener(SkillActivation);
    }
    void SkillActivation(ActivateEventArgs Arg)
    {
        SkillCollider.SetActive(true);
    }

    void SkillDeactivate()
    {
        SkillCollider.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            Debug.Log("GroundTagIdentified");
            Instantiate(ImpactEffect, other.transform, other.transform);
            SkillDeactivate();
        }
    }
}
