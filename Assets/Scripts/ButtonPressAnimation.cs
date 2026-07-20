using UnityEngine;

public class ButtonPressAnimation : MonoBehaviour
{
    [Header("Scale Settings")]
    public float xScaleAmount = 0.9f;
    
    [Header("Return Settings")]
    public float returnDelay = 0.1f;
    
    [Header("Audio")]
    public AudioClip pressSound;
    public float soundVolume = 1f;
    
    private Vector3 originalScale;
    private bool isScaling = false;
    private AudioSource audioSource;

    private void Start()
    {
        originalScale = transform.localScale;
        audioSource = GetComponent<AudioSource>();
        
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void OnMouseDown()
    {
        if (isScaling) return;
        isScaling = true;
        
        if (pressSound != null)
        {
            audioSource.PlayOneShot(pressSound, soundVolume);
        }
        
        transform.localScale = new Vector3(originalScale.x * xScaleAmount, originalScale.y, originalScale.z);
    }

    private void OnMouseUp()
    {
        if (!isScaling) return;
        isScaling = false;
        
        transform.localScale = originalScale;
    }
}
