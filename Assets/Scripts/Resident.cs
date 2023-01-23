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
        Tavern,
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
    public float happiness = 0f;
    public float religionSatisfaction = 0f;
    public float foodSatisfaction = 0f;
    public float tavernSatisfaction = 0f;
    private TimeController _timeController;
    private UIControl _uiControl;

    public float _lowestPriority = 1f;
    // Overall priority
    public float workPriority;
    // Needs
    public float foodPriority;
    public float religionPriority;
    // Happiness
    public float tavernPriority;

    public event EventHandler OnTaskAdded;

    private void Awake() {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        workplace = FindObjectOfType<Workplace>();
        _collider = GetComponent<CapsuleCollider>();
    }

    private void Start() {
        _timeController = TimeController.Instance;
        _uiControl = UIControl.Instance;
        
        _timeController.OnSleepTimeStart += EnterSleepMode;
        _timeController.OnSleepTimeEnd += StopSleepMode;
        _timeController.OnNewDay += (sender, args) => workedToday = false;

        CalculatePriorities();
    }

    private void Update() {
        if (sleeping) {
            _navMeshAgent.destination = _home.entrance.position;
        }
        else {
            if (currentTask == AvailableTasks.None && tasks.Count > 0) {
                ExecuteTask();
            } else if (currentTask == AvailableTasks.None && tasks.Count == 0) {
                AvailableTasks nextTask = GetNextTask(_uiControl.currentFoodPortion == "No Food");
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
            case AvailableTasks.Tavern:
                Transform nextTavernEntranceTransform = _home.FindNextTavern(this);
                if (nextTavernEntranceTransform != null) {
                    _navMeshAgent.destination = nextTavernEntranceTransform.position;
                }
                else {
                    CantCompleteTask();
                    tavernSatisfaction = 0f;
                }
                break;
            case AvailableTasks.Home:
            default:
                _navMeshAgent.destination = _home.entrance.position;
                break;
        }
    }

    private AvailableTasks GetNextTask(bool noFood) {
        Dictionary<string, float> needs = new Dictionary<string, float>();
        if (!workedToday) {
            needs.Add("work", (workPriority + Random.Range(-0.15f, 0.15f)) * 100);
        }
        if (_home.FindNextMarket(this) != null && !noFood) {
            needs.Add("food", (100f - foodSatisfaction) * foodPriority + (foodSatisfaction < 25 ? 10 : Random.Range(-10, 10)));
        }
        if (_home.FindNextChurch(this) != null) {
            needs.Add("religion", (100f - religionSatisfaction) * religionPriority + (religionSatisfaction < 25 ? 10 : Random.Range(-10, 10)));
        }
        if (_home.FindNextTavern(this) != null) {
            needs.Add("tavern", (100f - tavernSatisfaction) * tavernPriority + Random.Range(-10, 10));
        }

        Debug.Log("------------- Priorities -------------");
        foreach (KeyValuePair<string, float> need in needs) {
            Debug.Log(need.Key + ": " + need.Value);
        }
        Debug.Log("--------------------------------------");
        
        string highestPriorityNeedName = "";
        float highestPriorityNeed = 0f;
        foreach (KeyValuePair<string, float> need in needs) {
            if (need.Value > highestPriorityNeed) {
                highestPriorityNeedName = need.Key;
                highestPriorityNeed = need.Value;
            }
        }
        
        Debug.Log("highestPriorityNeed: " + highestPriorityNeedName +" - " + highestPriorityNeed);
        
        if (highestPriorityNeed < _lowestPriority * 50) {
            return _home._residentsCurrentlyInHome.Contains(transform) ? AvailableTasks.None : AvailableTasks.Home;
        }

        switch (highestPriorityNeedName) {
            case "work":
                return AvailableTasks.Work;
            case "food":
                return AvailableTasks.Market;
            case "religion":
                return AvailableTasks.Church;
            case "tavern":
                return AvailableTasks.Tavern;
            default:
                return _home._residentsCurrentlyInHome.Contains(transform) ? AvailableTasks.None : AvailableTasks.Home;
        }
    }
    
    private void CalculateSatisfactions() {
        if (religionSatisfaction > 0) {
            religionSatisfaction -= Time.deltaTime * 0.5f;
            if (religionSatisfaction < 0) {
                religionSatisfaction = 0f;
            }
        }
        if (foodSatisfaction > 0) {
            foodSatisfaction -= Time.deltaTime * 1.5f;
            if (foodSatisfaction < 0) {
                foodSatisfaction = 0f;
            }
        }
        if (tavernSatisfaction > 0) {
            tavernSatisfaction -= Time.deltaTime * 0.3f;
            if (tavernSatisfaction < 0) {
                tavernSatisfaction = 0f;
            }
        }

        satisfaction = foodSatisfaction * 0.6f + religionSatisfaction * 0.4f;
        happiness = (100f - _uiControl.currentTaxes) * 0.3f + tavernSatisfaction * 0.7f;
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

    private void CalculatePriorities() {
        foodSatisfaction = Random.Range(20, 101);
        religionSatisfaction = Random.Range(0, 101);
        tavernSatisfaction = Random.Range(0, 101);
        
        List<float> priorities = new List<float>();
        float overallPriority = 1f;
        workPriority = Random.Range(0.2f, 0.45f);
        overallPriority -= workPriority;
        foodPriority = Random.Range(overallPriority * 0.25f, overallPriority * 0.75f);
        overallPriority -= foodPriority;
        religionPriority = Random.Range(overallPriority * 0.25f, overallPriority * 0.75f);
        overallPriority -= religionPriority;
        tavernPriority = overallPriority;
        
        priorities.Add(workPriority);
        priorities.Add(religionPriority);
        priorities.Add(foodPriority);
        priorities.Add(tavernPriority);
        foreach (float priority in priorities) {
            if (_lowestPriority > priority) {
                _lowestPriority = priority;
            }
        }
    }
}
