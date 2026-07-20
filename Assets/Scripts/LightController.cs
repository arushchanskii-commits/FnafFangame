using UnityEngine;

public class LightController : MonoBehaviour
{
    [Header("Light Settings")]
    public Light lightSource;
    public bool isOn = false;
    
    private void Start()
    {
        if (lightSource == null)
        {
            lightSource = GetComponent<Light>();
        }
        
        if (lightSource != null)
        {
            lightSource.enabled = false;
        }
    }

    public void TurnOn()
    {
        if (!isOn && PowerManager.Instance.CanUseDevice())
        {
            isOn = true;
            if (lightSource != null)
            {
                lightSource.enabled = true;
                Debug.Log($"Light turned ON - consuming power");
            }
            PowerManager.Instance.RegisterDevice(PowerManager.DeviceType.Light);
        }
        else if (!PowerManager.Instance.CanUseDevice())
        {
            Debug.LogWarning("Cannot turn on light - no power!");
        }
    }
    
    public void TurnOff()
    {
        if (isOn)
        {
            isOn = false;
            if (lightSource != null)
            {
                lightSource.enabled = false;
                Debug.Log($"Light turned OFF");
            }
            PowerManager.Instance.UnregisterDevice(PowerManager.DeviceType.Light);
        }
    }
    
    public void Toggle()
    {
        if (isOn)
        {
            TurnOff();
        }
        else
        {
            TurnOn();
        }
    }
    
    public bool IsPowered()
    {
        return PowerManager.Instance.CanUseDevice();
    }
}
