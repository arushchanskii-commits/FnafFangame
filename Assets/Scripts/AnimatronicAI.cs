using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatronicAI : MonoBehaviour
{
    [Header("Settings")]
    public string animatronicName = "Animatronic";
    [Range(0, 20)]
    public int aiScore = 5;

    [Tooltip("Rooms in order from start to goal. The animatronic walks this list top to bottom.")]
    public List<Room> roomPath = new();

    [Header("Freddy Door Paths")]
    [Tooltip("Used only when this animatronic is Freddy. Freddy starts on one of these paths and switches to the other when blocked.")]
    public List<Room> leftDoorPath = new();

    public List<Room> rightDoorPath = new();

    [Header("Timer")]
    [Tooltip("Seconds to wait after initialization before the first movement roll.")]
    public float initialDelay = 0f;

    [Tooltip("Seconds between each movement attempt.")]
    public float tickInterval = 1f;

    [Header("Room Visuals")]
    [Tooltip("One entry per room (same order as Room Path). Assign either a SpriteRenderer OR a GameObject – whichever is set will be shown/hidden. If both are set, both are toggled.")]
    public List<RoomVisual> roomVisuals = new();

    [Tooltip("Freddy visuals for the leftDoorPath, in the same order as that path.")]
    public List<RoomVisual> leftDoorVisuals = new();

    [Tooltip("Freddy visuals for the rightDoorPath, in the same order as that path.")]
    public List<RoomVisual> rightDoorVisuals = new();

    // ── Events ─────────────────────────────────────────────────────
    /// <summary>Fired when the animatronic reaches the last room (the office).</summary>
    public System.Action OnJumpscare;

    /// <summary>Fired whenever the animatronic successfully moves to a new room.</summary>
    public System.Action OnMoved;

    // ── Runtime state ──────────────────────────────────────────────
    public Room CurrentRoom { get; private set; }
    private int       _pathIndex   = 0;
    private bool      _active      = false;
    private Coroutine _aiCoroutine = null;
    private FreddyPath _freddyPath = FreddyPath.Left;

    private enum FreddyPath
    {
        Left,
        Right
    }

    // ──────────────────────────────────────────────────────────────

    private void Start()
    {
        Initialize(aiScore);
    }

    // ──────────────────────────────────────────────────────────────
    // Public API

    /// <summary>
    /// Resets the animatronic to the first room and starts the AI loop
    /// using the given score (0 = never moves, 20 = always moves).
    /// </summary>
    public void Initialize(int score)
    {
        if (IsFreddy)
            PickRandomFreddyPath();

        List<Room> path = GetCurrentPath();
        if (path == null || path.Count == 0)
        {
            Debug.LogWarning($"[{animatronicName}] has no configured movement path – AI will not move.");
            return;
        }

        aiScore     = Mathf.Clamp(score, 0, 20);
        _pathIndex  = 0;
        CurrentRoom = path[0];
        _active     = true;

        UpdateVisuals();

        Debug.Log($"[{animatronicName}] initialized with AI score {aiScore} in: {CurrentRoom.roomName}");

        if (_aiCoroutine != null)
            StopCoroutine(_aiCoroutine);

        _aiCoroutine = StartCoroutine(AiLoop());
    }

    /// <summary>Stops the AI loop and hides all visuals.</summary>
    public void Deactivate()
    {
        _active = false;

        if (_aiCoroutine != null)
        {
            StopCoroutine(_aiCoroutine);
            _aiCoroutine = null;
        }

        HideAllVisuals();
        Debug.Log($"[{animatronicName}] deactivated.");
    }

    /// <summary>
    /// Moves the animatronic to the given path index and restarts the AI loop.
    /// Called by AnimatronicResetter – you can also call it directly.
    /// </summary>
    public void ResetToRoom(int pathIndex)
    {
        if (IsFreddy && pathIndex == 0)
            PickRandomFreddyPath();

        List<Room> path = GetCurrentPath();
        if (path == null || path.Count == 0) return;

        _pathIndex  = Mathf.Clamp(pathIndex, 0, path.Count - 1);
        CurrentRoom = path[_pathIndex];
        _active     = true;

        UpdateVisuals();

        if (_aiCoroutine != null)
            StopCoroutine(_aiCoroutine);

        _aiCoroutine = StartCoroutine(AiLoop());

        Debug.Log($"[{animatronicName}] reset to: {CurrentRoom.roomName}");
    }

    // ──────────────────────────────────────────────────────────────
    // Internal loop

    private IEnumerator AiLoop()
    {
        if (initialDelay > 0f)
            yield return new WaitForSeconds(initialDelay);

        while (_active)
        {
            yield return new WaitForSeconds(tickInterval);
            TryMove();
        }
    }

    private void TryMove()
    {
        List<Room> path = GetCurrentPath();
        if (path == null || path.Count == 0)
            return;

        if (_pathIndex >= path.Count - 1)
        {
            Debug.Log($"[{animatronicName}] reached the end of the path: {CurrentRoom.roomName}");
            TriggerJumpscare();
            return;
        }

        int roll = Random.Range(1, 21);
        Debug.Log($"[{animatronicName}] Roll: {roll}  (needs <= {aiScore} to move)");

        if (roll <= aiScore)
        {
            if (CurrentRoom is DoorRoom currentDoorRoom && !currentDoorRoom.TryEnter(this))
            {
                if (IsFreddy)
                    SwitchFreddyPath();

                Debug.Log($"[{animatronicName}] blocked by door; current path is now {GetPathName()}.");
                return;
            }

            _pathIndex++;
            CurrentRoom = path[_pathIndex];
            UpdateVisuals();
            OnMoved?.Invoke();
            Debug.Log($"[{animatronicName}] moved to: {CurrentRoom.roomName}");

            if (_pathIndex >= path.Count - 1)
                TriggerJumpscare();
        }
    }

    private bool IsFreddy => animatronicName.ToLowerInvariant().Contains("freddy");

    private List<Room> GetCurrentPath()
    {
        if (!IsFreddy)
            return roomPath;

        return _freddyPath == FreddyPath.Left ? leftDoorPath : rightDoorPath;
    }

    private void PickRandomFreddyPath()
    {
        _freddyPath = Random.Range(0, 2) == 0 ? FreddyPath.Left : FreddyPath.Right;
    }

    private void SwitchFreddyPath()
    {
        _freddyPath = _freddyPath == FreddyPath.Left ? FreddyPath.Right : FreddyPath.Left;
        _pathIndex = 0;

        List<Room> path = GetCurrentPath();
        if (path == null || path.Count == 0)
            return;

        CurrentRoom = path[0];
        UpdateVisuals();
    }

    private string GetPathName()
    {
        return IsFreddy ? _freddyPath.ToString() : "main";
    }

    private void TriggerJumpscare()
    {
        Deactivate();
        Debug.Log($"[{animatronicName}] JUMPSCARE!");
        OnJumpscare?.Invoke();
    }

    // ──────────────────────────────────────────────────────────────
    // Visual helpers

    private void UpdateVisuals()
    {
        HideAllVisuals();

        List<RoomVisual> visuals = GetCurrentVisuals();
        if (_pathIndex >= 0 && _pathIndex < visuals.Count)
            visuals[_pathIndex].SetVisible(true);
    }

    private void HideAllVisuals()
    {
        foreach (var v in roomVisuals)
            v.SetVisible(false);

        foreach (var v in leftDoorVisuals)
            v.SetVisible(false);

        foreach (var v in rightDoorVisuals)
            v.SetVisible(false);
    }

    private List<RoomVisual> GetCurrentVisuals()
    {
        if (!IsFreddy)
            return roomVisuals;

        return _freddyPath == FreddyPath.Left ? leftDoorVisuals : rightDoorVisuals;
    }
}

[System.Serializable]
public class RoomVisual
{
    [Tooltip("Assign a SpriteRenderer to show/hide via renderer.enabled.")]
    public SpriteRenderer spriteRenderer;

    [Tooltip("Assign a GameObject to show/hide via SetActive(). Can be used instead of or alongside the SpriteRenderer.")]
    public GameObject gameObject;

    public void SetVisible(bool visible)
    {
        if (spriteRenderer != null)
            spriteRenderer.enabled = visible;

        if (gameObject != null)
            gameObject.SetActive(visible);
    }
}
