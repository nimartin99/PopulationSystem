using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    public static TimeController Instance { get; private set; }
    [SerializeField] private Light sun;
    private Transform _sunTransform;

    [SerializeField] private float RealSecondsPerInGameDay = 10f;
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
    private bool _sleepTime = false;
    
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
        _percentageOfDayProgress += Time.deltaTime / RealSecondsPerInGameDay;
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
        if (hours >= 23 && !_sleepTime) {
            OnSleepTimeStart?.Invoke(this, EventArgs.Empty);
            _sleepTime = true;
        } else if (hours >= 7 && hours < 23 && _sleepTime) {
            OnSleepTimeEnd?.Invoke(this, EventArgs.Empty);
            _sleepTime = false;
        }
    }

    private void AdjustLight() {
        // if (hours is > 6 and < 18) {
        //     float xRoation = 0f;
        //     if (hours > 12) {
        //         xRoation = 6f + 0.11f * ((float)hours * 60f + (float)minutes);
        //     }
        //     else {
        //         xRoation = 50f - 0.11f * ((float)hours * 60f + (float)minutes);
        //     }
        //     _sunTransform.rotation = Quaternion.Euler(xRoation, -85f + 0.23f * ((float) hours * 60f + (float) minutes), -50f + 0.14f * (float) hours * 60f + (float) minutes);
        // } else if (_sunTransform.rotation != Quaternion.Euler(6, -85, -50)) {
        //     _sunTransform.rotation = Quaternion.Euler(6, -85, -50);
        // }
        
        
        if (hours > 18) {
            sun.intensity = 0.5f +  (1440 - (float) hours * 60 - (float) minutes) / 360;
        } else if (hours < 6 ) {
            sun.intensity = 0.5f + ((float) hours * 60 + (float) minutes) / 360;
        }
        
    }
}
