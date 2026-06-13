using UnityEngine;
using UnityEngine.SceneManagement;

public class hotwireStarting : MonoBehaviour
{
    private const string DefaultWiresPrefabName = "Wires";

    [Tooltip("The wires prefab to spawn when this object is clicked.")]
    public GameObject wiresPrefab;

    [Tooltip("Camera used to convert screen cursor position into world space. If left empty, Camera.main will be used.")]
    public Camera targetCamera;

    [Tooltip("World-space Z distance from the camera to place the spawned object.")]
    public float worldZDistance = 10f;

    private void OnMouseDown()
    {
        if (wiresPrefab == null)
        {
            wiresPrefab = Resources.Load<GameObject>(DefaultWiresPrefabName);
            if (wiresPrefab == null)
            {
                Debug.LogWarning($"hotwireStarting: wiresPrefab is not assigned and Resources/{DefaultWiresPrefabName} could not be loaded.");
                return;
            }
        }

        var cameraToUse = targetCamera != null ? targetCamera : Camera.main;
        if (cameraToUse == null)
        {
            Debug.LogWarning("hotwireStarting: No camera found to position the spawned object. Assign a camera or use Camera.main.");
            return;
        }

        Vector3 spawnPosition = cameraToUse.transform.position;
        spawnPosition.x -= 1f;
        spawnPosition.z += worldZDistance;

        var spawnedObject = Instantiate(wiresPrefab, spawnPosition, Quaternion.identity);
        SceneManager.MoveGameObjectToScene(spawnedObject, gameObject.scene);
    }
}
