using UnityEngine;

public class FNAFCameraMove : MonoBehaviour
{
    [Header("Camera Rotation")]
    public float rotateSpeed = 5f;
    public float edgeThreshold = 0.1f;
    
    [Header("Camera Rotations")]
    public Camera mainCamera;
    public Quaternion leftRotation;
    public Quaternion rightRotation;
    public Quaternion centerRotation;
    
    [Header("Generator Reference")]
    public PowerCharger powerCharger;
    
    private Quaternion currentRotation;
    private Quaternion originalRotation;

    private void Start()
    {
        if (centerRotation == Quaternion.identity)
            centerRotation = transform.rotation;
        
        originalRotation = transform.rotation;
        currentRotation = centerRotation;
    }

    private void Update()
    {
        // Disable looking around while generator is charging
        if (powerCharger != null && powerCharger.isCharging)
        {
            // Lock camera to original position while charging
            transform.rotation = Quaternion.Slerp(transform.rotation, originalRotation, rotateSpeed * 2f * Time.deltaTime);
            return;
        }
        
        Vector3 mousePos = Input.mousePosition;
        float screenWidth = Screen.width;
        
        if (mousePos.x < screenWidth * edgeThreshold)
        {
            currentRotation = leftRotation;
        }
        else if (mousePos.x > screenWidth * (1f - edgeThreshold))
        {
            currentRotation = rightRotation;
        }
        else
        {
            currentRotation = centerRotation;
        }
        
        transform.rotation = Quaternion.Slerp(transform.rotation, currentRotation, rotateSpeed * Time.deltaTime);
    }
}
