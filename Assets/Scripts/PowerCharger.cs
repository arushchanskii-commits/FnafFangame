using UnityEngine;

public class PowerCharger : MonoBehaviour
{
    [Header("Charging Settings")]
    public float powerRestoredPerSecond = 10f;
    
    [Header("Camera Settings")]
    public Camera mainCamera;
    public Vector3 zoomPosition;
    public Quaternion zoomRotation;
    public float zoomFOV = 40f;
    public float originalFOV = 60f;
    public float zoomSpeed = 5f;
    
    [Header("Audio")]
    public AudioClip chargingSound;
    public float soundVolume = 0.5f;
    public bool loopChargingSound = true;
    [Tooltip("Seconds before the sound can replay after each play")]
    public float soundCooldown = 0.5f;
    
    [Header("Visual Feedback")]
    public GameObject chargingIndicator;
    public Light chargingLight;
    
    [Header("Camera Reference")]
    public MainCameraButton cameraButton;
    
    private AudioSource audioSource;
    private Camera mainCam;
    private float currentZoom = 0f;
    public bool isCharging = false;
    private float chargeTimer = 0f;
    private float originalFOVValue;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private bool powerOutageOccurred = false;
    private float soundCooldownTimer = 0f;
    private bool soundIsPlaying = false;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.volume = soundVolume;
        audioSource.Stop();
        soundCooldownTimer = 0f;
        soundIsPlaying = false;

        if (chargingSound != null)
        {
            audioSource.clip = chargingSound;
        }
        
        mainCam = mainCamera;
        if (mainCam == null)
        {
            mainCam = Camera.main;
        }
        
        if (mainCam != null)
        {
            originalPosition = mainCam.transform.position;
            originalRotation = mainCam.transform.rotation;
            originalFOVValue = mainCam.fieldOfView;
        }
        
        if (chargingIndicator != null)
        {
            chargingIndicator.SetActive(false);
        }
    }

    private void Update()
    {
        // Check if cameras are open - disable generator
        if (cameraButton != null && cameraButton.isCameraOpen)
        {
            if (isCharging)
            {
                StopCharging();
            }
            return;
        }
        
        // Check if power outage has occurred - disable generator after power outage
        if (PowerManager.Instance != null && !PowerManager.Instance.CanUseDevice() && !powerOutageOccurred)
        {
            powerOutageOccurred = true;
        }
        
        // Don't allow charging after power outage
        if (powerOutageOccurred)
        {
            if (isCharging)
            {
                StopCharging();
            }
            return;
        }
        
        // Check if pressing E key to charge
        if (Input.GetKey(KeyCode.E))
        {
            if (!isCharging)
            {
                StartCharging();
            }
            
            UpdateCharging();
        }
        else
        {
            if (isCharging)
            {
                StopCharging();
            }
        }
    }

    private void StartCharging()
    {
        isCharging = true;
        soundCooldownTimer = 0f;
        soundIsPlaying = false;
        
        if (chargingIndicator != null)
        {
            chargingIndicator.SetActive(true);
        }
        
        if (chargingLight != null)
        {
            chargingLight.enabled = true;
        }

        if (audioSource != null && chargingSound != null)
        {
            audioSource.clip = chargingSound;
            audioSource.loop = false;
            audioSource.volume = soundVolume;
            PlayChargingSound();
        }
    }

    private void PlayChargingSound()
    {
        if (audioSource == null || chargingSound == null)
        {
            return;
        }

        audioSource.PlayOneShot(chargingSound, soundVolume);
        soundIsPlaying = true;
        soundCooldownTimer = Mathf.Max(soundCooldown, 0.01f);
    }

    private void UpdateCharging()
    {
        // Restore power continuously while held
        if (PowerManager.Instance != null)
        {
            float powerToAdd = powerRestoredPerSecond * Time.deltaTime;
            float newPower = PowerManager.Instance.currentPower + powerToAdd;
            
            // Cap at max power
            if (newPower > PowerManager.Instance.maxPower)
            {
                newPower = PowerManager.Instance.maxPower;
            }
            
            PowerManager.Instance.currentPower = newPower;
            PowerManager.Instance.UpdatePowerUIPublic();
        }

        if (audioSource != null && chargingSound != null && isCharging)
        {
            if (soundCooldownTimer > 0f)
            {
                soundCooldownTimer -= Time.deltaTime;
            }

            if (soundCooldownTimer <= 0f)
            {
                PlayChargingSound();
            }
        }
        
        // Keep camera zoomed in while held
        if (mainCam != null)
        {
            // Full zoom
            mainCam.transform.position = Vector3.Lerp(mainCam.transform.position, zoomPosition, zoomSpeed * Time.deltaTime);
            mainCam.transform.rotation = Quaternion.Lerp(mainCam.transform.rotation, zoomRotation, zoomSpeed * Time.deltaTime);
            mainCam.fieldOfView = Mathf.Lerp(mainCam.fieldOfView, zoomFOV, zoomSpeed * Time.deltaTime);
        }
    }

    private void StopCharging()
    {
        isCharging = false;
        
        if (chargingIndicator != null)
        {
            chargingIndicator.SetActive(false);
        }
        
        if (chargingLight != null)
        {
            chargingLight.enabled = false;
        }

        if (audioSource != null)
        {
            audioSource.Stop();
            soundIsPlaying = false;
            soundCooldownTimer = 0f;
        }
        
        // Immediately return camera to original position
        if (mainCam != null)
        {
            mainCam.transform.position = originalPosition;
            mainCam.transform.rotation = originalRotation;
            mainCam.fieldOfView = originalFOVValue;
        }
    }

    private void OnMouseDown()
    {
        // Click the object directly to charge
        StartCharging();
    }

    private void OnMouseUp()
    {
        StopCharging();
    }

    private void OnMouseExit()
    {
        // Stop charging if mouse leaves the object
        if (isCharging)
        {
            StopCharging();
        }
    }
}
