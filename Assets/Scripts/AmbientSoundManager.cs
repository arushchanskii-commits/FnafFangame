using UnityEngine;

public class AmbientSoundManager : MonoBehaviour
{
    [Header("Target Sound")]
    public AudioClip targetSound;
    public float soundVolume = 0.5f;
    public bool loop = true;
    
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        audioSource.playOnAwake = true;
        
        if (targetSound != null)
        {
            audioSource.clip = targetSound;
            audioSource.volume = soundVolume;
            audioSource.loop = loop;
        }
    }

    private void PlaySound()
    {
        if (audioSource != null && targetSound != null)
        {
            audioSource.Play();
        }
    }
}
