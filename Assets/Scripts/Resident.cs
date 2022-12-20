using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Resident : MonoBehaviour {
    public enum AvailableTasks {
        None,
        Church,
        Home,
    }

    public AvailableTasks currentTask = AvailableTasks.None;
    [SerializeField] public List<AvailableTasks> tasks;
    private NavMeshAgent _navMeshAgent;
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;
    private House _home;
    [SerializeField] private Transform positionIndicator;

    public float religionSatisfaction = 0f;
    
    public event EventHandler OnTaskAdded;

    private void Awake() {
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Update() {
        if (currentTask == AvailableTasks.None && tasks.Count > 0) {
            currentTask = tasks[0];
            switch (currentTask) {
                case AvailableTasks.Church:
                    Transform nextChurchTransform = _home.FindNextChurch();
                    if (nextChurchTransform != null) {
                        _navMeshAgent.destination = nextChurchTransform.position;
                    }
                    else {
                        RemoveTask(0);
                        currentTask = AvailableTasks.None;
                        religionSatisfaction = 0f;
                    }
                    break;
                case AvailableTasks.Home:
                default:
                    _navMeshAgent.destination = _home.entrance.position;
                    break;
            }
        }
        else if(currentTask == AvailableTasks.None && tasks.Count == 0 && !_home._residentsCurrentlyInHome.Contains(transform)) {
            AddTask(AvailableTasks.Home);
        }
        
        // Rotate indicator
        positionIndicator.Rotate(60 * Time.deltaTime, 0, 0);
    }

    public void ResidentConstructor(House home) {
        _home = home;
    }

    public void AddTask(AvailableTasks task) {
        tasks.Add(task);
        OnTaskAdded?.Invoke(this, EventArgs.Empty);
    }

    public void RemoveTask(int index) {
        tasks.RemoveAt(index);
    }

    public void CompleteTask() {
        currentTask = AvailableTasks.None;
        tasks.RemoveAt(0);
    }

    public void DisableVisual() {
        skinnedMeshRenderer.enabled = false;
    }
    
    public void EnableVisual() {
        skinnedMeshRenderer.enabled = true;
    }
    
    public void EnableIndicator() {
        positionIndicator.gameObject.SetActive(true);
    }
    
    public void DisableIndicator() {
        positionIndicator.gameObject.SetActive(false);
    }
}
