using UnityEngine;

public class newMouseFollow : MonoBehaviour
{
    [Tooltip("Camera used to convert screen cursor position into world space. If left empty, Camera.main will be used.")]
    public Camera targetCamera;

    [Tooltip("Optional tag to find the camera at runtime if no direct reference is available.")]
    public string cameraTag = "HotWireCamera";

    [Tooltip("World-space Z distance from the camera to place the follower object.")]
    public float worldZDistance = 10f;

    [Tooltip("Speed at which the object follows the cursor. Set to 0 for instant movement.")]
    public float followSpeed = 10f;

    private void Start()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        if (targetCamera == null && !string.IsNullOrEmpty(cameraTag))
        {
            var cameraObject = GameObject.FindGameObjectWithTag(cameraTag);
            if (cameraObject != null)
            {
                targetCamera = cameraObject.GetComponent<Camera>();
            }
        }

        if (targetCamera == null)
        {
            Debug.LogWarning("newMouseFollow: No camera found. Please assign a Camera in the Inspector or add the main camera tag.");
        }
    }

    private void Update()
    {
        if (targetCamera == null)
        {
            return;
        }

        Vector3 mouseScreenPosition = Input.mousePosition;
        mouseScreenPosition.z = targetCamera.WorldToScreenPoint(transform.position).z;
        Vector3 targetPosition = targetCamera.ScreenToWorldPoint(mouseScreenPosition);

        if (followSpeed > 0f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
        }
        else
        {
            transform.position = targetPosition;
        }
    }
}
