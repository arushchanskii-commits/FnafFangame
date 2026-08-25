using UnityEngine;

public class JumpscareController : MonoBehaviour
{
    [Header("Jumpscare Objects")]
    public GameObject freddyJumpscare;
    public GameObject bonnieJumpscare;
    public GameObject chicaJumpscare;
    public GameObject foxyJumpscare;
    
    [Header("Audio")]
    public AudioClip jumpscareSound;
    
    [Header("Screen Shake")]
    public float shakeDuration = 0.5f;
    public float shakeIntensity = 0.3f;
    
    [Header("Fade")]
    public ScreenFade screenFade;
    
    [Header("Jumpscare Light")]
    [Tooltip("Light to flash during jumpscare")]
    public Light jumpscareLight;
    public float lightFlashDuration = 0.5f;
    public float lightFlashIntensity = 2f;
    
    [Header("Reset Camera")]
    [Tooltip("Camera to reset to on jumpscare (usually your office camera)")]
    public Camera resetCamera;
    
    private Camera mainCamera;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private AudioSource audioSource;
    private bool isJumpscareActive = false;
    private FNAFCameraMove cameraMoveScript;
    private Light originalLightState;
    
    public bool IsJumpscareActive => isJumpscareActive;
    
    private void Start()
    {
        // Use resetCamera if assigned, otherwise use Camera.main
        if (resetCamera != null)
        {
            mainCamera = resetCamera;
        }
        else
        {
            mainCamera = Camera.main;
        }
        
        if (mainCamera != null)
        {
            originalPosition = mainCamera.transform.position;
            originalRotation = mainCamera.transform.rotation;
        }
        
        cameraMoveScript = FindObjectOfType<FNAFCameraMove>();
        
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        // Store original light state if jumpscare light is assigned
        if (jumpscareLight != null)
        {
            originalLightState = jumpscareLight;
        }
        
        // Hide all jumpscare objects initially
        HideAllJumpscares();
    }
    
    public void TriggerJumpscare(AnimatronicAI killer)
    {
        if (isJumpscareActive) return;
        isJumpscareActive = true;
        
        // Disable camera movement script immediately
        if (cameraMoveScript != null)
        {
            cameraMoveScript.enabled = false;
        }
        
        // Close cameras immediately (like power outage)
        CloseCamerasImmediately();
        
        // Show the correct jumpscare object
        ShowJumpscare(killer.animatronicName);
        
        // Play scream sound
        if (jumpscareSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(jumpscareSound);
        }
        
        // Flash the light during jumpscare
        if (jumpscareLight != null)
        {
            StartCoroutine(FlashLight(lightFlashDuration, lightFlashIntensity));
        }
        
        // Start screen shake
        StartCoroutine(ScreenShake(shakeDuration, shakeIntensity));
        
        // Wait 2 seconds then show death screen
        Invoke(nameof(ShowDeathScreen), 2f);
    }
    
    private void CloseCamerasImmediately()
    {
        // Find MainCameraButton and force close
        MainCameraButton cameraButton = FindObjectOfType<MainCameraButton>();
        if (cameraButton != null)
        {
            cameraButton.isCameraOpen = false;
            cameraButton.isAnimating = false;
            
            // Hide camera buttons
            if (cameraButton.cameraButtons != null)
            {
                foreach (GameObject button in cameraButton.cameraButtons)
                {
                    if (button != null) button.SetActive(false);
                }
            }
            
            // Get the office camera position from MainCameraButton
            if (mainCamera != null)
            {
                mainCamera.transform.position = cameraButton.originalCameraPosition;
                mainCamera.transform.rotation = cameraButton.originalCameraRotation;
            }
        }
        
        // Force reset main camera position immediately
        if (mainCamera != null)
        {
            mainCamera.transform.position = originalPosition;
            mainCamera.transform.rotation = originalRotation;
            
            // Disable all other cameras to ensure only main camera renders
            Camera[] allCameras = FindObjectsOfType<Camera>();
            foreach (Camera cam in allCameras)
            {
                if (cam != mainCamera)
                {
                    cam.enabled = false;
                }
            }
            
            // Ensure main camera is enabled
            mainCamera.enabled = true;
        }
    }
    
    private void ShowJumpscare(string animatronicName)
    {
        switch (animatronicName.ToLower())
        {
            case "freddy":
                if (freddyJumpscare != null) freddyJumpscare.SetActive(true);
                break;
            case "bonnie":
                if (bonnieJumpscare != null) bonnieJumpscare.SetActive(true);
                break;
            case "chica":
                if (chicaJumpscare != null) chicaJumpscare.SetActive(true);
                break;
            case "foxy":
                if (foxyJumpscare != null) foxyJumpscare.SetActive(true);
                break;
            default:
                Debug.LogWarning($"No jumpscare object assigned for {animatronicName}");
                break;
        }
    }
    
    private void HideAllJumpscares()
    {
        if (freddyJumpscare != null) freddyJumpscare.SetActive(false);
        if (bonnieJumpscare != null) bonnieJumpscare.SetActive(false);
        if (chicaJumpscare != null) chicaJumpscare.SetActive(false);
        if (foxyJumpscare != null) foxyJumpscare.SetActive(false);
    }
    
    private System.Collections.IEnumerator FlashLight(float duration, float targetIntensity)
    {
        if (jumpscareLight == null) yield break;
        
        float originalIntensity = jumpscareLight.intensity;
        float elapsed = 0f;
        
        // Turn light on and increase intensity
        jumpscareLight.enabled = true;
        jumpscareLight.intensity = targetIntensity;
        
        // Keep light on for a brief moment
        while (elapsed < duration * 0.3f)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        // Turn light off
        jumpscareLight.enabled = false;
        jumpscareLight.intensity = originalIntensity;
    }
    
    private System.Collections.IEnumerator ScreenShake(float duration, float intensity)
    {
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * intensity;
            float y = Random.Range(-1f, 1f) * intensity;
            
            if (mainCamera != null)
            {
                mainCamera.transform.localPosition = new Vector3(
                    mainCamera.transform.localPosition.x + x,
                    mainCamera.transform.localPosition.y + y,
                    mainCamera.transform.localPosition.z
                );
            }
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        if (mainCamera != null)
        {
            mainCamera.transform.localPosition = originalPosition;
        }
    }
    
    private void ShowDeathScreen()
    {
        // Fade to black first
        if (screenFade != null)
        {
            screenFade.FadeToBlack(1f);
            
            // Wait for fade to complete then load death screen
            Invoke(nameof(LoadDeathScreen), 1f);
        }
        else
        {
            // No fade, load immediately
            Death.GlobalDeath("DeathScreen");
        }
    }
    
    private void LoadDeathScreen()
    {
        Death.GlobalDeath("DeathScreen");
    }
}
