using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class backgroundsounds : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioClip targetAudio;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        PlayAssignedAudio();
    }

    public void PlayAssignedAudio()
    {
        if (targetAudio != null && audioSource != null)
        {
            audioSource.clip = targetAudio;
            audioSource.Play();
        }
    }
}
