using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Resident : MonoBehaviour {
    public enum AvailableTasks {
        None,
        Church,
        Home,
        Work,
        Market,
    }

    public bool sleeping = false;
    public bool workedToday = false;
    public AvailableTasks currentTask = AvailableTasks.None;
    [SerializeField] public List<AvailableTasks> tasks;
    private NavMeshAgent _navMeshAgent;
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;
    private CapsuleCollider _collider;
    private House _home;
    [SerializeField] private Transform positionIndicator;
    [SerializeField] private Workplace workplace;

    public float satisfaction = 0f;
    public float religionSatisfaction = 0f;
    public float foodSatisfaction = 0f;
    private TimeController _timeController;
    
    public event EventHandler OnTaskAdded;

    private void Awake() {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        workplace = FindObjectOfType<Workplace>();
        _collider = GetComponent<CapsuleCollider>();
    }

    private void Start() {
        _timeController = TimeController.Instance;

        _timeController.OnSleepTimeStart += EnterSleepMode;
        _timeController.OnSleepTimeEnd += StopSleepMode;
        _timeController.OnNewDay += (sender, args) => workedToday = false;

        foodSatisfaction = Random.Range(20, 101);
    }

    private void Update() {
        if (sleeping) {
            _navMeshAgent.destination = _home.entrance.position;
        }
        else {
            if (currentTask == AvailableTasks.None && tasks.Count > 0) {
                ExecuteTask();
            } else if (currentTask == AvailableTasks.None && tasks.Count == 0) {
                AvailableTasks nextTask = GetNextTask();
                if (nextTask == AvailableTasks.None) {
                    currentTask = AvailableTasks.None;
                } else {
                    AddTask(nextTask);
                }
                
            }
        }
        // Decrease satisfactions
        CalculateSatisfactions();
        // Rotate indicator
        positionIndicator.Rotate(60 * Time.deltaTime, 0, 0);
    }

    private void ExecuteTask() {
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
            case AvailableTasks.Market:
                Transform nextMarketEntranceTransform = _home.FindNextMarket(this);
                if (nextMarketEntranceTransform != null) {
                    _navMeshAgent.destination = nextMarketEntranceTransform.position;
                }
                else {
                    CantCompleteTask();
                    foodSatisfaction = 0f;
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

    private AvailableTasks GetNextTask() {
        if (foodSatisfaction > 50 && !workedToday) {
            return AvailableTasks.Work;
        } else if (foodSatisfaction <= 50) {
            return AvailableTasks.Market;
        } else if (religionSatisfaction < 50) {
            return AvailableTasks.Church;
        } else if (foodSatisfaction < 75) {
            return AvailableTasks.Market;
        } else if (religionSatisfaction < 60) {
            return AvailableTasks.Church;
        } else if (foodSatisfaction < 85) {
            return AvailableTasks.Market;
        } else if (religionSatisfaction < 75) {
            return AvailableTasks.Church;
        } else if (foodSatisfaction < 90) {
            return AvailableTasks.Market;
        } else if (religionSatisfaction < 90) {
            return AvailableTasks.Church;
        } else if(!_home._residentsCurrentlyInHome.Contains(transform)) {
            return AvailableTasks.Home;
        } else {
            return AvailableTasks.None;
        }
    }
    
    private void CalculateSatisfactions() {
        if (religionSatisfaction > 0) {
            religionSatisfaction -= Time.deltaTime * 0.5f;
        }
        if (foodSatisfaction > 0) {
            foodSatisfaction -= Time.deltaTime * 1.5f;
        }

        satisfaction = foodSatisfaction * 0.6f + religionSatisfaction * 0.4f;
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
        currentTask = AvailableTasks.None;
        tasks.RemoveAt(0);
    }

    public void DisableResident() {
        _collider.enabled = false;
        skinnedMeshRenderer.enabled = false;
    }
    
    public void EnableResident() {
        _collider.enabled = true;
        skinnedMeshRenderer.enabled = true;
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

    private void EnterSleepMode(object sender, EventArgs e) {
        sleeping = true;
    }
    
    private void StopSleepMode(object sender, EventArgs e) {
        sleeping = false;
    }
}
