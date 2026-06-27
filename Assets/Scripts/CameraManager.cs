using UnityEngine;
using UnityEngine.UI;

public class CameraManager : MonoBehaviour
{
    [Tooltip("The main camera that the player spawns in with.")]
    public Camera mainCamera;

    [Tooltip("Assign the 3 swappable cameras here.")]
    public Camera[] swappableCameras = new Camera[3];

    [Tooltip("Optional UI Buttons that correspond to the swappable cameras. Used by ToggleSwappableButtons().")]
    public Button[] swappableButtons;

    [Tooltip("Optional button that toggles the visibility and interactability of the assigned toggle buttons.")]
    public Button toggleButton;
    [Tooltip("Buttons to hide/show when the toggle button is pressed. If empty, swappableButtons will be used.")]
    public Button[] buttonsToToggle;

    [Tooltip("Index of the camera to start on. -1 for main camera, 0-2 for swappable cameras.")]
    public int defaultCameraIndex = -1;

    [Tooltip("When buttons are toggled back on, switch to this camera. -1 = main camera.")]
    public int cameraIndexOnToggle = -1;

    private Camera activeCamera;

    public static CameraManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        if (defaultCameraIndex == -1)
        {
            SwitchToMainCamera();
        }
        else if (defaultCameraIndex >= 0 && defaultCameraIndex < swappableCameras.Length)
        {
            SwitchToCamera(swappableCameras[defaultCameraIndex]);
        }
    }

    public void SwitchToMainCamera()
    {
        if (mainCamera == null)
        {
            Debug.LogWarning("CameraManager: Main camera is not assigned.");
            return;
        }

        DisableAllCameras();
        mainCamera.enabled = true;
        activeCamera = mainCamera;
    }

    public void SwitchToCamera(int index)
    {
        if (index < 0 || index >= swappableCameras.Length)
        {
            Debug.LogWarning($"CameraManager: Invalid camera index {index}.");
            return;
        }

        if (swappableCameras[index] == null)
        {
            Debug.LogWarning($"CameraManager: Camera at index {index} is not assigned.");
            return;
        }

        DisableAllCameras();
        swappableCameras[index].enabled = true;
        activeCamera = swappableCameras[index];
    }

    public void SwitchToCamera(Camera targetCamera)
    {
        if (targetCamera == null)
        {
            Debug.LogWarning("CameraManager: Cannot switch to a null camera.");
            return;
        }

        DisableAllCameras();
        targetCamera.enabled = true;
        activeCamera = targetCamera;
    }

    private void DisableAllCameras()
    {
        if (mainCamera != null) mainCamera.enabled = false;
        for (int i = 0; i < swappableCameras.Length; i++)
        {
            if (swappableCameras[i] != null) swappableCameras[i].enabled = false;
        }
    }

    public void SwitchToTarget(GameObject target)
    {
        if (target == null)
        {
            Debug.LogWarning("CameraManager: Cannot switch to a null target.");
            return;
        }

        Camera targetCamera = target.GetComponentInChildren<Camera>();
        if (targetCamera == null)
        {
            Debug.LogWarning($"CameraManager: Target object '{target.name}' does not contain a Camera component.");
            return;
        }

        SwitchToCamera(targetCamera);
    }

    public Button[] GetButtonsToToggle()
    {
        if (buttonsToToggle != null && buttonsToToggle.Length > 0)
            return buttonsToToggle;

        return swappableButtons;
    }

    public void ToggleButtons()
    {
        ToggleButtons(GetButtonsToToggle());
    }

    public void ToggleSwappableButtons()
    {
        ToggleButtons();
    }

    private void OnButtonsToggledOn()
    {
        if (cameraIndexOnToggle == -1)
        {
            SwitchToMainCamera();
        }
        else
        {
            SwitchToCamera(cameraIndexOnToggle);
        }
    }

    public void ToggleOtherButtons(GameObject pressedButtonObj)
    {
        if (pressedButtonObj == null)
        {
            Debug.Log("CameraManager: pressed button object is null.");
            return;
        }

        Button[] buttons = GetButtonsToToggle();
        if (buttons == null || buttons.Length == 0)
        {
            Debug.Log("CameraManager: No buttons assigned to toggle.");
            return;
        }

        bool newState = !AreOtherButtonsVisible(buttons, pressedButtonObj);
        foreach (var b in buttons)
        {
            if (b == null || b.gameObject == pressedButtonObj) continue;
            b.gameObject.SetActive(newState);
            b.interactable = newState;
        }
    }

    private bool AreOtherButtonsVisible(Button[] buttons, GameObject pressedButtonObj)
    {
        foreach (var b in buttons)
        {
            if (b == null || b.gameObject == pressedButtonObj) continue;
            if (b.gameObject.activeSelf) return true;
        }
        return false;
    }

    public void ToggleCustomButtons(GameObject[] targets)
    {
        if (targets == null || targets.Length == 0)
        {
            Debug.Log("CameraManager: No custom button targets assigned to toggle.");
            return;
        }

        bool newState = !targets[0].activeSelf;
        foreach (var obj in targets)
        {
            if (obj == null) continue;
            obj.SetActive(newState);

            Button button = obj.GetComponent<Button>();
            if (button != null)
            {
                button.interactable = newState;
            }
        }
    }

    private void ToggleButtons(Button[] buttons)
    {
        if (buttons == null || buttons.Length == 0)
        {
            Debug.Log("CameraManager: No buttons assigned to toggle.");
            return;
        }

        bool newState = !buttons[0].gameObject.activeSelf;
        foreach (var b in buttons)
        {
            if (b == null) continue;
            b.gameObject.SetActive(newState);
            b.interactable = newState;
        }

        if (newState)
        {
            OnButtonsToggledOn();
        }
    }
}
