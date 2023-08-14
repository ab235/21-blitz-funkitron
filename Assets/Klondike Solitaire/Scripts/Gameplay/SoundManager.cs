using UnityEngine;
using System.Collections;
using System;

// The SoundManager class is responsible for playing audio in the game.
public class SoundManager : MonoBehaviour {

    // Reference to the AudioSource component
    private AudioSource audioSource;

    // Static reference to the SoundManager instance
    public static SoundManager instance;

    // Audio clips for different sound effects
    public AudioClip reverseCardSound;
    public AudioClip dropCardSound;
    public AudioClip pickCardSound;
    public AudioClip btnClickSound;

    // Boolean to lock audio playback
    private bool locked = false;

    void Awake()
    {
        // Set the static instance reference to this instance of SoundManager
        instance = this;

        // Get reference to the AudioSource component
        audioSource = GetComponent<AudioSource>();
    }

    // Play the reverse card sound effect
    public void PlayReverseCardSound()
    {
        PlaySound(reverseCardSound);
    }

    // Play the pick card sound effect
    public void PlayPickCardSound()
    {
        PlaySound(pickCardSound);
    }

    // Play the drop card sound effect
    public void PlayDropCardSound()
    {
        PlaySound(dropCardSound);
    }

    // Play the button click sound effect
    public void PlayBtnClickSound()
    {
        PlaySound(btnClickSound, true);
    }

    // Play a sound effect
    private void PlaySound(AudioClip audioClip, bool isPrioritySound = false)
    {
        // Only play the sound if audio is not locked or if it's a priority sound
        if (!locked || isPrioritySound)
        {
            // Lock audio playback
            locked = true;

            // Play the sound effect
            audioSource.PlayOneShot(audioClip);

            // Invoke the Unlock method after Constants.SOUND_LOCK_TIME seconds
            Invoke("Unlock", Constants.SOUND_LOCK_TIME);
        }
    }

    // Unlock audio playback
    void Unlock()
    {
        locked = false;
    }
}