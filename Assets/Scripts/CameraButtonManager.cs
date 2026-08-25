using UnityEngine;

public class CameraButtonManager : MonoBehaviour
{
    [Header("Camera Button Reference")]
    public GameObject mainCameraButton;
    
    [Header("Generator Reference")]
    public PowerCharger powerCharger;
    
    private void Update()
    {
        if (mainCameraButton == null)
        {
            return;
        }
        
        bool shouldHide = false;
        
        // Hide during power outage (power at 0%)
        if (PowerManager.Instance != null && PowerManager.Instance.currentPower <= 0)
        {
            shouldHide = true;
        }
        
        // Hide while generator is charging
        if (powerCharger != null && powerCharger.isCharging)
        {
            shouldHide = true;
        }
        
        // Toggle visibility
        if (shouldHide && mainCameraButton.activeSelf)
        {
            mainCameraButton.SetActive(false);
        }
        else if (!shouldHide && !mainCameraButton.activeSelf)
        {
            mainCameraButton.SetActive(true);
        }
    }
}
