using UnityEngine;

public class TargetLightController : MonoBehaviour
{
    [Header("Light Settings")]
    public Light targetLight;
    public float lightIntensity = 2f;
    
    [Header("Target Object")]
    [Tooltip("Object that disappears when the light is on")]
    public GameObject lightTargetObject;
    
    [Header("Audio")]
    public AudioClip clickSound;
    public AudioClip disabledClickSound;
    public float soundVolume = 1f;
    
    private float originalIntensity;
    private bool isPowerRegistered = false;
    private AudioSource audioSource;

    private void Update()
    {
        // If power runs out while light is on, turn it off
        if (isPowerRegistered && PowerManager.Instance != null && !PowerManager.Instance.CanUseDevice())
        {
            Debug.Log("Power out! Light turning off");
            targetLight.enabled = false;
            isPowerRegistered = false;
            PowerManager.Instance.UnregisterDevice(PowerManager.DeviceType.Light);
        }
    }

    private void Start()
    {
        Debug.Log($"TargetLightController started on {gameObject.name}");
        
        if (targetLight == null)
        {
            targetLight = GetComponent<Light>();
            if (targetLight == null)
            {
                Debug.LogError("No Light component found on this object or assigned!");
                return;
            }
        }
        
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        originalIntensity = targetLight.intensity;
        isPowerRegistered = false;
        
        Debug.Log($"Original light intensity: {originalIntensity}");
        Debug.Log($"Light range: {targetLight.range}");
        Debug.Log($"Light color: {targetLight.color}");
        Debug.Log($"Light type: {targetLight.type}");
        Debug.Log($"Light enabled: {targetLight.enabled}");
        
        if (PowerManager.Instance == null)
        {
            Debug.LogError("PowerManager instance is null! Make sure PowerManager is in the scene.");
        }
        else
        {
            Debug.Log("PowerManager found!");
        }
    }

    private void OnMouseDown()
    {
        Debug.Log("OnMouseDown called on light!");
        Debug.Log($"targetLight is null: {targetLight == null}");
        Debug.Log($"CanUseDevice: {PowerManager.Instance?.CanUseDevice()}");
        
        if (PowerManager.Instance == null)
        {
            Debug.LogError("PowerManager is null, turning on light anyway!");
            if (targetLight != null)
            {
                targetLight.enabled = true;
                targetLight.intensity = lightIntensity;
                if (clickSound != null && audioSource != null)
                {
                    audioSource.PlayOneShot(clickSound, soundVolume);
                }
                Debug.Log($"Light intensity set to {lightIntensity}, enabled: {targetLight.enabled}");
            }
            return;
        }
        
        if (!PowerManager.Instance.CanUseDevice())
        {
            Debug.LogWarning("Cannot turn on light - no power!");
            if (disabledClickSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(disabledClickSound, soundVolume);
            }
            return;
        }
        
        if (targetLight != null)
        {
            targetLight.enabled = true;
            targetLight.intensity = lightIntensity;
            
            if (lightTargetObject != null)
            {
                lightTargetObject.SetActive(false);
            }
            
            Debug.Log($"Light enabled: {targetLight.enabled}");
            Debug.Log($"Light intensity: {targetLight.intensity}");
            Debug.Log($"Light range: {targetLight.range}");
            Debug.Log($"Light color: {targetLight.color}");
            Debug.Log($"Light ON - consuming power, intensity: {lightIntensity}");
            
            if (clickSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(clickSound, soundVolume);
            }
            
            if (!isPowerRegistered)
            {
                isPowerRegistered = true;
                PowerManager.Instance.RegisterDevice(PowerManager.DeviceType.Light);
            }
        }
        else
        {
            Debug.LogError("targetLight is null! Check the assignment in Inspector.");
        }
    }

    private void OnMouseUp()
    {
        Debug.Log("OnMouseUp called on light!");
        
        if (targetLight != null)
        {
            targetLight.enabled = false;
            targetLight.intensity = originalIntensity;
            
            if (lightTargetObject != null)
            {
                lightTargetObject.SetActive(true);
            }
            
            Debug.Log($"Light disabled, intensity: {originalIntensity}");
            
            if (isPowerRegistered)
            {
                isPowerRegistered = false;
                PowerManager.Instance.UnregisterDevice(PowerManager.DeviceType.Light);
            }
        }
    }
}
