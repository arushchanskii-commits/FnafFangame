using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PowerManager : MonoBehaviour
{
    [Header("Power Settings")]
    public float maxPower = 100f;
    public float currentPower = 100f;
    
    [Header("Power Consumption")]
    public float baseDrainRate = 2f;
    public float cameraDrainRate = 5f;
    public float doorDrainRate = 8f;
    public float lightDrainRate = 3f;
    
    [Header("UI")]
    public Text powerText;
    
    [Header("Power Bars")]
    public Image[] powerBars;
    
    [Header("Generator Reference")]
    public PowerCharger powerCharger;
    
    [Header("Power Outage")]
    public GameObject powerOutageScreen;
    public bool isPowerOutage = false;
    
    [Header("Lights to Disable on Power Outage")]
    public List<PowerLight> powerLights = new List<PowerLight>();
    
    private int activeDoors = 0;
    private int activeLights = 0;
    private bool isCameraActive = false;
    private float currentDrainRate;
    private float drainTimer = 0f;
    private float drainInterval = 1f;
    
    public static PowerManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        // Removed DontDestroyOnLoad - power resets per scene
    }

    private void Start()
    {
        currentPower = maxPower;
        UpdatePowerUI();
    }

    private void Update()
    {
        if (isPowerOutage) return;
        
        drainTimer += Time.deltaTime;
        
        if (drainTimer >= drainInterval)
        {
            drainTimer = 0f;
            CalculateDrainRate();
            DrainPower();
            UpdatePowerUI();
        }
    }

    private void CalculateDrainRate()
    {
        currentDrainRate = baseDrainRate;
        
        if (activeDoors > 0)
        {
            currentDrainRate += activeDoors * doorDrainRate;
        }
        
        if (activeLights > 0)
        {
            currentDrainRate += activeLights * lightDrainRate;
        }
        
        if (isCameraActive)
        {
            currentDrainRate += cameraDrainRate;
        }
    }

    private void DrainPower()
    {
        currentPower -= currentDrainRate;
        
        if (currentPower <= 0)
        {
            currentPower = 0;
            TriggerPowerOutage();
        }
    }

    private void UpdatePowerUI()
    {
        if (powerText != null)
        {
            float percentage = (currentPower / maxPower) * 100f;
            powerText.text = $"Power: {percentage:F0}%";
        }
        
        UpdatePowerBars();
    }
    
    private void UpdatePowerBars()
    {
        if (powerBars == null || powerBars.Length == 0) return;
        
        int activeBars = CalculateActiveBars();
        
        for (int i = 0; i < powerBars.Length; i++)
        {
            if (powerBars[i] != null)
            {
                powerBars[i].enabled = (i < activeBars);
            }
        }
    }
    
    private int CalculateActiveBars()
    {
        int bars = 1;
        
        if (activeDoors > 0)
        {
            bars += Mathf.Min(activeDoors, 2);
        }
        
        if (activeLights > 0)
        {
            bars += 1;
        }
        
        if (isCameraActive)
        {
            bars += 1;
        }
        
        if (powerCharger != null && powerCharger.isCharging)
        {
            bars -= 1;
        }
        
        return Mathf.Max(0, Mathf.Min(bars, powerBars.Length));
    }

    public void UpdatePowerUIPublic()
    {
        UpdatePowerUI();
    }

    private void TriggerPowerOutage()
    {
        isPowerOutage = true;
        Debug.Log("POWER OUTAGE!");
        
        if (powerOutageScreen != null)
        {
            powerOutageScreen.SetActive(true);
        }
        
        DisableAllDevices();
    }

    private void DisableAllDevices()
    {
        // Disable all assigned lights
        foreach (PowerLight powerLight in powerLights)
        {
            if (powerLight.lightSource != null)
            {
                powerLight.Disable();
                Debug.Log($"Light {powerLight.lightSource.name} disabled due to power outage");
            }
        }
        
        Debug.Log("All devices disabled due to power outage");
    }

    public void RegisterDevice(DeviceType deviceType)
    {
        if (isPowerOutage) return;
        
        switch (deviceType)
        {
            case DeviceType.Door:
                activeDoors++;
                break;
            case DeviceType.Light:
                activeLights++;
                break;
            case DeviceType.Camera:
                isCameraActive = true;
                break;
        }
        
        UpdatePowerBars();
    }

    public void UnregisterDevice(DeviceType deviceType)
    {
        if (isPowerOutage) return;
        
        switch (deviceType)
        {
            case DeviceType.Door:
                activeDoors = Mathf.Max(0, activeDoors - 1);
                break;
            case DeviceType.Light:
                activeLights = Mathf.Max(0, activeLights - 1);
                break;
            case DeviceType.Camera:
                isCameraActive = false;
                break;
        }
        
        UpdatePowerBars();
    }

    public bool CanUseDevice()
    {
        return !isPowerOutage && currentPower > 0;
    }

    public float GetPowerPercentage()
    {
        return currentPower / maxPower;
    }

    public float GetTimeUntilOutage()
    {
        if (currentDrainRate <= 0) return float.MaxValue;
        return currentPower / currentDrainRate;
    }

    public enum DeviceType
    {
        Camera,
        Door,
        Light
    }
}
