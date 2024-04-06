using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;

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

    [Header("Weapon Animator")]
    public Animator Animator;

    [Header("Weapon Attributes")]
    public float WeaponChargeDelay;
    public int EnergyCost;

    public Health Health;

    public void Fire()
    {
        Animator.SetBool("ChargingUp", true);
        AudioSource.PlayOneShot(ChargingUpClip);
        StartCoroutine(Firing());
    }

    public void ConsumeEnergy()
    {
        Health.DepleteEnergy(EnergyCost);
    }

    public void Disengage()
    {
        AudioSource.PlayOneShot(DisengageClip);
    }

    private IEnumerator Firing()
    {
        yield return new WaitForSeconds(WeaponChargeDelay);

        // Instantiate the bullet, play audio, and trigger animation as before...
        GameObject spawnedBullet = Instantiate(Bullet, MuzzleTransform.position, MuzzleTransform.rotation);
        spawnedBullet.transform.Rotate(Vector3.up, 180f);
        AudioSource.PlayOneShot(ShootClip);
        ConsumeEnergy();
    }
}
