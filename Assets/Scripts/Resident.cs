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
        Work,
        Sleep,
    }

    public bool sleeping = false;

    public AvailableTasks currentTask = AvailableTasks.None;
    [SerializeField] public List<AvailableTasks> tasks;
    private NavMeshAgent _navMeshAgent;
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;
    private House _home;
    [SerializeField] private Transform positionIndicator;
    [SerializeField] private Workplace workplace;

    public float religionSatisfaction = 0f;
    private TimeController _timeController;
    
    public event EventHandler OnTaskAdded;

    private void Awake() {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        workplace = FindObjectOfType<Workplace>();
        _timeController = TimeController.Instance;

        _timeController.OnSleepTimeStart += EnterSleepMode;
        _timeController.OnSleepTimeEnd += StopSleepMode;
    }

    private void Update() {
        if (sleeping) {
            _navMeshAgent.destination = _home.entrance.position;
        }
        else if (currentTask == AvailableTasks.None && tasks.Count > 0)
        {
            currentTask = tasks[0];
            switch (currentTask) {
                case AvailableTasks.Church:
                    Transform nextChurchEntranceTransform = _home.FindNextChurch(this);
                    if (nextChurchEntranceTransform != null) {
                        _navMeshAgent.destination = nextChurchEntranceTransform.position;
                    }
                    else {
                        CantCompleteTask();
                        religionSatisfaction = 0f;
                    }
                    break;
                case AvailableTasks.Work:
                    if (_home.PathFromHomeAvailable(workplace.transform, this)) {
                        _navMeshAgent.destination = workplace.transform.position;
                    }
                    else {
                        CantCompleteTask();
                    }
                    break;
                case AvailableTasks.Home:
                default:
                    _navMeshAgent.destination = _home.entrance.position;
                    break;
            }
        }
        else if(currentTask == AvailableTasks.None && tasks.Count == 0 && !_home._residentsCurrentlyInHome.Contains(transform)) {
            Debug.Log("no tasks, no current task and not in home -> so add add task Home");
            AddTask(AvailableTasks.Home);
        }
        
        // Rotate indicator
        positionIndicator.Rotate(60 * Time.deltaTime, 0, 0);
    }

    private void CantCompleteTask() {
        RemoveTask(0);
        currentTask = AvailableTasks.None;
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
        Debug.Log("Complete Task");
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
        AddTask(AvailableTasks.Work);
    }
    
    public void DisableIndicator() {
        positionIndicator.gameObject.SetActive(false);
    }

    private void EnterSleepMode(object sender, EventArgs e) {
        Debug.Log("Enter Sleep Mode");
        sleeping = true;
    }
    
    private void StopSleepMode(object sender, EventArgs e) {
        Debug.Log("Stop Sleep Mode");
        sleeping = false;
    }
}
