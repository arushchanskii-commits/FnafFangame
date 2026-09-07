using UnityEngine;
using UnityEngine.UI;

public class GameClock : MonoBehaviour
{
    [Header("Clock Settings")]
    public Text clockText;
    public float nightDuration = 360f; // 6 minutes in seconds
    
    [Header("Flicker Sync")]
    [Tooltip("Assign the LightFlicker component to sync clock flickering with lights")]
    public LightFlicker lightFlicker;
    
    [Header("Time Display")]
    public bool showSeconds = false;

    [Header("Night End")]
    [SerializeField] private ScreenFade screenFade;
    [SerializeField] private Text nightEndText;
    [SerializeField] private Font nightEndFont;
    [SerializeField] private int nightEndFontSize = 100;
    [SerializeField] private AudioSource fadeAudioSource;
    [SerializeField] private AudioClip fadeSoundEffect;
    [Range(0f, 1f)]
    [SerializeField] private float fadeSoundVolume = 1f;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private float sceneChangeDelay = 3f;
    [SerializeField] private float blinkInterval = 0.5f;
    
    private float timeElapsed = 0f;
    private int currentHour = 12;
    private int currentMinute = 0;
    private bool isAM = true;
    private bool nightStarted = false;
    private bool nightEnded = false;
    private bool isTextVisible = true;
    private Coroutine nightEndCoroutine;
    
    // Time calculation: 12am to 6am = 6 hours = 360 minutes
    // Night duration = 360 seconds (6 minutes)
    // Ratio: 360 minutes / 360 seconds = 1 game minute per 1 real second
    private const float MINUTES_PER_SECOND = 1f;

    private void Start()
    {
        timeElapsed = 0f;
        currentHour = 12;
        currentMinute = 0;
        isAM = true; // 12am
        nightStarted = false;
        nightEnded = false;
        
        UpdateClockDisplay();
    }

    private void Update()
    {
        if (nightEnded) return;
        
        // Start night on first frame
        if (!nightStarted)
        {
            nightStarted = true;
            Debug.Log("Night started at 12am");
        }
        
        // Sync flickering with LightFlicker if assigned
        if (lightFlicker != null && lightFlicker.isFlickering)
        {
            // Mirror the light flicker state
            isTextVisible = lightFlicker.isLightOn;
            if (clockText != null)
            {
                clockText.enabled = isTextVisible;
            }
        }
        else if (lightFlicker != null && !lightFlicker.isFlickering)
        {
            // Power is available - ensure text is visible
            isTextVisible = true;
            if (clockText != null) clockText.enabled = true;
        }
        
        // Increment time (time keeps progressing even when clock is hidden)
        timeElapsed += Time.deltaTime;
        
        // Calculate game minutes passed (1 game minute per 1 real second)
        float gameMinutesPassed = timeElapsed * MINUTES_PER_SECOND;
        
        // Calculate total minutes from 12am
        int totalMinutes = Mathf.FloorToInt(gameMinutesPassed);
        
        // Convert to hour:minute format
        int totalHours = totalMinutes / 60;
        currentMinute = totalMinutes % 60;
        
        int gameHour = 12 + totalHours;
        
        // Convert to 12-hour format with AM/PM
        if (gameHour >= 24)
        {
            gameHour -= 24;
            isAM = true;
        }
        else if (gameHour >= 12)
        {
            isAM = false;
            if (gameHour > 12)
            {
                gameHour -= 12;
            }
        }
        else
        {
            isAM = true;
            if (gameHour == 0)
            {
                gameHour = 12;
            }
        }
        
        currentHour = gameHour;
        
        // End the night from the timer threshold so frame timing cannot skip 6:00 AM.
        if (timeElapsed >= nightDuration)
        {
            OnNightEnd();
        }
        
        UpdateClockDisplay();
    }
    
    private void UpdateClockDisplay()
    {
        if (clockText == null) return;
        
        // Only update text if it's visible
        if (!isTextVisible) return;
        
        string timeString = string.Format("{0:D2}:{1:D2} {2}", currentHour, currentMinute, isAM ? "AM" : "PM");
        
        if (showSeconds)
        {
            int seconds = Mathf.FloorToInt(timeElapsed % 60);
            timeString = string.Format("{0:D2}:{1:D2}:{2:D2} {3}", currentHour, currentMinute, seconds, isAM ? "AM" : "PM");
        }
        
        clockText.text = timeString;
    }
    
    private void OnNightEnd()
    {
        nightEnded = true;
        currentHour = 6;
        currentMinute = 0;
        isAM = true;
        UpdateClockDisplay();
        Debug.Log("6am reached! Night ended.");

        if (nightEndCoroutine != null)
        {
            StopCoroutine(nightEndCoroutine);
        }

        nightEndCoroutine = StartCoroutine(ShowNightEndAndAdvance());
    }

    private System.Collections.IEnumerator ShowNightEndAndAdvance()
    {
        if (fadeAudioSource != null && fadeSoundEffect != null)
        {
            fadeAudioSource.PlayOneShot(fadeSoundEffect, fadeSoundVolume);
        }

        if (screenFade != null)
        {
            screenFade.FadeToBlack(fadeDuration);
            yield return new WaitForSeconds(Mathf.Max(0f, fadeDuration));
        }

        if (nightEndText != null)
        {
            if (nightEndFont != null)
            {
                nightEndText.font = nightEndFont;
            }

            if (nightEndFontSize > 0)
            {
                nightEndText.fontSize = nightEndFontSize;
            }

            nightEndText.text = "6 AM";
            Color textColor = nightEndText.color;
            textColor.a = 0f;
            nightEndText.color = textColor;
            nightEndText.enabled = true;

            float fadeElapsed = 0f;
            while (fadeElapsed < fadeDuration)
            {
                fadeElapsed += Time.deltaTime;
                textColor.a = Mathf.Clamp01(fadeElapsed / fadeDuration);
                nightEndText.color = textColor;
                yield return null;
            }

            float blinkElapsed = 0f;
            while (blinkElapsed < sceneChangeDelay)
            {
                nightEndText.enabled = !nightEndText.enabled;
                float interval = Mathf.Max(0.05f, blinkInterval);
                yield return new WaitForSeconds(interval);
                blinkElapsed += interval;
            }
        }
        else
        {
            yield return new WaitForSeconds(Mathf.Max(0f, sceneChangeDelay));
        }

        MiniGameSwapper swapper = FindObjectOfType<MiniGameSwapper>();
        if (swapper != null)
        {
            swapper.OnMiniGameComplete();
        }
        else
        {
            Debug.LogError("GameClock: 6 AM transition finished, but no MiniGameSwapper was found.");
        }
    }
    
    public float GetTimeProgress()
    {
        return timeElapsed / nightDuration;
    }
    
    public bool IsNightEnded()
    {
        return nightEnded;
    }
    
    public void ResetClock()
    {
        timeElapsed = 0f;
        currentHour = 12;
        currentMinute = 0;
        isAM = false;
        nightStarted = false;
        nightEnded = false;
        UpdateClockDisplay();
    }
}
