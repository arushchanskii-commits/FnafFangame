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
    
    private AudioSource audioSource;
    private Vector3 originalCameraPosition;
    private Quaternion originalCameraRotation;
    private bool isCameraOpen = false;
    private bool isAnimating = false;

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
            Debug.Log("MainCamera auto-assigned to Camera.main");
        }
        
        if (mainCamera != null)
        {
            originalCameraPosition = mainCamera.transform.position;
            originalCameraRotation = mainCamera.transform.rotation;
            Debug.Log($"Original camera position saved: {originalCameraPosition}");
            Debug.Log($"Original camera rotation saved: {originalCameraRotation}");
        }
        else
        {
            Debug.LogError("No main camera found in scene!");
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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ToggleCameraButtons();
        }
        
        // Check for power outage and close cameras automatically
        if (PowerManager.Instance != null && !PowerManager.Instance.CanUseDevice() && isCameraOpen && !isAnimating)
        {
            Debug.Log("Power outage detected! isCameraOpen: " + isCameraOpen + ", isAnimating: " + isAnimating);
            ForceCloseCameras();
        }
        else if (PowerManager.Instance == null)
        {
            Debug.LogWarning("PowerManager is null! Power outage detection won't work.");
        }
        
        // Hide button on power outage or while generator is charging
        bool shouldHide = false;
        
        if (PowerManager.Instance != null && !PowerManager.Instance.CanUseDevice())
        {
            shouldHide = true;
        }
        
        if (powerCharger != null && powerCharger.isCharging)
        {
            shouldHide = true;
        }
        
        if (shouldHide && gameObject.activeSelf)
        {
            gameObject.SetActive(false);
            Debug.Log("MainCameraButton hidden (power outage or charging)");
        }
    }

    public void ToggleCameraButtons()
    {
        if (isAnimating)
        {
            return;
        }

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
                if (animator != null)
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
            
            if (!isAnimating)
            {
                if (animator != null)
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
            return false;
        }

        foreach (Animator candidateAnimator in Resources.FindObjectsOfTypeAll<Animator>())
        {
            if (candidateAnimator != null && candidateAnimator.gameObject != null)
            {
                string candidateName = candidateAnimator.gameObject.name;
                if (candidateName == animationTargetName || candidateName.Contains(animationTargetName))
                {
                    animator = candidateAnimator;
                    animationTarget = candidateAnimator.gameObject;
                    return true;
                }
            }
        }

        foreach (Animation candidateAnimation in Resources.FindObjectsOfTypeAll<Animation>())
        {
            if (candidateAnimation != null && candidateAnimation.gameObject != null)
            {
                string candidateName = candidateAnimation.gameObject.name;
                if (candidateName == animationTargetName || candidateName.Contains(animationTargetName))
                {
                    legacyAnimation = candidateAnimation;
                    animationTarget = candidateAnimation.gameObject;
                    return true;
                }
            }
        }

        return false;
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

            foundImage = animationTarget.GetComponentInChildren<Image>();
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
            yield break;
        }

        if (!HasTriggerParameter(triggerName))
        {
            Debug.LogWarning($"Animator trigger '{triggerName}' was not found on {animator.gameObject.name}.");
            yield break;
        }

        isAnimating = true;
        animator.SetTrigger(triggerName);

        yield return null;

        float duration = 0.2f;
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.length > 0f)
        {
            duration = stateInfo.length;
        }

        yield return new WaitForSeconds(duration);
        isAnimating = false;
    }

    private IEnumerator PlayLegacyAnimationOnce(string clipName)
    {
        if (legacyAnimation == null)
        {
            yield break;
        }

        isAnimating = true;

        if (legacyAnimation[clipName] != null)
        {
            legacyAnimation.Stop();
            legacyAnimation.Play(clipName);
            yield return new WaitForSeconds(legacyAnimation[clipName].length);
        }
        else
        {
            Debug.LogWarning($"Legacy animation clip '{clipName}' was not found.");
            yield return null;
        }

        isAnimating = false;
    }

    private void ForceCloseCameras()
    {
        Debug.Log("ForceCloseCameras called!");
        
        if (isAnimating)
        {
            Debug.Log("Cannot force close - already animating");
            return;
        }
        
        Debug.Log("Setting isCameraOpen to false");
        isCameraOpen = false;
        
        // Hide camera buttons
        foreach (GameObject button in cameraButtons)
        {
            if (button != null)
            {
                button.SetActive(false);
                Debug.Log("Camera button hidden");
            }
        }
        
        // Play close animation
        SetAnimationImageRaycasts(false);
        
        if (animator != null)
        {
            Debug.Log("Force closing: Animator found, triggering close animation");
            StartCoroutine(PlayAnimationOnce(closeTrigger));
        }
        else if (legacyAnimation != null)
        {
            Debug.Log("Force closing: Legacy Animation found, playing close clip");
            StartCoroutine(PlayLegacyAnimationOnce(closeAnimationClipName));
        }
        else
        {
            Debug.LogWarning("No animator or legacy animation found!");
        }
        
        // Force switch back to main camera immediately
        if (mainCamera != null)
        {
            Vector3 oldPos = mainCamera.transform.position;
            Quaternion oldRot = mainCamera.transform.rotation;
            
            Debug.Log($"BEFORE: Main camera at {oldPos}, rotation {oldRot.eulerAngles}");
            Debug.Log($"AFTER: Setting to position {originalCameraPosition}, rotation {originalCameraRotation.eulerAngles}");
            
            mainCamera.transform.position = originalCameraPosition;
            mainCamera.transform.rotation = originalCameraRotation;
            
            Vector3 newPos = mainCamera.transform.position;
            Quaternion newRot = mainCamera.transform.rotation;
            
            Debug.Log($"VERIFIED: Main camera now at {newPos}, rotation {newRot.eulerAngles}");
            
            // Disable all other cameras to ensure main camera is the only one rendering
            Camera[] allCameras = FindObjectsOfType<Camera>();
            Debug.Log($"Total cameras in scene: {allCameras.Length}");
            foreach (Camera cam in allCameras)
            {
                if (cam != mainCamera)
                {
                    cam.enabled = false;
                    Debug.Log($"Disabled camera: {cam.name}");
                }
                else
                {
                    cam.enabled = true;
                    Debug.Log($"Enabled main camera: {cam.name}");
                }
            }
            
            Debug.Log("Camera position/rotation reset - should be back in office view!");
        }
        else
        {
            Debug.LogError("Main camera is null! Cannot switch back to office view.");
        }
    }
}
