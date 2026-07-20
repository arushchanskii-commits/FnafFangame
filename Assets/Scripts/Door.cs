using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("Door Settings")]
    public bool isClosed = false;
    
    private void Start()
    {
        // Door starts open by default
        isClosed = false;
    }

    public void Close()
    {
        if (!isClosed && PowerManager.Instance.CanUseDevice())
        {
            isClosed = true;
            PowerManager.Instance.RegisterDevice(PowerManager.DeviceType.Door);
            Debug.Log($"Door closed - power consumption increased");
        }
    }
    
    public void Open()
    {
        if (isClosed)
        {
            isClosed = false;
            PowerManager.Instance.UnregisterDevice(PowerManager.DeviceType.Door);
            Debug.Log($"Door opened - power consumption decreased");
        }
    }
    
    public void Toggle()
    {
        if (isClosed)
        {
            Open();
        }
        else
        {
            Close();
        }
    }
    
    public bool IsPowered()
    {
        return PowerManager.Instance.CanUseDevice();
    }
    
    public bool IsClosed()
    {
        return isClosed;
    }
}
