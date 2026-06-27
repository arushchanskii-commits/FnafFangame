// Assets/Scripts/Core/GameManager.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Nacht")]
    public int currentNight = 1;
    public float nightDurationSeconds = 534f; // 8:53 Uhr ≈ 535s Echtzeit

    [Header("Animatronics")]
    public List<AnimatronicAI> animatronics = new();

    [Header("Events")]
    public UnityEvent<string> onGameOver;
    public UnityEvent onNightComplete;

    private float _nightTimer;
    private bool  _nightRunning;

    // Ingame-Stunden: 12 AM – 6 AM = 6 Stunden
    public int CurrentHour => Mathf.FloorToInt((_nightTimer / nightDurationSeconds) * 6);

    // ----------------------------------------------------------------

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start() => StartNight(currentNight);

    // ----------------------------------------------------------------

    public void StartNight(int night)
    {
        currentNight = night;
        _nightTimer  = 0f;
        _nightRunning = true;

        foreach (var anim in animatronics)
        {
            anim.Initialize(night);
            anim.OnJumpscare += () => TriggerGameOver(anim.animatronicName);
        }

        StartCoroutine(NightTimer());
    }

    private IEnumerator NightTimer()
    {
        while (_nightTimer < nightDurationSeconds && _nightRunning)
        {
            _nightTimer += Time.deltaTime;
            yield return null;
        }

        if (_nightRunning)
            CompleteNight();
    }

    public void TriggerGameOver(string killer)
    {
        _nightRunning = false;
        foreach (var anim in animatronics)
            anim.Deactivate();

        Debug.Log($"Game Over – getötet von {killer}");
        onGameOver?.Invoke(killer);
    }

    private void CompleteNight()
    {
        foreach (var anim in animatronics)
            anim.Deactivate();

        Debug.Log($"Nacht {currentNight} überstanden!");
        onNightComplete?.Invoke();
    }
}