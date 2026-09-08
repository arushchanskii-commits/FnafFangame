using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Shows a UI GameObject and plays a sound whenever an animatronic moves
/// while the player is watching the cameras.
/// Attach this to any persistent GameObject (e.g. the GameManager or HUD).
/// </summary>
public class MovementAlert : MonoBehaviour
{
    [Header("Animatronics to Track")]
    [Tooltip("Optional filter. Leave empty to track every AnimatronicAI in the scene.")]
    public List<AnimatronicAI> animatronics = new();

    [Header("Alert UI")]
    [Tooltip("The UI GameObject to show briefly when an animatronic moves (e.g. a '!' icon or a flash panel).")]
    public GameObject alertObject;

    [Tooltip("How long the alert stays visible, in seconds.")]
    public float displayDuration = 1.5f;

    [Header("Alert Sound")]
    [Tooltip("Sound to play when the alert triggers.")]
    public AudioClip alertSound;

    [Tooltip("Optional AudioSource used to play the alert sound. If empty, the sound plays at this object's position.")]
    public AudioSource audioSource;

    private AudioSource _audioSource;
    private Coroutine   _hideRoutine;

    // ──────────────────────────────────────────────────────────────

    private void Awake()
    {
        _audioSource = audioSource != null ? audioSource : GetComponent<AudioSource>();

        if (_audioSource == null)
            _audioSource = gameObject.AddComponent<AudioSource>();

        if (alertObject != null)
            alertObject.SetActive(false);
    }

    private void OnEnable()
    {
        AnimatronicAI.OnAnyMoved += OnAnimatronicMoved;
    }

    private void OnDisable()
    {
        AnimatronicAI.OnAnyMoved -= OnAnimatronicMoved;

        HideAlert();
    }

    // ──────────────────────────────────────────────────────────────

    private void OnAnimatronicMoved(AnimatronicAI animatronic)
    {
        if (animatronics.Count > 0 && !animatronics.Contains(animatronic))
            return;

        // Only trigger when the player has the camera monitor open
        if (!IsWatchingCameras())
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

    private void Update()
    {
        if (alertObject != null && alertObject.activeSelf && !IsWatchingCameras())
            HideAlert();
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration);

        HideAlert();
    }

    private void HideAlert()
    {
        if (alertObject != null)
            alertObject.SetActive(false);

        if (_audioSource != null)
            _audioSource.Stop();

        if (_hideRoutine != null)
        {
            StopCoroutine(_hideRoutine);
            _hideRoutine = null;
        }
    }

    private bool IsWatchingCameras()
    {
        bool watchingThroughCameraManager = CameraManager.Instance != null && CameraManager.Instance.IsWatchingCameras;
        bool watchingThroughCameraSystem = CameraSystem.Instance != null && CameraSystem.Instance.IsWatchingCameras;
        return watchingThroughCameraManager || watchingThroughCameraSystem;
    }

    private void PlaySound()
    {
        if (alertSound == null) return;

        if (_audioSource != null)
            _audioSource.PlayOneShot(alertSound);
    }
}
