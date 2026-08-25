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
    
    [Header("Glitch Effect")]
    public Sprite glitchSprite;
    public float glitchDuration = 0.5f;
    public float glitchInterval = 0.1f;

    private bool _monitorUp;
    private Coroutine _glitchCoroutine;

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
    
    public void TriggerGlitchEffect(int camIndex)
    {
        if (!_monitorUp || ActiveCamera != camIndex) return;
        
        if (_glitchCoroutine != null)
        {
            StopCoroutine(_glitchCoroutine);
        }
        
        _glitchCoroutine = StartCoroutine(GlitchCoroutine());
    }
    
    private System.Collections.IEnumerator GlitchCoroutine()
    {
        float elapsed = 0f;
        Sprite originalSprite = cameraFeed.sprite;
        
        while (elapsed < glitchDuration)
        {
            cameraFeed.sprite = glitchSprite;
            yield return new WaitForSeconds(glitchInterval);
            
            if (elapsed + glitchInterval < glitchDuration)
            {
                cameraFeed.sprite = originalSprite;
                yield return new WaitForSeconds(glitchInterval);
            }
            
            elapsed += glitchInterval * 2f;
        }
        
        cameraFeed.sprite = originalSprite;
        _glitchCoroutine = null;
    }
}