using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class PowerLight
{
    public Light lightSource;
    public bool isOn = false;
    public float originalIntensity = 1f;
    
    public void Enable()
    {
        if (lightSource != null)
        {
            lightSource.enabled = true;
            isOn = true;
        }
    }
    
    public void Disable()
    {
        if (lightSource != null)
        {
            originalIntensity = lightSource.intensity;
            lightSource.enabled = false;
            isOn = false;
        }
    }
}
