using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class FnafCameraExampleCreator
{
    [MenuItem("Tools/Create FNAF Camera Example v2")]
    public static void CreateExample()
    {
        // CameraManager
        GameObject managerGO = new GameObject("CameraManager");
        var cameraManager = managerGO.AddComponent<CameraManager>();

        // Create Main Camera
        GameObject mainCamGO = new GameObject("MainCamera");
        var mainCam = mainCamGO.AddComponent<Camera>();
        mainCam.orthographic = true;
        mainCam.transform.position = new Vector3(0f, 0f, -10f);
        cameraManager.mainCamera = mainCam;
        mainCam.enabled = true;

        // Create 3 Swappable Cameras
        Camera[] swappableCameras = new Camera[3];
        for (int i = 0; i < 3; i++)
        {
            GameObject camGO = new GameObject($"SwappableCamera_{i}");
            swappableCameras[i] = camGO.AddComponent<Camera>();
            swappableCameras[i].orthographic = true;
            swappableCameras[i].transform.position = new Vector3(i * 8f, 0f, -10f);
            swappableCameras[i].enabled = false;
        }

        cameraManager.swappableCameras = swappableCameras;
        cameraManager.defaultCameraIndex = -1; // start with main camera

        // Create UI Canvas with 4 buttons
        GameObject canvasGO = new GameObject("CameraUI");
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        // Main Camera Button
        Button mainButton = CreateButton(canvasGO, "MainCamButton", "Main", 0, cameraManager, -1);

        // 3 Swappable Camera Buttons
        Button[] swappableBtns = new Button[3];
        for (int i = 0; i < 3; i++)
        {
            swappableBtns[i] = CreateButton(canvasGO, $"CameraButton_{i}", $"Cam {i}", i + 1, cameraManager, i);
        }

        // register buttons with camera manager so main button can toggle them
        cameraManager.swappableButtons = swappableBtns;
        if (mainButton != null)
        {
            mainButton.onClick.AddListener(cameraManager.ToggleSwappableButtons);
        }

        Selection.activeGameObject = managerGO;

        Debug.Log("FNAF Camera example created: CameraManager with 1 main camera, 3 swappable cameras, and 4 UI buttons.");
    }

    private static Button CreateButton(GameObject canvasGO, string btnName, string btnText, int rowIndex, CameraManager cameraManager, int cameraIndex)
    {
        GameObject btnGO = new GameObject(btnName);
        btnGO.transform.SetParent(canvasGO.transform);
        var img = btnGO.AddComponent<Image>();
        img.color = Color.white;
        var btn = btnGO.AddComponent<Button>();

        RectTransform rt = btnGO.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(160, 30);
        rt.anchoredPosition = new Vector2(100, -30 - rowIndex * 40);

        // add text
        GameObject textGO = new GameObject("Text");
        textGO.transform.SetParent(btnGO.transform);
        var text = textGO.AddComponent<Text>();
        text.text = btnText;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.black;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        RectTransform trt = textGO.GetComponent<RectTransform>();
        trt.anchorMin = Vector2.zero;
        trt.anchorMax = Vector2.one;
        trt.offsetMin = Vector2.zero;
        trt.offsetMax = Vector2.zero;

        // Wire button listener
        if (cameraIndex == -1)
        {
            btn.onClick.AddListener(cameraManager.SwitchToMainCamera);
        }
        else
        {
            btn.onClick.AddListener(() => cameraManager.SwitchToCamera(cameraIndex));
        }

        return btn;
    }
}
