using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Rotates a target around a separate pivot while the camera monitor is up,
/// then reverses the rotation when the monitor is closed.
/// </summary>
public class RotateAroundPivotOnCamera : MonoBehaviour
{
    public enum CameraStateSource
    {
        CameraSystem,
        CameraManager,
        Automatic,
        Button
    }

    [Header("Objects")]
    [Tooltip("The object that will move around the pivot.")]
    public Transform target;

    [Tooltip("The object to rotate around. The pivot's position is used as the orbit center.")]
    public Transform pivot;

    [Header("Rotation")]
    [Tooltip("Total rotation in degrees around the X axis while open. Negative values rotate the other direction.")]
    public float xRotationAmount = 0f;

    [Tooltip("Total rotation in degrees around the Y axis while open. Negative values rotate the other direction.")]
    public float yRotationAmount = 0f;

    [Tooltip("Total rotation in degrees around the Z axis while open. Negative values rotate the other direction.")]
    public float zRotationAmount = 90f;

    [Min(0f)]
    [Tooltip("Time in seconds used to open or close the rotation.")]
    public float rotationTime = 0.5f;

    [Header("Camera State")]
    [Tooltip("Use Button to control the rotation from a UI Button instead of the camera state.")]
    public CameraStateSource cameraStateSource = CameraStateSource.Automatic;

    [Header("Button Control")]
    [Tooltip("Optional. Assign a UI Button to automatically toggle the rotation when it is pressed.")]
    public Button toggleButton;

    private bool _lastCameraState;
    private bool _isOpen;
    private float _rotationProgress;
    private Vector3 _closedPosition;
    private Quaternion _closedRotation;
    private Coroutine _rotationRoutine;

    private void Awake()
    {
        if (target == null)
            target = transform;
    }

    private void Start()
    {
        if (target != null)
        {
            _closedPosition = target.position;
            _closedRotation = target.rotation;

            if (pivot != null)
                ApplyRotationProgress();
        }

        if (IsButtonControlled())
            return;

        _lastCameraState = IsCameraUp();
    }

    private void OnEnable()
    {
        MainCameraButton.OnCameraToggleRequested += OnMainCameraToggleRequested;

        if (toggleButton != null)
            toggleButton.onClick.AddListener(ToggleRotation);
    }

    private void OnDisable()
    {
        MainCameraButton.OnCameraToggleRequested -= OnMainCameraToggleRequested;

        if (toggleButton != null)
            toggleButton.onClick.RemoveListener(ToggleRotation);
    }

    private void OnMainCameraToggleRequested(bool opening)
    {
        if (IsButtonControlled())
            return;

        SetRotation(opening);
    }

    private void Update()
    {
        if (IsButtonControlled())
            return;

        bool cameraIsUp = IsCameraUp();
        if (cameraIsUp == _lastCameraState)
            return;

        _lastCameraState = cameraIsUp;
        SetRotation(cameraIsUp);
    }

    /// <summary>Toggles between the open and closed rotation states.</summary>
    public void ToggleRotation()
    {
        SetRotation(!_isOpen);
    }

    /// <summary>Rotates to the open position.</summary>
    public void OpenRotation()
    {
        SetRotation(true);
    }

    /// <summary>Rotates back to the closed position.</summary>
    public void CloseRotation()
    {
        SetRotation(false);
    }

    private void SetRotation(bool open)
    {
        float targetProgress = open ? 1f : 0f;
        if (_isOpen == open && Mathf.Approximately(_rotationProgress, targetProgress))
        {
            ApplyRotationProgress();
            return;
        }

        _isOpen = open;
        StartRotation(targetProgress);
    }

    private bool IsButtonControlled()
    {
        return cameraStateSource == CameraStateSource.Button || toggleButton != null;
    }

    private bool IsCameraUp()
    {
        switch (cameraStateSource)
        {
            case CameraStateSource.Button:
                return false;

            case CameraStateSource.CameraSystem:
                return CameraSystem.Instance != null && CameraSystem.Instance.IsWatchingCameras;

            case CameraStateSource.CameraManager:
                return CameraManager.Instance != null && CameraManager.Instance.IsWatchingCameras;

            case CameraStateSource.Automatic:
            default:
                bool cameraSystemIsUp = CameraSystem.Instance != null && CameraSystem.Instance.IsWatchingCameras;
                bool cameraManagerIsUp = CameraManager.Instance != null && CameraManager.Instance.IsWatchingCameras;
                return cameraSystemIsUp || cameraManagerIsUp;
        }
    }

    private void StartRotation(float targetProgress)
    {
        if (target == null || pivot == null)
        {
            Debug.LogWarning("RotateAroundPivotOnCamera needs both a target and a pivot.", this);
            return;
        }

        if (_rotationRoutine != null)
            StopCoroutine(_rotationRoutine);

        _rotationRoutine = StartCoroutine(RotateTo(targetProgress));
    }

    private IEnumerator RotateTo(float targetProgress)
    {
        if (rotationTime <= 0f)
        {
            _rotationProgress = targetProgress;
            ApplyRotationProgress();
            _rotationRoutine = null;
            yield break;
        }

        while (!Mathf.Approximately(_rotationProgress, targetProgress))
        {
            float changePerSecond = 1f / rotationTime;
            _rotationProgress = Mathf.MoveTowards(
                _rotationProgress,
                targetProgress,
                changePerSecond * Time.deltaTime);
            ApplyRotationProgress();
            yield return null;
        }

        _rotationProgress = targetProgress;
        ApplyRotationProgress();
        _rotationRoutine = null;
    }

    private void ApplyRotationProgress()
    {
        Vector3 rotationAmount = new Vector3(xRotationAmount, yRotationAmount, zRotationAmount);
        Quaternion orbitRotation = Quaternion.Euler(rotationAmount * _rotationProgress);
        Vector3 offsetFromPivot = _closedPosition - pivot.position;

        target.position = pivot.position + orbitRotation * offsetFromPivot;
        target.rotation = orbitRotation * _closedRotation;
    }
}