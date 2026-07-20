using UnityEngine;

public class FreddyNose : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioClip noseSound;
    public float soundVolume = 1f;
    
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void OnMouseDown()
    {
        if (noseSound != null)
        {
            audioSource.PlayOneShot(noseSound, soundVolume);
        }
    }
}
