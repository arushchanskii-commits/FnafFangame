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
    
    private float timeElapsed = 0f;
    private int currentHour = 12;
    private int currentMinute = 0;
    private bool isAM = true;
    private bool nightStarted = false;
    private bool nightEnded = false;
    private bool isTextVisible = true;
    
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
        
        // Check if 6am reached (end of night)
        if (gameHour == 6 && currentMinute == 0 && isAM)
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
        
        // You can add 6am music/jumpscare logic here
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
