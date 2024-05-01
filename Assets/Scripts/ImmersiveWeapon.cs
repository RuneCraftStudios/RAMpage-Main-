using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ImmersiveWeapon : MonoBehaviour
{
    [Header("Select Bullet")]
    public GameObject Bullet;
    public Transform MuzzleTransform;
    public GameObject MuzzleFlash;

    public WeaponSoundManager SoundManager;

    [Header("Weapon Attributes")]
    public int EnergyCost;
    public float RecoilForceMagnitude;  // Magnitude of the recoil force
    public float RecoilTorqueMagnitude; // Magnitude of the recoil torque

    private Health health;
    private Rigidbody weaponRigidbody;

    void Start()
    {
        XRGrabInteractable grabbable = GetComponent<XRGrabInteractable>();
        grabbable.activated.AddListener(FireBullet);
        weaponRigidbody = GetComponent<Rigidbody>();
        GameObject playerGameObject = GameObject.Find("Player");
        if (playerGameObject != null)
        {
            // Get the Health component from the player GameObject
            health = playerGameObject.GetComponent<Health>();
        }
        else
        {
            Debug.LogError("Player GameObject not found!");
        }
    }

    public void FireBullet(ActivateEventArgs Arg)
    {
        
        if (health.CurrentEnergy > EnergyCost)
        {
            GameObject spawnedBullet = Instantiate(Bullet, MuzzleTransform.position, MuzzleTransform.rotation);
            InstantiateMuzzleFlash();
            SoundManager.PlayWeaponShootClip();
            ConsumeEnergy();
            ApplyRecoil();
        }
        else
        {
            SoundManager.PlayWeaponLowEnergyClip();
        }
           
    }

    private void InstantiateMuzzleFlash()
    {
        GameObject muzzleFlashInstance = Instantiate(MuzzleFlash, MuzzleTransform.position, MuzzleTransform.rotation);
        StartCoroutine(DestroyAfterDelay(muzzleFlashInstance, 0.15f));
    }

    private void ApplyRecoil()
    {
        // Applying backward force
        weaponRigidbody.AddRelativeForce(Vector3.back * RecoilForceMagnitude, ForceMode.Impulse);

        // Applying rotational torque
        weaponRigidbody.AddRelativeTorque(Vector3.right * RecoilTorqueMagnitude, ForceMode.Impulse);
    }

    public void ConsumeEnergy()
    {
        health.DepleteEnergy(EnergyCost);
    }

    private IEnumerator DestroyAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(obj);
    }
}
