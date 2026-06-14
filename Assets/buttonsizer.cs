using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(AudioSource))]
public class buttonsizer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Tooltip("Multiplier applied to the original scale when the cursor hovers over this object.")]
    public float scaleMultiplier = 1.2f;

    [Tooltip("Sound played once when the cursor first enters the object.")]
    public AudioClip targetSound;

    [Tooltip("How fast the object interpolates to the hover scale.")]
    public float transitionSpeed = 10f;

    private AudioSource audioSource;
    private Vector3 originalScale;
    private Vector3 desiredScale;

    void Start()
    {
        originalScale = transform.localScale;
        desiredScale = originalScale;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
        audioSource.loop = false;

        if (targetSound != null)
        {
            audioSource.clip = targetSound;
        }
    }

    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, desiredScale, Time.deltaTime * transitionSpeed);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        desiredScale = originalScale * scaleMultiplier;
        PlayHoverSound();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        desiredScale = originalScale;
    }

    void OnMouseEnter()
    {
        desiredScale = originalScale * scaleMultiplier;
        PlayHoverSound();
    }

    void OnMouseExit()
    {
        desiredScale = originalScale;
    }

    private void PlayHoverSound()
    {
        if (targetSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(targetSound);
        }
    }
}
