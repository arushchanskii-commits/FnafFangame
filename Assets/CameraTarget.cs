using UnityEngine;

public class CameraTarget : MonoBehaviour
{
    [System.Serializable]
    public class MoveEntry
    {
        public Transform objectToMove;
        public Vector3 targetPosition;
    }

    [Tooltip("Assign the objects that should move when this target is pressed, along with their target positions.")]
    public MoveEntry[] moveGroup;
    
    [Header("Camera Integration (optional)")]
    [Tooltip("If true, pressing this target will switch the FNAF camera to 'viewIndex'.")]
    public bool switchCameraOnPress = false;
    [Tooltip("Index of the view in FnafCameraSystem to switch to when pressed. -1 to disable.")]
    public int viewIndex = -1;
    [Tooltip("If true, pressing this target will also toggle the visibility and functionality of the assigned buttons.")]
    public bool toggleButtonsOnPress = false;
    [Tooltip("Optional specific buttons to toggle when this target is pressed. If empty, CameraManager.buttonsToToggle will be used.")]
    public GameObject[] toggleButtonTargets;
    [Tooltip("If true, the target button itself will be preserved while other buttons are toggled.")]
    public bool excludePressedButtonFromToggle = true;

    public void OnPress()
    {
        MoveGroup();
        TrySwitchCamera();
        TryToggleButtons();
    }

    private void OnMouseDown()
    {
        OnPress();
    }

    private void Update()
    {
        if (Input.touchCount == 0 || Camera.main == null)
        {
            return;
        }

        Touch touch = Input.GetTouch(0);
        if (touch.phase != TouchPhase.Ended)
        {
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(touch.position);
        if (Physics.Raycast(ray, out RaycastHit hit3D) && hit3D.collider != null && hit3D.collider.gameObject == gameObject)
        {
            MoveGroup();
            return;
        }

        RaycastHit2D hit2D = Physics2D.GetRayIntersection(ray);
        if (hit2D.collider != null && hit2D.collider.gameObject == gameObject)
        {
            MoveGroup();
            TrySwitchCamera();
            TryToggleButtons();
        }
    }

    private void MoveGroup()
    {
        if (moveGroup == null || moveGroup.Length == 0)
        {
            Debug.LogWarning($"{name}: No objects assigned to move.");
            return;
        }

        foreach (MoveEntry entry in moveGroup)
        {
            if (entry == null || entry.objectToMove == null)
            {
                Debug.LogWarning($"{name}: One of the move entries is missing an object reference.");
                continue;
            }

            entry.objectToMove.position = entry.targetPosition;
        }
    }

    private void TrySwitchCamera()
    {
        if (!switchCameraOnPress) return;
        if (viewIndex < 0) return;
        if (FnafCameraSystem.Instance == null)
        {
            Debug.LogWarning("CameraTarget: No FnafCameraSystem instance found in scene.");
            return;
        }

        FnafCameraSystem.Instance.SetView(viewIndex);
    }

    private void TryToggleButtons()
    {
        if (!toggleButtonsOnPress) return;
        if (CameraManager.Instance == null)
        {
            Debug.LogWarning("CameraTarget: No CameraManager instance found in scene.");
            return;
        }

        if (toggleButtonTargets != null && toggleButtonTargets.Length > 0)
        {
            CameraManager.Instance.ToggleCustomButtons(toggleButtonTargets);
        }
        else
        {
            GameObject pressedObject = gameObject;
            if (excludePressedButtonFromToggle)
            {
                CameraManager.Instance.ToggleOtherButtons(pressedObject);
            }
            else
            {
                CameraManager.Instance.ToggleButtons();
            }
        }
    }
}
