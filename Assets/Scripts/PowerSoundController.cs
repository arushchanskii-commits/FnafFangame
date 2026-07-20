using UnityEngine;

public class PowerSoundController : MonoBehaviour
{
    [Header("Audio Clips")]
    public AudioClip powerOnSound;
    public AudioClip powerOutageSound;
    
    [Header("Audio Settings")]
    public float powerOnVolume = 0.5f;
    public float powerOutageVolume = 0.7f;
    public bool loopPowerOnSound = true;
    public bool loopPowerOutageSound = true;
    
    private AudioSource audioSource;
    private bool powerOutage = false;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        // Start with power on sound
        if (powerOnSound != null && PowerManager.Instance != null && PowerManager.Instance.CanUseDevice())
        {
            PlayPowerOnSound();
        }
        else if (PowerManager.Instance != null && !PowerManager.Instance.CanUseDevice())
        {
            // If already out of power, play outage sound
            PlayPowerOutageSound();
        }
    }

    private void Update()
    {
        // Check if power ran out
        if (PowerManager.Instance != null && !PowerManager.Instance.CanUseDevice() && !powerOutage)
        {
            Debug.Log("Power ran out! Switching to outage sound");
            powerOutage = true;
            SwitchToPowerOutageSound();
        }
        // Check if power was restored
        else if (PowerManager.Instance != null && PowerManager.Instance.CanUseDevice() && powerOutage)
        {
            Debug.Log("Power restored! Switching back to power on sound");
            powerOutage = false;
            SwitchToPowerOnSound();
        }
    }

    private void PlayPowerOnSound()
    {
        if (powerOnSound != null)
        {
            audioSource.clip = powerOnSound;
            audioSource.volume = powerOnVolume;
            audioSource.loop = loopPowerOnSound;
            audioSource.Play();
        }
    }

    private void PlayPowerOutageSound()
    {
        if (powerOutageSound != null)
        {
            audioSource.clip = powerOutageSound;
            audioSource.volume = powerOutageVolume;
            audioSource.loop = loopPowerOutageSound;
            audioSource.Play();
        }
    }

    private void SwitchToPowerOutageSound()
    {
        // Stop current sound and play outage sound
        if (powerOnSound != null)
        {
            audioSource.Stop();
        }
        PlayPowerOutageSound();
    }

    private void SwitchToPowerOnSound()
    {
        // Stop current sound and play power on sound
        if (powerOutageSound != null)
        {
            audioSource.Stop();
        }
        PlayPowerOnSound();
    }

    public void StopAllSounds()
    {
        audioSource.Stop();
    }
}
