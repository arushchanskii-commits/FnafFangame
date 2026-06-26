using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimatronicResetter : MonoBehaviour
{
    [Header("Target")]
    [Tooltip("The AnimatronicAI to reset.")]
    public AnimatronicAI animatronic;

    [Header("Reset Mode")]
    public ResetMode resetMode = ResetMode.FirstRoom;

    [Tooltip("Only used when Reset Mode is set to SpecificRoom. Zero-based index into the animatronic's roomPath list.")]
    public int specificRoomIndex = 0;

    [Header("Trigger")]
    [Tooltip("Call ResetAnimatronic() from this UnityEvent, a Button's OnClick, or any other script.")]
    public UnityEvent onResetRequested;

    // ──────────────────────────────────────────────────────────────

    /// <summary>
    /// Resets the animatronic according to the chosen ResetMode.
    /// Wire this to a Button's OnClick or call it from any script.
    /// </summary>
    public void ResetAnimatronic()
    {
        if (animatronic == null)
        {
            Debug.LogWarning("[AnimatronicResetter] No animatronic assigned.");
            return;
        }

        List<Room> path = animatronic.roomPath;

        if (path == null || path.Count == 0)
        {
            Debug.LogWarning("[AnimatronicResetter] The animatronic's roomPath is empty.");
            return;
        }

        int targetIndex = resetMode switch
        {
            ResetMode.FirstRoom    => 0,
            ResetMode.LastRoom     => path.Count - 1,
            ResetMode.RandomRoom   => Random.Range(0, path.Count),
            ResetMode.SpecificRoom => Mathf.Clamp(specificRoomIndex, 0, path.Count - 1),
            _                      => 0
        };

        animatronic.ResetToRoom(targetIndex);

        Debug.Log($"[AnimatronicResetter] Reset {animatronic.animatronicName} to room [{targetIndex}]: {path[targetIndex].roomName}");

        onResetRequested?.Invoke();
    }
}

public enum ResetMode
{
    FirstRoom,    // Always back to index 0
    LastRoom,     // Last room in the path
    RandomRoom,   // Random room in the path
    SpecificRoom  // Exact index you choose in the Inspector
}
