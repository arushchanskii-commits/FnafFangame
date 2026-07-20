using UnityEngine;

public class SetAmbientLighting : MonoBehaviour
{
    [Header("Ambient Lighting Settings")]
    public Color ambientColor = Color.black;
    public float ambientIntensity = 1f;
    
    private void Start()
    {
        // Force ambient lighting to dark
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = ambientColor;
        RenderSettings.ambientIntensity = ambientIntensity;
        
        Debug.Log($"Ambient lighting set to: {ambientColor}, Intensity: {ambientIntensity}");
    }
}
