using UnityEngine;

public class ConstantRotation : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float rotationSpeed = 100f;
    public Vector3 rotationAxis = Vector3.up;
    
    [Header("Power Outage Settings")]
    public float slowdownSpeed = 5f;
    public bool stopOnPowerOutage = true;
    
    private float currentSpeed;
    private float targetSpeed;
    private bool powerOutage = false;

    private void Start()
    {
        currentSpeed = rotationSpeed;
        targetSpeed = rotationSpeed;
    }

    private void Update()
    {
        // Check for power outage
        if (stopOnPowerOutage && PowerManager.Instance != null && !PowerManager.Instance.CanUseDevice() && !powerOutage)
        {
            Debug.Log("Power outage! Slowing down rotation");
            powerOutage = true;
            targetSpeed = 0f;
        }
        else if (stopOnPowerOutage && PowerManager.Instance != null && PowerManager.Instance.CanUseDevice() && powerOutage)
        {
            Debug.Log("Power restored! Speeding up rotation");
            powerOutage = false;
            targetSpeed = rotationSpeed;
        }
        
        // Smoothly interpolate current speed to target speed
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, slowdownSpeed * Time.deltaTime);
        
        // Rotate using current speed
        transform.Rotate(rotationAxis, currentSpeed * Time.deltaTime);
    }
}
