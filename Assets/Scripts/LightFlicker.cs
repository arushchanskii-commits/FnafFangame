using UnityEngine;
using System.Collections.Generic;

public class LightFlicker : MonoBehaviour
{
    [Header("Flicker Settings")]
    public Light targetLight;
    public List<Light> secondaryLights = new List<Light>();
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
    private List<float> secondaryLightOriginalIntensities = new List<float>();
    private List<bool> secondaryLightWasOnes = new List<bool>();

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
        
        // Store secondary light original states if assigned
        secondaryLightOriginalIntensities.Clear();
        secondaryLightWasOnes.Clear();
        
        foreach (Light secLight in secondaryLights)
        {
            if (secLight != null)
            {
                secondaryLightOriginalIntensities.Add(secLight.intensity);
                secondaryLightWasOnes.Add(secLight.enabled);
            }
            else
            {
                secondaryLightOriginalIntensities.Add(0f);
                secondaryLightWasOnes.Add(false);
            }
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
            // Restore secondary lights
            for (int i = 0; i < secondaryLights.Count && i < secondaryLightOriginalIntensities.Count && i < secondaryLightWasOnes.Count; i++)
            {
                if (secondaryLights[i] != null)
                {
                    secondaryLights[i].enabled = secondaryLightWasOnes[i];
                    secondaryLights[i].intensity = secondaryLightOriginalIntensities[i];
                }
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
        
        // Toggle secondary lights in sync
        for (int i = 0; i < secondaryLights.Count; i++)
        {
            if (secondaryLights[i] != null)
            {
                secondaryLights[i].enabled = isLightOn;
                if (isLightOn && i < secondaryLightOriginalIntensities.Count)
                {
                    secondaryLights[i].intensity = secondaryLightOriginalIntensities[i];
                }
            }
        }
    }
}
