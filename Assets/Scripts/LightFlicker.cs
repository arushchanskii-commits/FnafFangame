using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    [Header("Flicker Settings")]
    public Light targetLight;
    public Light secondaryLight;
    public bool isFlickering = false;
    public bool isLightOn = true;
    
    [Header("Flicker Timing")]
    public float minTimeOn = 0.05f;
    public float maxTimeOn = 0.3f;
    public float minTimeOff = 0.1f;
    public float maxTimeOff = 1f;
    
    [Header("Original State")]
    public float originalIntensity = 1f;
    
    private float currentTimer = 0f;
    private float flickerInterval = 0f;
    private float secondaryLightOriginalIntensity = 0f;
    private bool secondaryLightWasOn = false;

    private void Start()
    {
        if (targetLight == null)
        {
            targetLight = GetComponent<Light>();
        }
        
        if (targetLight != null)
        {
            originalIntensity = targetLight.intensity;
            isLightOn = targetLight.enabled;
        }
        
        // Store secondary light original state if assigned
        if (secondaryLight != null)
        {
            secondaryLightOriginalIntensity = secondaryLight.intensity;
            secondaryLightWasOn = secondaryLight.enabled;
        }
    }

    private void Update()
    {
        // Only flicker when power is out
        if (PowerManager.Instance == null || PowerManager.Instance.CanUseDevice())
        {
            // Power is available - stop flickering
        if (isFlickering)
        {
            isFlickering = false;
            if (targetLight != null)
            {
                targetLight.enabled = true;
                targetLight.intensity = originalIntensity;
            }
            if (secondaryLight != null)
            {
                secondaryLight.enabled = secondaryLightWasOn;
                secondaryLight.intensity = secondaryLightOriginalIntensity;
            }
        }
            return;
        }
        
        // Power is out - start flickering
        if (!isFlickering)
        {
            isFlickering = true;
            currentTimer = 0f;
            SetRandomFlickerInterval();
        }
        
        currentTimer += Time.deltaTime;
        
        if (currentTimer >= flickerInterval)
        {
            currentTimer = 0f;
            ToggleLight();
            SetRandomFlickerInterval();
        }
    }
    
    private void SetRandomFlickerInterval()
    {
        if (!isLightOn)
        {
            flickerInterval = Random.Range(minTimeOff, maxTimeOff);
        }
        else
        {
            flickerInterval = Random.Range(minTimeOn, maxTimeOn);
        }
    }
    
    private void ToggleLight()
    {
        if (targetLight == null) return;
        
        isLightOn = !isLightOn;
        targetLight.enabled = isLightOn;
        
        if (isLightOn)
        {
            targetLight.intensity = originalIntensity;
        }
        
        // Toggle secondary light in sync
        if (secondaryLight != null)
        {
            secondaryLight.enabled = isLightOn;
            if (isLightOn)
            {
                secondaryLight.intensity = secondaryLightOriginalIntensity;
            }
        }
    }
}
