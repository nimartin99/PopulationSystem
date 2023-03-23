using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    public static TimeController Instance { get; private set; }
    [SerializeField] private Light sun;
    private Transform _sunTransform;

    public float realSecondsPerInGameDay = 10f;
    private float _percentageOfDayProgress = 0f;
    public int hoursInADay = 24;
    public int workHours;
    public int daysSinceStart = 0;
    public double hours;
    public double minutes;

    public event EventHandler OnNextHour;
    public event EventHandler OnSleepTimeStart;
    public event EventHandler OnSleepTimeEnd;
    public event EventHandler OnNewDay;
    public bool sleepTime = false;
    
    private void Awake() {
        if (Instance != null && Instance != this) { 
            Destroy(this); 
        } 
        else { 
            Instance = this; 
        }

        _sunTransform = sun.transform;
    }

    // Update is called once per frame
    void Update() {
        _percentageOfDayProgress += Time.deltaTime / realSecondsPerInGameDay;
        double hoursBeforeAdding = hours;
        hours = Math.Floor(_percentageOfDayProgress * 24);
        minutes = Math.Floor(_percentageOfDayProgress * 1440 - hours * 60);
        if (hours >= 24) { 
            daysSinceStart++;
            OnNewDay?.Invoke(this, EventArgs.Empty);
            _percentageOfDayProgress = 0f;
        }
        AdjustLight();

        if (hoursBeforeAdding != hours) {
            OnNextHour?.Invoke(this, EventArgs.Empty);
        }
        if (hours >= 23 && !sleepTime) {
            OnSleepTimeStart?.Invoke(this, EventArgs.Empty);
            sleepTime = true;
        } else if (hours >= 7 && hours < 23 && sleepTime) {
            OnSleepTimeEnd?.Invoke(this, EventArgs.Empty);
            sleepTime = false;
        }
    }

    private void AdjustLight() {
        if (hours > 18) {
            sun.intensity = 0.5f +  (1440 - (float) hours * 60 - (float) minutes) / 360;
        } else if (hours < 6 ) {
            sun.intensity = 0.5f + ((float) hours * 60 + (float) minutes) / 360;
        }
    }
}
