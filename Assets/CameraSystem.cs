// Assets/Scripts/Player/CameraSystem.cs
using UnityEngine;
using UnityEngine.UI;

public class CameraSystem : MonoBehaviour
{
    public static CameraSystem Instance { get; private set; }

    [Header("Kameras")]
    public int ActiveCamera { get; private set; } = -1; // -1 = Monitor unten

    [Header("UI")]
    public GameObject cameraMonitor;
    public Image      cameraFeed;
    public Sprite[]   cameraSprites; // Index = Kamera-ID

    private bool _monitorUp;

    // ----------------------------------------------------------------

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    // Aufgerufen durch UI-Buttons
    public void ToggleMonitor()
    {
        _monitorUp = !_monitorUp;
        cameraMonitor.SetActive(_monitorUp);

        if (!_monitorUp)
            ActiveCamera = -1;
    }

    public void SwitchToCamera(int camIndex)
    {
        if (!_monitorUp) return;

        ActiveCamera = camIndex;

        if (camIndex >= 0 && camIndex < cameraSprites.Length)
            cameraFeed.sprite = cameraSprites[camIndex];
    }
}