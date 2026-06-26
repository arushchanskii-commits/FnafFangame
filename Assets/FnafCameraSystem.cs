using UnityEngine;

public class FnafCameraSystem : MonoBehaviour
{
    [System.Serializable]
    public class CameraView
    {
        public string name;
        [Tooltip("If set, camera will focus on this transform position. Otherwise use 'position'.")]
        public Transform focusTarget;
        [Tooltip("Fallback world position for the camera view (used when focusTarget is null).")]
        public Vector3 position;
        public float orthographicSize = 5f;
        [Tooltip("Optional objects assigned to this view (for inspector organization).")]
        public GameObject[] assignedTargets;
    }

    public static FnafCameraSystem Instance { get; private set; }

    public Camera mainCamera;
    public CameraView[] views;
    public float moveSpeed = 8f;

    private int currentIndex = -1;
    private Vector3 targetPosition;
    private float targetSize;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }

    private void Start()
    {
        if (mainCamera == null) mainCamera = Camera.main;
        
        // Disable all other cameras in all loaded scenes to avoid conflicts
        Camera[] allCameras = FindObjectsOfType<Camera>();
        foreach (Camera cam in allCameras)
        {
            if (cam != mainCamera)
            {
                cam.enabled = false;
                Debug.Log($"FnafCameraSystem: Disabled conflicting camera '{cam.name}'");
            }
        }
        
        if (views != null && views.Length > 0)
        {
            SetView(0);
        }
    }

    private void Update()
    {
    }

    public void SetView(int index)
    {
        if (views == null || index < 0 || index >= views.Length)
        {
            Debug.LogWarning($"FnafCameraSystem: invalid view index {index}");
            return;
        }

        if (mainCamera == null) return;

        currentIndex = index;
        CameraView v = views[index];
        Vector3 newPos = (v.focusTarget != null)
            ? new Vector3(v.focusTarget.position.x, v.focusTarget.position.y, mainCamera.transform.position.z)
            : new Vector3(v.position.x, v.position.y, mainCamera.transform.position.z);

        mainCamera.transform.position = newPos;
        mainCamera.orthographicSize = v.orthographicSize;
    }

    public void SetViewByAssignedObject(GameObject obj)
    {
        if (obj == null || views == null) return;
        for (int i = 0; i < views.Length; i++)
        {
            var assigned = views[i].assignedTargets;
            if (assigned == null) continue;
            for (int j = 0; j < assigned.Length; j++)
            {
                if (assigned[j] == obj)
                {
                    SetView(i);
                    return;
                }
            }
        }
    }
}

