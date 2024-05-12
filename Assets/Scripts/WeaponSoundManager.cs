using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSoundManager : MonoBehaviour
{
    public AudioSource WeaponAudioSource;

    // Arrays for different types of sound clips
    public AudioClip[] ShootClips;
    public AudioClip[] ChargeClips;
    public AudioClip[] LowEnergyClips;
    public AudioClip[] ImpactClips;
    public AudioClip[] SkillActivate;
    public AudioClip[] SkillDeactivate;
    public AudioClip[] SkillCooldown;
    public AudioClip[] SkillImpact;

    // Adjust this value to change the pitch variation
    public float pitchVariation = 0.1f;

    public void PlayWeaponShootClip()
    {
        PlayRandomSound(ShootClips);
    }

    public void PlayWeaponChargeClip()
    {
        PlayRandomSound(ChargeClips);
    }

    public void PlayWeaponLowEnergyClip()
    {
        PlayRandomSound(LowEnergyClips);
    }

    public void PlayWeaponImpactClips()
    {
        PlayRandomSound(ImpactClips);
    }

    public void PlaySkillActivate()
    {
        PlayRandomSound(SkillActivate);
    }

    public void PlaySkillDeactivate()
    {
        PlayRandomSound(SkillDeactivate);
    }
    public void PlaySkillCooldown()
    {
        PlayRandomSound(SkillCooldown);
    }
    public void PlaySkillImpact()
    {
        PlayRandomSound(SkillImpact);
    }


    private void PlayRandomSound(AudioClip[] clips)
    {
        if (clips != null && clips.Length > 0)
        {
            // Select a random clip from the array
            AudioClip clipToPlay = clips[Random.Range(0, clips.Length)];
            // Apply pitch variation
            WeaponAudioSource.pitch = 1f + Random.Range(-pitchVariation, pitchVariation);
            // Set the audio clip
            WeaponAudioSource.clip = clipToPlay;
            // Play the sound
            WeaponAudioSource.Play();
        }
    }
}
