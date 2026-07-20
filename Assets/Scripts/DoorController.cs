using UnityEngine;

public class DoorController : MonoBehaviour
{
    [Header("Door Settings")]
    public Transform door;
    public float openSpeed = 2f;
    public float closeSpeed = 10f;
    
    [Header("Door Positions")]
    public float openHeight = 5f;
    public float closedHeight = 0f;
    
    [Header("Target Object")]
    public GameObject targetObject;
    public Light targetLight;
    public float lightIntensity = 2f;
    
    [Header("Audio")]
    public AudioClip openSound;
    public AudioClip closeSound;
    public AudioClip disabledClickSound;
    public float soundVolume = 1f;
    
    private Vector3 closedPosition;
    private Vector3 openPosition;
    private bool isDoorOpen = true;
    private float originalLightIntensity;
    private AudioSource audioSource;
    private bool isPowerRegistered = false;
    private bool powerOutage = false;

    private void Start()
    {
        if (door == null)
        {
            door = transform;
        }
        
        closedPosition = door.position;
        openPosition = new Vector3(closedPosition.x, closedPosition.y + openHeight, closedPosition.z);
        
        if (targetLight != null)
        {
            originalLightIntensity = targetLight.intensity;
        }
        
        audioSource = GetComponent<AudioSource>();
        
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        // Door starts open, so not consuming power yet
        isPowerRegistered = false;
    }

    private void OnMouseDown()
    {
        if (powerOutage)
        {
            Debug.Log("Cannot close door - power outage!");
            if (disabledClickSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(disabledClickSound, soundVolume);
            }
            return;
        }
        
        if (targetObject != null && targetObject.activeSelf)
        {
            if (isDoorOpen)
            {
                isDoorOpen = false;
                
                if (closeSound != null)
                {
                    audioSource.PlayOneShot(closeSound, soundVolume);
                }
                
                if (targetLight != null)
                {
                    targetLight.intensity = originalLightIntensity;
                }
                
                // Register door as closed (power consuming)
                if (!isPowerRegistered && PowerManager.Instance.CanUseDevice())
                {
                    isPowerRegistered = true;
                    PowerManager.Instance.RegisterDevice(PowerManager.DeviceType.Door);
                }
            }
            else
            {
                isDoorOpen = true;
                
                if (openSound != null)
                {
                    audioSource.PlayOneShot(openSound, soundVolume);
                }
                
                if (targetLight != null)
                {
                    targetLight.intensity = lightIntensity;
                }
                
                // Unregister door from power consumption
                if (isPowerRegistered)
                {
                    isPowerRegistered = false;
                    PowerManager.Instance.UnregisterDevice(PowerManager.DeviceType.Door);
                }
            }
        }
    }

    private void Update()
    {
        if (targetObject == null || !targetObject.activeSelf)
            return;
        
        // Check if power went out
        if (PowerManager.Instance != null && !PowerManager.Instance.CanUseDevice() && !powerOutage)
        {
            Debug.Log("Power outage! Door opening automatically");
            powerOutage = true;
            
            // Force door open if it was closed
            if (!isDoorOpen)
            {
                isDoorOpen = true;
                if (isPowerRegistered)
                {
                    isPowerRegistered = false;
                    PowerManager.Instance.UnregisterDevice(PowerManager.DeviceType.Door);
                }
            }
        }
        
        // During power outage, force door open and disable closing
        if (powerOutage)
        {
            door.position = Vector3.Lerp(door.position, openPosition, closeSpeed * Time.deltaTime);
            if (targetLight != null)
            {
                targetLight.intensity = originalLightIntensity;
            }
            return;
        }
        
        if (isDoorOpen)
        {
            door.position = Vector3.Lerp(door.position, openPosition, openSpeed * Time.deltaTime);
            
            if (targetLight != null)
            {
                targetLight.intensity = lightIntensity;
            }
        }
        else
        {
            door.position = Vector3.Lerp(door.position, closedPosition, closeSpeed * Time.deltaTime);
            
            if (targetLight != null)
            {
                targetLight.intensity = originalLightIntensity;
            }
        }
    }
}
