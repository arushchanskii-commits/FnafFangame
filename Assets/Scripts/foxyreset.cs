using UnityEngine;

public class DoorClick : MonoBehaviour
{
    [Header("Target")]
    [Tooltip("The animatronic to check and reset.")]
    public AnimatronicAI animatronic;

    [Tooltip("The room the animatronic must be in for the reset to trigger.")]
    public Room triggerRoom;

    [Header("Sound")]
    [Tooltip("Plays when the reset triggers. Needs an AudioSource on this GameObject.")]
    public AudioClip resetSound;

    private AudioSource _audioSource;

    // ──────────────────────────────────────────────────────────────

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnMouseDown()
    {
        if (animatronic == null)
        {
            Debug.LogWarning("[DoorClick] No animatronic assigned.");
            return;
        }

        if (triggerRoom == null)
        {
            Debug.LogWarning("[DoorClick] No trigger room assigned.");
            return;
        }

        if (animatronic.CurrentRoom == triggerRoom)
        {
            Debug.Log($"[DoorClick] {animatronic.animatronicName} was in {triggerRoom.roomName} – resetting to first room.");
            animatronic.ResetToRoom(0);
            PlayResetSound();
        }
        else
        {
            Debug.Log($"[DoorClick] {animatronic.animatronicName} is not in {triggerRoom.roomName} – no reset.");
        }
    }

    private void PlayResetSound()
    {
        if (resetSound == null) return;

        if (_audioSource != null)
        {
            _audioSource.PlayOneShot(resetSound);
        }
        else
        {
            // Fallback: plays at world position without needing an AudioSource component
            AudioSource.PlayClipAtPoint(resetSound, transform.position);
        }
    }
}
