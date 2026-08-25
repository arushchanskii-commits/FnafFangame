using UnityEngine;

[CreateAssetMenu(fileName = "NewDoorRoom", menuName = "FNAF/Door Room")]
public class DoorRoom : Room
{
    public enum DoorSide { Left, Right }
    
    [Header("Door Settings")]
    public DoorSide doorSide;
    
    [Header("Door State")]
    [Tooltip("Current door state - synced with DoorController")]
    public bool isDoorClosed = false;
    
    [Header("Reset Settings")]
    [Tooltip("Which path index to reset to when blocked (usually 0 for start)")]
    public int resetPathIndex = 0;
    
    [Header("Audio (Optional)")]
    public AudioClip knockSound;
    
    /// <summary>
    /// Called when animatronic tries to enter this door room.
    /// Returns true if animatronic can pass, false if blocked.
    /// </summary>
    public bool TryEnter(AnimatronicAI animatronic)
    {
        if (isDoorClosed)
        {
            Debug.Log($"[{animatronic.animatronicName}] blocked by {doorSide} door! Resetting to start.");
            animatronic.ResetToRoom(resetPathIndex);
            return false;
        }
        
        return true;
    }
    
    public void CloseDoor()
    {
        isDoorClosed = true;
    }
    
    public void OpenDoor()
    {
        isDoorClosed = false;
    }
}
