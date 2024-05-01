using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySoundManager : MonoBehaviour
{
    public AudioSource enemyAudioSource;

    // Arrays for different types of sound clips
    public AudioClip[] enemyRelocateClips;
    public AudioClip[] enemySearchClips;
    public AudioClip[] enemyTargetSightedClips;
    public AudioClip[] enemyTakeDamageClips;
    public AudioClip[] enemyDeathClips;
    public AudioClip[] enemyAttackClips;
    public AudioClip[] enemyGrenadeClips;
    public AudioClip[] enemyLocateClips;
    public AudioClip[] enemyExplodeClips;
    public AudioClip[] enemyChargeClips;

    // Adjust this value to change the pitch variation
    public float pitchVariation = 0.1f;

    public void PlayEnemyRelocateSound()
    {
        PlayRandomSound(enemyRelocateClips);
    }

    public void PlayEnemySearchSound()
    {
        PlayRandomSound(enemySearchClips);
    }

    public void PlayEnemyTargetSightedSound()
    {
        PlayRandomSound(enemyTargetSightedClips);
    }

    public void PlayEnemyAttackSound()
    {
        PlayRandomSound(enemyAttackClips);
    }

    public void PlayEnemyTakeDamageSound()
    {
        PlayRandomSound(enemyTakeDamageClips);
    }

    public void PlayEnemyDeathSound()
    {
        PlayRandomSound(enemyDeathClips);
    }

    public void PlayEnemyGrenadeSound()
    {
        PlayRandomSound(enemyGrenadeClips);
    }

    public void PlayEnemyLocateSound()
    {
        PlayRandomSound(enemyLocateClips);
    }

    public void PlayEnemyExplodeSound()
    {
        PlayRandomSound(enemyExplodeClips);
    }

    public void PlayEnemyChargeSound()
    {
        PlayRandomSound(enemyChargeClips);
    }
    

    private void PlayRandomSound(AudioClip[] clips)
    {
        if (clips != null && clips.Length > 0)
        {
            // Select a random clip from the array
            AudioClip clipToPlay = clips[Random.Range(0, clips.Length)];
            // Apply pitch variation
            enemyAudioSource.pitch = 1f + Random.Range(-pitchVariation, pitchVariation);
            // Set the audio clip
            enemyAudioSource.clip = clipToPlay;
            // Play the sound
            enemyAudioSource.Play();
        }
    }
}
