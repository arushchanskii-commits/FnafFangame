using UnityEngine;

public class GeneratorRotation : MonoBehaviour
{
    [Header("Rotation Settings")]
    public Transform rotationTarget;
    public Vector3 rotationAxis = Vector3.up;
    public float rotationSpeed = 50f;
    public bool useLocalSpace = true;
    
    [Header("Generator Reference")]
    public PowerCharger powerCharger;
    
    [Header("Starting Rotation")]
    public float startingYRotation = 0f;
    
    private Vector3 originalPosition;
    private Vector3 orbitCenter;
    private float orbitAngle = 0f;
    private float selfRotationAngle = 0f;
    private Vector3 orbitOffset;
    private Quaternion originalRotation;

    private void Start()
    {
        if (rotationTarget == null)
        {
            Debug.LogWarning("No rotation target assigned! Using parent or self.");
            rotationTarget = transform.parent;
            if (rotationTarget == null)
            {
                rotationTarget = transform;
            }
        }
        
        orbitCenter = rotationTarget.position;
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        orbitAngle = 0f;
        selfRotationAngle = 0f;
        
        // Calculate the offset from the center
        orbitOffset = transform.position - orbitCenter;
        
        Debug.Log($"Starting position: {originalPosition}");
        Debug.Log($"Orbit center: {orbitCenter}");
        Debug.Log($"Orbit offset: {orbitOffset}");
        Debug.Log($"Starting rotation: {originalRotation.eulerAngles}");
    }

    private void Update()
    {
        // Check if generator is charging
        bool isCharging = false;
        
        if (powerCharger != null)
        {
            isCharging = powerCharger.isCharging;
        }
        
        if (isCharging)
        {
            // Increment orbit angle
            orbitAngle += rotationSpeed * Time.deltaTime;
            selfRotationAngle += rotationSpeed * Time.deltaTime;
            
            // Calculate orbit position: rotate the offset around the center using the assigned rotationAxis
            Vector3 rotatedOffset = Quaternion.Euler(rotationAxis * orbitAngle) * orbitOffset;
            Vector3 newPosition = orbitCenter + rotatedOffset;
            
            transform.position = newPosition;
            
            // Self-rotate the object around its own axis, preserving original rotation
            transform.rotation = originalRotation * Quaternion.Euler(rotationAxis * selfRotationAngle);
            
            Debug.Log($"Orbit angle: {orbitAngle:F1}, Position: {transform.position}, Rotation: {transform.rotation.eulerAngles}");
        }
        else
        {
            // Reset to original position and rotation when not charging
            orbitAngle = 0f;
            selfRotationAngle = 0f;
            transform.position = originalPosition;
            transform.rotation = originalRotation;
            
            Debug.Log($"Reset to: {transform.position}, rotation: {transform.rotation.eulerAngles}");
        }
    }
}
