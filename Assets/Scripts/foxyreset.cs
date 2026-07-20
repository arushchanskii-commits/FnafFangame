using UnityEngine;

public class DoorClick : MonoBehaviour
{
    [Header("Target")]
    [Tooltip("The animatronic to check and reset.")]
    public AnimatronicAI animatronic;

    [Tooltip("The room the animatronic must be in for the reset to trigger.")]
    public Room triggerRoom;
    
    [Header("Light")]
    [Tooltip("Light that turns on when pressed.")]
    public Light pressLight;
    public float lightIntensity = 2f;
    
    [Header("Power")]
    [Tooltip("Power consumed each time the button is pressed.")]
    public float powerCost = 5f;
    
    [Header("Sound")]
    [Tooltip("Plays when the reset triggers. Needs an AudioSource on this GameObject.")]
    public AudioClip resetSound;
    public AudioClip errorSound;
    public AudioClip flashSound;
    public float soundVolume = 1f;

    private AudioSource _audioSource;
    private float originalLightIntensity;
    private bool isPressed = false;

    // ──────────────────────────────────────────────────────────────

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        
        if (pressLight == null)
        {
            pressLight = GetComponent<Light>();
            if (pressLight == null)
            {
                Debug.LogWarning("[DoorClick] No Light component found. Assign one in Inspector or add to this GameObject.");
            }
        }
        
        if (pressLight != null)
        {
            originalLightIntensity = pressLight.intensity;
            pressLight.enabled = false;
            Debug.Log($"[DoorClick] Light initialized with intensity: {originalLightIntensity}");
        }
    }

    private void OnMouseDown()
    {
        if (animatronic == null)
        {
            Debug.LogWarning("[DoorClick] No animatronic assigned.");
            return;
        }

        // Turn on light
        if (pressLight != null)
        {
            pressLight.enabled = true;
            pressLight.intensity = lightIntensity;
            isPressed = true;
        }
        
        // Consume power
        if (PowerManager.Instance != null && PowerManager.Instance.CanUseDevice())
        {
            PowerManager.Instance.currentPower -= powerCost;
            if (PowerManager.Instance.currentPower < 0) PowerManager.Instance.currentPower = 0;
            PowerManager.Instance.UpdatePowerUIPublic();
            Debug.Log($"[DoorClick] Power used: {powerCost}. Remaining: {PowerManager.Instance.currentPower:F1}%");
        }
        else
        {
            Debug.LogWarning("[DoorClick] No power available!");
            if (errorSound != null && _audioSource != null)
            {
                _audioSource.PlayOneShot(errorSound, soundVolume);
            }
        }
        
        // Play flash sound regardless of reset
        PlaySound(flashSound, soundVolume);
        
        // Check if Foxy is in Element 3 Room (3rd room in path, index 2)
        bool isElement3Room = false;
        if (animatronic.roomPath != null && animatronic.roomPath.Count > 2)
        {
            Room element3Room = animatronic.roomPath[2]; // Index 2 = 3rd room
            isElement3Room = (animatronic.CurrentRoom == element3Room);
            Debug.Log($"[DoorClick] Checking if Foxy is in Element 3 Room. Current: {animatronic.CurrentRoom.roomName}, Target: {element3Room.roomName}");
        }
        
        if (!isElement3Room)
        {
            // Light up and use power, but don't reset
            Debug.Log($"[DoorClick] Foxy is NOT in Element 3 Room - light on, power used, no reset");
        }
        else
        {
            // Foxy is in Element 3 Room - do the reset
            Debug.Log($"[DoorClick] {animatronic.animatronicName} is in Element 3 Room – resetting to first room.");
            animatronic.ResetToRoom(0);
            PlayResetSound();
        }
        
        // Turn off light after a short delay
        Invoke(nameof(TurnOffLight), 0.5f);
    }
    
    private void TurnOffLight()
    {
        if (pressLight != null)
        {
            pressLight.enabled = false;
            pressLight.intensity = originalLightIntensity;
            isPressed = false;
            Debug.Log("[DoorClick] Light turned off");
        }
    }

    private void PlayResetSound()
    {
        PlaySound(resetSound, soundVolume);
    }

    private void PlaySound(AudioClip clip, float volume)
    {
        if (clip == null) return;

        if (_audioSource != null)
        {
            _audioSource.PlayOneShot(clip, volume);
        }
        else
        {
            // Fallback: plays at world position without needing an AudioSource component
            AudioSource.PlayClipAtPoint(clip, transform.position, volume);
        }
    }
}
