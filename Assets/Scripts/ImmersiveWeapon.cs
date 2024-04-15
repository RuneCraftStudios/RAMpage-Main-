using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class ImmersiveWeapon : MonoBehaviour
{
    [Header("Select Bullet")]
    public GameObject Bullet;
    public Transform MuzzleTransform;

    [Header("Select Sound Source")]
    public AudioSource AudioSource;

    [Header("AudioClip Inputs")]
    public AudioClip ChargingUpClip;
    public AudioClip ShootClip;
    public AudioClip DisengageClip;
    public AudioClip NoEnergyClip;

    [Header("Weapon Animator")]
    public Animator Animator;

    [Header("Weapon Attributes")]
    public int EnergyCost;

    public Health Health;

    void Start()
    {
        XRGrabInteractable grabbable = GetComponent<XRGrabInteractable>();
        grabbable.activated.AddListener(FireBullet);
        int energy = Health.CurrentEnergy;
    }
    public void FireBullet(ActivateEventArgs Arg)
    {
        
        if (ChargingUpClip != null)
        {
            AudioSource.PlayOneShot(ChargingUpClip);
            Animator.Play("ChargingUp");
        }
        int energy = Health.CurrentEnergy;

        Animator.Play("Shoot");
        // Instantiate the bullet, play audio, and trigger animation as before...
        GameObject spawnedBullet = Instantiate(Bullet, MuzzleTransform.position, MuzzleTransform.rotation);
        spawnedBullet.transform.Rotate(Vector3.up, 0f);
        AudioSource.PlayOneShot(ShootClip);
        ConsumeEnergy();
    }

    public void ConsumeEnergy()
    {
        Health.DepleteEnergy(EnergyCost);
    }

    public void Disengage()
    {
        if (!DisengageClip)
        {
            AudioSource.PlayOneShot(DisengageClip);
        }
        
    }

    
}
