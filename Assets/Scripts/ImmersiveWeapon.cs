using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ImmersiveWeapon : MonoBehaviour
{
    [Header("Select Bullet")]
    public GameObject Bullet;
    public Transform MuzzleTransform;

    [Header("Select Sound Source")]
    public AudioSource AudioSource;

    [Header("AudioClip Inputs")]
    public AudioClip ShootClip;
    public AudioClip NoEnergyClip;

    [Header("Weapon Attributes")]
    public int EnergyCost;
    public float RecoilForceMagnitude;  // Magnitude of the recoil force
    public float RecoilTorqueMagnitude; // Magnitude of the recoil torque

    public Health Health;
    private Rigidbody weaponRigidbody;

    void Start()
    {
        XRGrabInteractable grabbable = GetComponent<XRGrabInteractable>();
        grabbable.activated.AddListener(FireBullet);
        weaponRigidbody = GetComponent<Rigidbody>();
    }

    public void FireBullet(ActivateEventArgs Arg)
    {
        if (Health.CurrentEnergy >= EnergyCost)
        {
            GameObject spawnedBullet = Instantiate(Bullet, MuzzleTransform.position, MuzzleTransform.rotation);
            AudioSource.PlayOneShot(ShootClip);
            ConsumeEnergy();
            ApplyRecoil();
        }
        else
        {
            AudioSource.PlayOneShot(NoEnergyClip);
        }
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
        Health.DepleteEnergy(EnergyCost);
    }
}
