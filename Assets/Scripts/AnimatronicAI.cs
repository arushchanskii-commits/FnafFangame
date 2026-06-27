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

    [Header("Timer")]
    [Tooltip("Seconds between each movement attempt.")]
    public float tickInterval = 1f;

    [Header("Room Visuals")]
    [Tooltip("One entry per room (same order as Room Path). Assign either a SpriteRenderer OR a GameObject – whichever is set will be shown/hidden. If both are set, both are toggled.")]
    public List<RoomVisual> roomVisuals = new();

    // ── Events ─────────────────────────────────────────────────────
    /// <summary>Fired when the animatronic reaches the last room (the office).</summary>
    public System.Action OnJumpscare;

    // ── Runtime state ──────────────────────────────────────────────
    public Room CurrentRoom { get; private set; }
    private int       _pathIndex   = 0;
    private bool      _active      = false;
    private Coroutine _aiCoroutine = null;

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
        if (roomPath == null || roomPath.Count == 0)
        {
            Debug.LogWarning($"[{animatronicName}] roomPath is empty – AI will not move.");
            return;
        }

        aiScore     = Mathf.Clamp(score, 0, 20);
        _pathIndex  = 0;
        CurrentRoom = roomPath[0];
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
        if (roomPath == null || roomPath.Count == 0) return;

        _pathIndex  = Mathf.Clamp(pathIndex, 0, roomPath.Count - 1);
        CurrentRoom = roomPath[_pathIndex];
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
        while (_active)
        {
            yield return new WaitForSeconds(tickInterval);
            TryMove();
        }
    }

    private void TryMove()
    {
        if (_pathIndex >= roomPath.Count - 1)
        {
            Debug.Log($"[{animatronicName}] reached the end of the path: {CurrentRoom.roomName}");
            TriggerJumpscare();
            return;
        }

        int roll = Random.Range(1, 21);
        Debug.Log($"[{animatronicName}] Roll: {roll}  (needs <= {aiScore} to move)");

        if (roll <= aiScore)
        {
            _pathIndex++;
            CurrentRoom = roomPath[_pathIndex];
            UpdateVisuals();
            Debug.Log($"[{animatronicName}] moved to: {CurrentRoom.roomName}");

            if (_pathIndex >= roomPath.Count - 1)
                TriggerJumpscare();
        }
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
        for (int i = 0; i < roomVisuals.Count; i++)
            roomVisuals[i].SetVisible(i == _pathIndex);
    }

    private void HideAllVisuals()
    {
        foreach (var v in roomVisuals)
            v.SetVisible(false);
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
