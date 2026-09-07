using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Shows a UI GameObject and plays a sound whenever a tracked animatronic moves
/// while the player is watching the cameras.
/// Attach this to any persistent GameObject (e.g. the GameManager or HUD).
/// </summary>
public class MovementAlert : MonoBehaviour
{
    [Header("Animatronics to Track")]
    [Tooltip("All animatronics that should trigger the alert when they move.")]
    public List<AnimatronicAI> animatronics = new();

    [Header("Alert UI")]
    [Tooltip("The UI GameObject to show briefly when an animatronic moves (e.g. a '!' icon or a flash panel).")]
    public GameObject alertObject;

    [Tooltip("How long the alert stays visible, in seconds.")]
    public float displayDuration = 1.5f;

    [Header("Alert Sound")]
    [Tooltip("Sound to play when the alert triggers. Needs an AudioSource on this GameObject.")]
    public AudioClip alertSound;

    private AudioSource _audioSource;
    private Coroutine   _hideRoutine;

    // ──────────────────────────────────────────────────────────────

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();

        if (alertObject != null)
            alertObject.SetActive(false);
    }

    private void OnEnable()
    {
        foreach (var anim in animatronics)
        {
            if (anim != null)
                anim.OnMoved += OnAnimatronicMoved;
        }
    }

    private void OnDisable()
    {
        foreach (var anim in animatronics)
        {
            if (anim != null)
                anim.OnMoved -= OnAnimatronicMoved;
        }
    }

    // ──────────────────────────────────────────────────────────────

    private void OnAnimatronicMoved()
    {
        // Only trigger when the player has the camera monitor open
        if (CameraManager.Instance == null || !CameraManager.Instance.IsWatchingCameras)
            return;

        ShowAlert();
    }

    private void ShowAlert()
    {
        if (alertObject != null)
            alertObject.SetActive(true);

        PlaySound();

        // Restart the hide timer so rapid moves extend the display correctly
        if (_hideRoutine != null)
            StopCoroutine(_hideRoutine);

        _hideRoutine = StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration);

        if (alertObject != null)
            alertObject.SetActive(false);

        _hideRoutine = null;
    }

    private void PlaySound()
    {
        if (alertSound == null) return;

        if (_audioSource != null)
            _audioSource.PlayOneShot(alertSound);
        else
            AudioSource.PlayClipAtPoint(alertSound, transform.position);
    }
}
