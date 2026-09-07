using UnityEngine;

public class CameraLight : MonoBehaviour
{
    [SerializeField] private Camera swappableCamera;

    private Light targetLight;

    private void Awake()
    {
        targetLight = GetComponent<Light>();
    }

    private void OnEnable()
    {
        UpdateLightState();
    }

    private void Update()
    {
        UpdateLightState();
    }

    private void UpdateLightState()
    {
        if (targetLight == null)
        {
            return;
        }

        targetLight.enabled = swappableCamera != null && swappableCamera.isActiveAndEnabled;
    }
}
