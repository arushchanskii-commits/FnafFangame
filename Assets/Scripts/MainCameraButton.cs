using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MainCameraButton : MonoBehaviour
{
    [Header("Camera Buttons")]
    public GameObject[] cameraButtons;
    
    [Header("Audio")]
    public AudioClip openSound;
    public AudioClip closeSound;
    public float soundVolume = 1f;
    
    [Header("Animation")]
    public Animator animator;
    public Animation legacyAnimation;
    public Image animationImage;
    public GameObject animationTarget;
    public string animationTargetName = "PullDownAnimation";
    public string openTrigger = "Open";
    public string closeTrigger = "Close";
    public string openAnimationClipName = "PullingDownCams";
    public string closeAnimationClipName = "PullingUpCams";
    
    [Header("Main Camera Reference")]
    public Camera mainCamera;
    
    [Header("Camera Views")]
    public Camera[] cameraViews;
    public Camera[] swappableCameras;
    public int currentCameraIndex = 0;
    public int defaultCameraIndex = 0;
    
    [Header("Generator Reference")]
    public PowerCharger powerCharger;
    
    [Header("Office Camera Position (Set in Inspector)")]
    [Tooltip("The position the camera should return to when cameras are closed")]
    public Vector3 originalCameraPosition = new Vector3(0f, 0f, 0f);
    [Tooltip("The rotation the camera should return to when cameras are closed")]
    public Quaternion originalCameraRotation = Quaternion.identity;
    
    private AudioSource audioSource;
    public bool isCameraOpen = false;
    public bool isAnimating = false;
    private float lastToggleTime = 0f;
    private float toggleCooldown = 0.3f;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        audioSource.playOnAwake = false;
        
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        
        if (mainCamera != null)
        {
            if (originalCameraPosition == Vector3.zero && originalCameraRotation == Quaternion.identity)
            {
                originalCameraPosition = mainCamera.transform.position;
                originalCameraRotation = mainCamera.transform.rotation;
            }
        }
        
        foreach (GameObject button in cameraButtons)
        {
            if (button != null)
                button.SetActive(false);
        }

        ResolveAnimationComponents();
        SetAnimationImageRaycasts(false);
    }

    private void Update()
    {
        // Space bar toggle disabled - only use mouse click on camera button
        
        // Check for power outage and close cameras automatically
        // Only trigger if power actually went out (not just during charging)
        if (PowerManager.Instance != null && !PowerManager.Instance.CanUseDevice() && isCameraOpen && !isAnimating)
        {
            // Only force close if power is actually out (0%), not just during charging
            if (PowerManager.Instance.currentPower <= 0)
            {
                ForceCloseCameras();
            }
        }
    }

    public void ToggleCameraButtons()
    {
        if (isAnimating)
        {
            return;
        }

        ResolveAnimationComponents();
        
        Debug.Log("ToggleCameraButtons called!");
        isCameraOpen = !isCameraOpen;
        Debug.Log($"Camera state: {isCameraOpen}");
        
        foreach (GameObject button in cameraButtons)
        {
            if (button != null)
            {
                button.SetActive(isCameraOpen);
                Debug.Log($"Camera button active: {isCameraOpen}");
            }
        }

        SetAnimationImageRaycasts(false);
        
        if (isCameraOpen)
        {
            if (openSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(openSound, soundVolume);
                Debug.Log("Playing open sound");
            }
            else
            {
                Debug.LogWarning("Open sound not configured");
            }
            
            // Register camera power consumption
            if (PowerManager.Instance != null)
            {
                PowerManager.Instance.RegisterDevice(PowerManager.DeviceType.Camera);
            }
            
            // Switch to camera view
            if (cameraViews != null && cameraViews.Length > 0)
            {
                if (currentCameraIndex >= cameraViews.Length)
                {
                    currentCameraIndex = 0;
                }
                
                Camera targetCamera = cameraViews[currentCameraIndex];
                if (targetCamera != null && mainCamera != null)
                {
                    mainCamera.transform.position = targetCamera.transform.position;
                    mainCamera.transform.rotation = targetCamera.transform.rotation;
                    Debug.Log($"Switched to camera view {currentCameraIndex + 1}");
                }
            }
            
            if (!isAnimating)
            {
                if (HasLegacyClip(openAnimationClipName))
                {
                    Debug.Log("Legacy Animation clip found, playing open clip");
                    StartCoroutine(PlayLegacyAnimationOnce(openAnimationClipName));
                }
                else if (ShouldUseAnimator(openTrigger))
                {
                    Debug.Log("Animator found, triggering open animation");
                    StartCoroutine(PlayAnimationOnce(openTrigger));
                }
                else if (legacyAnimation != null)
                {
                    Debug.Log("Legacy Animation found, playing open clip");
                    StartCoroutine(PlayLegacyAnimationOnce(openAnimationClipName));
                }
                else
                {
                    Debug.LogWarning("No Animator or legacy Animation component is assigned for the open animation.");
                }
            }
        }
        else
        {
            if (closeSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(closeSound, soundVolume);
                Debug.Log("Playing close sound");
            }
            else
            {
                Debug.LogWarning("Close sound not configured");
            }
            
            // Unregister camera power consumption
            if (PowerManager.Instance != null)
            {
                PowerManager.Instance.UnregisterDevice(PowerManager.DeviceType.Camera);
            }
            
            if (!isAnimating)
            {
                if (HasLegacyClip(closeAnimationClipName))
                {
                    Debug.Log("Legacy Animation clip found, playing close clip");
                    StartCoroutine(PlayLegacyAnimationOnce(closeAnimationClipName));
                }
                else if (ShouldUseAnimator(closeTrigger))
                {
                    Debug.Log("Animator found, triggering close animation");
                    StartCoroutine(PlayAnimationOnce(closeTrigger));
                }
                else if (legacyAnimation != null)
                {
                    Debug.Log("Legacy Animation found, playing close clip");
                    StartCoroutine(PlayLegacyAnimationOnce(closeAnimationClipName));
                }
                else
                {
                    Debug.LogWarning("No Animator or legacy Animation component is assigned for the close animation.");
                }
            }
            
            if (mainCamera != null)
            {
                mainCamera.transform.position = originalCameraPosition;
                Debug.Log("Camera position reset");
            }

            SwitchToDefaultCamera();
        }
    }

    private void ResolveAnimationComponents()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        if (legacyAnimation == null)
        {
            legacyAnimation = FindLegacyAnimation();
        }

        if (animator == null && legacyAnimation == null)
        {
            TryFindAnimationComponentByName();
        }

        if (legacyAnimation == null)
        {
            TryFindLegacyAnimationByClipNames();
        }

        if (animationTarget == null)
        {
            if (animator != null)
            {
                animationTarget = animator.gameObject;
            }
            else if (legacyAnimation != null)
            {
                animationTarget = legacyAnimation.gameObject;
            }
        }

        if (animationImage == null)
        {
            animationImage = FindAnimationImage();
        }

        if (animationTarget != null && animationImage == null)
        {
            if (animationTarget.GetComponent<Image>() == null)
            {
                Image createdImage = animationTarget.AddComponent<Image>();
                animationImage = createdImage;
                Debug.LogWarning($"Added Image component to {animationTarget.name} so the sprite animation can be displayed.");
            }
            else
            {
                animationImage = animationTarget.GetComponent<Image>();
            }
        }
    }

    private Animation FindLegacyAnimation()
    {
        if (animationTarget != null)
        {
            Animation foundAnimation = animationTarget.GetComponent<Animation>();
            if (foundAnimation != null)
            {
                return foundAnimation;
            }

            foundAnimation = animationTarget.GetComponentInChildren<Animation>();
            if (foundAnimation != null)
            {
                return foundAnimation;
            }
        }

        if (!string.IsNullOrEmpty(animationTargetName))
        {
            GameObject targetObject = GameObject.Find(animationTargetName);
            if (targetObject != null)
            {
                Animation foundAnimation = targetObject.GetComponent<Animation>();
                if (foundAnimation != null)
                {
                    return foundAnimation;
                }

                return targetObject.GetComponentInChildren<Animation>();
            }
        }

        return null;
    }

    private bool TryFindAnimationComponentByName()
    {
        if (string.IsNullOrEmpty(animationTargetName))
        {
            animationTargetName = "PullDownAnimation";
        }

        Animator preferredAnimator = null;
        Animation preferredLegacyAnimation = null;

        foreach (Animator candidateAnimator in FindObjectsOfType<Animator>())
        {
            if (candidateAnimator == null || candidateAnimator.gameObject == null)
            {
                continue;
            }

            string candidateName = candidateAnimator.gameObject.name;
            bool nameMatches = candidateName == animationTargetName || candidateName.Contains(animationTargetName) || candidateName.Contains("Cam") || candidateName.Contains("camera") || candidateName.Contains("Pull");
            bool hasMatchingTrigger = HasAnimatorTrigger(candidateAnimator, openTrigger) || HasAnimatorTrigger(candidateAnimator, closeTrigger);

            if (nameMatches || hasMatchingTrigger)
            {
                preferredAnimator = candidateAnimator;
                break;
            }
        }

        if (preferredAnimator != null)
        {
            animator = preferredAnimator;
            animationTarget = preferredAnimator.gameObject;
            return true;
        }

        foreach (Animation candidateAnimation in FindObjectsOfType<Animation>())
        {
            if (candidateAnimation == null || candidateAnimation.gameObject == null)
            {
                continue;
            }

            string candidateName = candidateAnimation.gameObject.name;
            bool nameMatches = candidateName == animationTargetName || candidateName.Contains(animationTargetName) || candidateName.Contains("Cam") || candidateName.Contains("camera") || candidateName.Contains("Pull");
            bool hasMatchingClip = candidateAnimation[openAnimationClipName] != null || candidateAnimation[closeAnimationClipName] != null;
            bool hasDisplayComponent = HasVisibleImage(candidateAnimation.gameObject);

            if ((nameMatches || hasMatchingClip) && hasDisplayComponent)
            {
                preferredLegacyAnimation = candidateAnimation;
                break;
            }
        }

        if (preferredLegacyAnimation != null)
        {
            legacyAnimation = preferredLegacyAnimation;
            animationTarget = preferredLegacyAnimation.gameObject;
            return true;
        }

        if (animationTarget != null)
        {
            Animator targetAnimator = animationTarget.GetComponent<Animator>();
            if (targetAnimator != null)
            {
                animator = targetAnimator;
                return true;
            }

            Animation targetAnimation = animationTarget.GetComponent<Animation>();
            if (targetAnimation != null)
            {
                legacyAnimation = targetAnimation;
                return true;
            }
        }

        return false;
    }

    private bool TryFindLegacyAnimationByClipNames()
    {
        foreach (Animation candidateAnimation in FindObjectsOfType<Animation>())
        {
            if (candidateAnimation == null || candidateAnimation.gameObject == null)
            {
                continue;
            }

            if ((candidateAnimation[openAnimationClipName] != null || candidateAnimation[closeAnimationClipName] != null) && HasVisibleImage(candidateAnimation.gameObject))
            {
                legacyAnimation = candidateAnimation;
                animationTarget = candidateAnimation.gameObject;
                return true;
            }
        }

        return false;
    }

    private bool HasAnimatorTrigger(Animator targetAnimator, string triggerName)
    {
        if (targetAnimator == null || string.IsNullOrEmpty(triggerName))
        {
            return false;
        }

        foreach (AnimatorControllerParameter parameter in targetAnimator.parameters)
        {
            if (parameter.type == AnimatorControllerParameterType.Trigger && parameter.name == triggerName)
            {
                return true;
            }
        }

        return false;
    }

    private bool HasVisibleImage(GameObject targetObject)
    {
        if (targetObject == null)
        {
            return false;
        }

        return targetObject.GetComponent<Image>() != null;
    }

    private bool HasLegacyClip(string clipName)
    {
        if (string.IsNullOrEmpty(clipName) || legacyAnimation == null)
        {
            return false;
        }

        return legacyAnimation[clipName] != null || legacyAnimation.GetClip(clipName) != null;
    }

    private bool ShouldUseAnimator(string triggerName)
    {
        return animator != null && HasTriggerParameter(triggerName);
    }

    private Image FindAnimationImage()
    {
        if (animationTarget != null)
        {
            Image foundImage = animationTarget.GetComponent<Image>();
            if (foundImage != null)
            {
                return foundImage;
            }
        }

        if (!string.IsNullOrEmpty(animationTargetName))
        {
            GameObject targetObject = GameObject.Find(animationTargetName);
            if (targetObject != null)
            {
                Image foundImage = targetObject.GetComponent<Image>();
                if (foundImage != null)
                {
                    return foundImage;
                }

                return targetObject.GetComponentInChildren<Image>();
            }
        }

        return null;
    }

    private void SetAnimationImageRaycasts(bool allowRaycasts)
    {
        if (animationImage != null)
        {
            animationImage.raycastTarget = allowRaycasts;
        }
    }

    private void SwitchToDefaultCamera()
    {
        if (swappableCameras == null || swappableCameras.Length == 0)
        {
            return;
        }

        int targetIndex = Mathf.Clamp(defaultCameraIndex, 0, swappableCameras.Length - 1);
        Camera targetCamera = swappableCameras[targetIndex];

        if (targetCamera != null && mainCamera != null)
        {
            mainCamera.transform.position = targetCamera.transform.position;
            mainCamera.transform.rotation = targetCamera.transform.rotation;
            Debug.Log($"Switched back to swappable camera {targetIndex + 1}");
        }
    }

    private bool HasTriggerParameter(string triggerName)
    {
        if (animator == null || string.IsNullOrEmpty(triggerName))
        {
            return false;
        }

        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            if (parameter.type == AnimatorControllerParameterType.Trigger && parameter.name == triggerName)
            {
                return true;
            }
        }

        return false;
    }

    private IEnumerator PlayAnimationOnce(string triggerName)
    {
        ResolveAnimationComponents();

        if (animator == null)
        {
            Debug.LogWarning("No Animator component found for camera animation.");
            isAnimating = false;
            yield break;
        }

        if (!HasTriggerParameter(triggerName))
        {
            Debug.LogWarning($"Animator trigger '{triggerName}' was not found on {animator.gameObject.name}.");
            isAnimating = false;
            yield break;
        }

        isAnimating = true;
        animator.SetTrigger(triggerName);

        yield return new WaitForSeconds(0.2f);
        isAnimating = false;
    }

    private IEnumerator PlayLegacyAnimationOnce(string clipName)
    {
        if (legacyAnimation == null)
        {
            isAnimating = false;
            yield break;
        }

        isAnimating = true;

        if (legacyAnimation[clipName] != null)
        {
            if (!HasVisibleImage(legacyAnimation.gameObject))
            {
                Debug.LogWarning($"Legacy animation clip '{clipName}' was found on {legacyAnimation.gameObject.name}, but that object has no Image component, so the sprite animation will not be visible.");
            }
            else
            {
                Debug.Log($"Playing legacy clip '{clipName}' on {legacyAnimation.gameObject.name}");
            }

            legacyAnimation.Stop();
            legacyAnimation.Play(clipName);
            yield return new WaitForSeconds(0.2f);
        }
        else
        {
            AnimationClip clip = legacyAnimation.GetClip(clipName);
            if (clip != null)
            {
                if (!HasVisibleImage(legacyAnimation.gameObject))
                {
                    Debug.LogWarning($"Legacy animation clip '{clipName}' was found on {legacyAnimation.gameObject.name}, but that object has no Image component, so the sprite animation will not be visible.");
                }
                else
                {
                    Debug.Log($"Playing legacy clip '{clipName}' on {legacyAnimation.gameObject.name}");
                }

                legacyAnimation.Stop();
                legacyAnimation.Play(clipName);
                yield return new WaitForSeconds(0.2f);
            }
            else
            {
                Debug.LogWarning($"Legacy animation clip '{clipName}' was not found.");
                isAnimating = false;
                yield break;
            }
        }

        isAnimating = false;
    }

    private void ForceCloseCameras()
    {
        Debug.Log("ForceCloseCameras called!");
        
        isAnimating = false;
        isCameraOpen = false;
        
        // Hide camera buttons immediately
        foreach (GameObject button in cameraButtons)
        {
            if (button != null)
            {
                button.SetActive(false);
            }
        }
        
        // Force switch back to main camera immediately - no animation delay
        if (mainCamera != null)
        {
            mainCamera.transform.position = originalCameraPosition;
            mainCamera.transform.rotation = originalCameraRotation;
            
            // Disable all other cameras to ensure main camera is the only one rendering
            Camera[] allCameras = FindObjectsOfType<Camera>();
            foreach (Camera cam in allCameras)
            {
                if (cam != mainCamera)
                {
                    cam.enabled = false;
                }
                else
                {
                    cam.enabled = true;
                }
            }
        }
    }
}
