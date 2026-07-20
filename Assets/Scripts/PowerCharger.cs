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
    
    [Header("Visual Feedback")]
    public GameObject chargingIndicator;
    public Light chargingLight;
    
    private AudioSource audioSource;
    private Camera mainCam;
    private float currentZoom = 0f;
    public bool isCharging = false;
    private float chargeTimer = 0f;
    private float originalFOVValue;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private bool powerOutageOccurred = false;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
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
        // Check if power outage has occurred - disable generator after power outage
        if (PowerManager.Instance != null && !PowerManager.Instance.CanUseDevice() && !powerOutageOccurred)
        {
            powerOutageOccurred = true;
            Debug.Log("Power outage occurred! Generator is now disabled.");
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
        
        Debug.Log("Charging started!");
        
        if (chargingSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(chargingSound, soundVolume);
        }
        
        if (chargingIndicator != null)
        {
            chargingIndicator.SetActive(true);
        }
        
        if (chargingLight != null)
        {
            chargingLight.enabled = true;
        }
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
            
            Debug.Log($"Power restored: {PowerManager.Instance.currentPower:F1}%");
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
        
        Debug.Log("Charging stopped!");
        
        if (chargingIndicator != null)
        {
            chargingIndicator.SetActive(false);
        }
        
        if (chargingLight != null)
        {
            chargingLight.enabled = false;
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
