using UnityEngine;

public class jumpscare : MonoBehaviour
{
    public GameObject scareObject;
    public AudioClip scareSound;
    public circleclicking scoreSource;
    public float disappearDelay = 4f;

    private AudioSource audioSource;
    private CirlcePath pathSource;
    private bool hasTriggered;
    private float scareTimer;

    void Start()
    {
        if (scareObject != null)
        {
            if (scareObject == gameObject)
            {
                Debug.LogWarning("jumpscare: scareObject should not be the same GameObject as the jumpscare controller.");
            }
            else
            {
                scareObject.SetActive(false);
            }
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource != null)
        {
            audioSource.playOnAwake = false;
        }

        if (audioSource == null && scareSound != null)
        {
            Debug.LogWarning("jumpscare needs an AudioSource on the same GameObject to play the scare sound.");
        }

        if (scoreSource == null)
        {
            scoreSource = FindObjectOfType<circleclicking>();
        }

        if (scoreSource != null)
        {
            pathSource = scoreSource.GetComponent<CirlcePath>();
        }
    }

    void Update()
    {
        if (scoreSource == null)
        {
            return;
        }

        bool isBelowZero = scoreSource.score < 0;
        if (isBelowZero && !hasTriggered)
        {
            hasTriggered = true;
            scareTimer = disappearDelay;
            if (scareObject != null)
            {
                scareObject.SetActive(true);
                scareObject.transform.SetAsLastSibling();
                var canvasGroup = scareObject.GetComponent<CanvasGroup>();
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = 1f;
                    canvasGroup.blocksRaycasts = true;
                }
                Debug.Log("jumpscare: activating scareObject " + scareObject.name);
            }

            if (scareSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(scareSound);
            }
        }

        if (hasTriggered && scareTimer > 0f)
        {
            scareTimer -= Time.deltaTime;
            if (scareTimer <= 0f)
            {
                if (scareObject != null)
                {
                    scareObject.SetActive(false);
                }

                if (scoreSource != null)
                {
                    scoreSource.score = 0;
                }

                if (pathSource != null)
                {
                    pathSource.ResetSpeedToInitial();
                }
            }
        }

        if (!isBelowZero)
        {
            hasTriggered = false;
            scareTimer = 0f;
            if (scareObject != null)
            {
                scareObject.SetActive(false);
            }
        }
    }
}
