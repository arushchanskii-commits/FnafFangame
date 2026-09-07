using UnityEngine;

public class TelephoneAudio : MonoBehaviour
{
    [Header("Telephone Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip telephoneClip;
    [Min(0f)]
    [SerializeField] private float playbackDelay = 5f;
    [Min(0f)]
    [SerializeField] private float uiAppearDelay = 5f;

    [Header("F Key Interaction")]
    [SerializeField] private GameObject targetUIObject;
    [SerializeField] private KeyCode stopKey = KeyCode.F;

    [Header("Animatronic Movement Detection")]
    [SerializeField] private MainCameraButton mainCameraButton;
    [SerializeField] private GameObject animatronicAlertUI;
    [SerializeField] private AudioClip movementSoundEffect;
    [Min(0f)]
    [SerializeField] private float alertUIDuration = 2f;

    private float playbackTimer;
    private bool playbackStarted;
    private Coroutine uiAppearCoroutine;
    private Coroutine alertUICoroutine;

    private void Start()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        playbackTimer = Mathf.Max(0f, playbackDelay);

        if (targetUIObject != null)
        {
            targetUIObject.SetActive(false);
            uiAppearCoroutine = StartCoroutine(ShowUIAfterDelay());
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(stopKey))
        {
            StopTelephone();
            return;
        }

        if (playbackStarted && audioSource != null && !audioSource.isPlaying)
        {
            HideTargetUI();
            return;
        }

        if (playbackStarted || telephoneClip == null || audioSource == null)
        {
            return;
        }

        playbackTimer -= Time.deltaTime;
        if (playbackTimer <= 0f)
        {
            audioSource.clip = telephoneClip;
            audioSource.Play();
            playbackStarted = true;
        }
    }

    private void StopTelephone()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
        }

        HideTargetUI();

        if (targetUIObject != null)
        {
            if (uiAppearCoroutine != null)
            {
                StopCoroutine(uiAppearCoroutine);
            }

            uiAppearCoroutine = StartCoroutine(ShowUIAfterDelay());
        }
    }

    private void HideTargetUI()
    {
        if (targetUIObject != null)
        {
            targetUIObject.SetActive(false);
        }
    }

    private System.Collections.IEnumerator ShowUIAfterDelay()
    {
        yield return new WaitForSeconds(Mathf.Max(0f, uiAppearDelay));

        if (targetUIObject != null)
        {
            targetUIObject.SetActive(true);
        }

        uiAppearCoroutine = null;
    }

    public void OnAnimatronicMove()
    {
        if (mainCameraButton != null && mainCameraButton.isCameraOpen && animatronicAlertUI != null)
        {
            if (alertUICoroutine != null)
            {
                StopCoroutine(alertUICoroutine);
            }

            alertUICoroutine = StartCoroutine(ShowAlertUI());
        }
    }

    private System.Collections.IEnumerator ShowAlertUI()
    {
        if (animatronicAlertUI != null)
        {
            animatronicAlertUI.SetActive(true);
        }

        if (movementSoundEffect != null && audioSource != null)
        {
            audioSource.PlayOneShot(movementSoundEffect);
        }

        yield return new WaitForSeconds(Mathf.Max(0f, alertUIDuration));

        if (animatronicAlertUI != null)
        {
            animatronicAlertUI.SetActive(false);
        }

        alertUICoroutine = null;
    }
}
