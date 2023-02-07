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
        Riot,
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
    private AvailableTasks _previousCompletedTask = AvailableTasks.None;
    
    // Animation
    [SerializeField] private Animator _animator;

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

    public List<float> _overallSatisfactionMeasurements = new List<float>();
    public List<float> _foodSatisfactionMeasurements = new List<float>();
    public List<float> _religionSatisfactionMeasurements = new List<float>();
    public List<float> _tavernSatisfactionMeasurements = new List<float>();

    public float _overallSatisfactionOverTime = 100f;
    public float _foodSatisfactionOverTime = 100f;
    public float _religionSatisfactionOverTime = 100f;
    public float _tavernSatisfactionOverTime = 100f;
    
    // Riot
    public List<string> riotingReasons = new List<string>();
    public bool rioting;
    [SerializeField] private Transform riotPrefab;
    [HideInInspector] public int riotCooldown = 0;

    private void Awake() {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        workplace = FindObjectOfType<Workplace>();
        _collider = GetComponent<CapsuleCollider>();
    }

    private void Start() {
        _timeController = TimeController.Instance;
        _uiControl = UIControl.Instance;

        _timeController.OnNextHour += MeasureSatisfactions;
        _timeController.OnNextHour += CalculateRiotRisk;
        _timeController.OnNextHour += (obj, eventArgs) => { if (riotCooldown > 0) riotCooldown--; };
        _timeController.OnSleepTimeStart += EnterSleepMode;
        _timeController.OnSleepTimeEnd += StopSleepMode;
        _timeController.OnNewDay += (sender, args) => workedToday = false;

        CalculatePriorities();
    }

    private void Update() {
        if (sleeping) {
            if (currentTask == AvailableTasks.None && !_home._residentsCurrentlyInHome.Contains(transform)) {
                EnableResident();
                AddTask(AvailableTasks.Home);
                currentTask = AvailableTasks.Home;
                _navMeshAgent.destination = _home.entrance.position;
            }
        } else if (rioting) {
            if (currentTask != AvailableTasks.Riot) {
                EnableResident();
                currentTask = AvailableTasks.Riot;
                RemoveTask(0);

                Riot riot = FindObjectOfType<Riot>();
                if (riot != null) {
                    _navMeshAgent.destination = riot.transform.position;
                }
                else {
                    Vector3 riotPosition = _home.FindNextPathCrossroad(this);
                    Transform newRiot = Instantiate(riotPrefab, new Vector3(riotPosition.x + 0.5f, 0.05f, riotPosition.z + 0.5f), Quaternion.identity);
                    _navMeshAgent.destination = new Vector3(riotPosition.x + 0.5f, 0.05f, riotPosition.z + 0.5f);
                    newRiot.GetComponent<Riot>().riotingReasons = riotingReasons;
                }
            }
        } else {
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
        
        if (_navMeshAgent.velocity.x != 0f || _navMeshAgent.velocity.z != 0f) {
            _animator.SetFloat("Speed_f", 1f);
        }
        else {
            _animator.SetFloat("Speed_f", 0f);
        }
    }

    private void ExecuteTask() {
        EnableResident();
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
                if (_home.PathFromHomeAvailable(workplace.transform.position, this)) {
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
        Dictionary<AvailableTasks, float> needs = new Dictionary<AvailableTasks, float>();
        if (!workedToday && _home.PathFromHomeAvailable(workplace.transform.position, this)) {
            needs.Add(AvailableTasks.Work, (workPriority + Random.Range(-0.15f, 0.15f)) * 100);
        }
        if (_home.FindNextMarket(this) != null && !noFood) {
            needs.Add(AvailableTasks.Market, (100f - foodSatisfaction) * foodPriority + (foodSatisfaction < 25 ? 10 : Random.Range(-10, 10)));
        }
        if (_home.FindNextChurch(this) != null) {
            needs.Add(AvailableTasks.Church, (100f - religionSatisfaction) * religionPriority + (religionSatisfaction < 25 ? 10 : Random.Range(-10, 10)));
        }
        if (_home.FindNextTavern(this) != null) {
            needs.Add(AvailableTasks.Tavern, (100f - tavernSatisfaction) * tavernPriority + Random.Range(-10, 10));
        }

        AvailableTasks highestPriorityNeedName = AvailableTasks.None;
        float highestPriorityNeed = 0f;
        foreach (KeyValuePair<AvailableTasks, float> need in needs) {
            if (need.Value > highestPriorityNeed && need.Key != _previousCompletedTask) {
                highestPriorityNeedName = need.Key;
                highestPriorityNeed = need.Value;
            }
        }
        if (highestPriorityNeed < _lowestPriority * 100) {
            return _home._residentsCurrentlyInHome.Contains(transform) ? AvailableTasks.None : AvailableTasks.Home;
        }

        if (highestPriorityNeedName == AvailableTasks.None) {
            return _home._residentsCurrentlyInHome.Contains(transform) ? AvailableTasks.None : AvailableTasks.Home;
        } else {
            return highestPriorityNeedName;
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

    private void MeasureSatisfactions(object sender, EventArgs e) {
        _overallSatisfactionMeasurements.Add(satisfaction);
        _foodSatisfactionMeasurements.Add(foodSatisfaction);
        _religionSatisfactionMeasurements.Add(religionSatisfaction);
        _tavernSatisfactionMeasurements.Add(tavernSatisfaction);
        if (_timeController.daysSinceStart >= 1) {
            _overallSatisfactionMeasurements.RemoveAt(0);
            _foodSatisfactionMeasurements.RemoveAt(0);
            _religionSatisfactionMeasurements.RemoveAt(0);
            _tavernSatisfactionMeasurements.RemoveAt(0);

            float summedUpOverallMeasurements = 0f;
            float summedUpFoodMeasurements = 0f;
            float summedUpReligionMeasurements = 0f;
            float summedUpTavernMeasurements = 0f;
            for (int i = 0; i < _overallSatisfactionMeasurements.Count; i++) {
                summedUpOverallMeasurements += _overallSatisfactionMeasurements[i];
                summedUpFoodMeasurements += _foodSatisfactionMeasurements[i];
                summedUpReligionMeasurements += _religionSatisfactionMeasurements[i];
                summedUpTavernMeasurements += _tavernSatisfactionMeasurements[i];
            }

            _overallSatisfactionOverTime = summedUpOverallMeasurements / _overallSatisfactionMeasurements.Count;
            _foodSatisfactionOverTime = summedUpFoodMeasurements / _overallSatisfactionMeasurements.Count;
            _religionSatisfactionOverTime = summedUpReligionMeasurements / _overallSatisfactionMeasurements.Count;
            _tavernSatisfactionOverTime = summedUpTavernMeasurements / _overallSatisfactionMeasurements.Count;
        }
    }

    private void CalculateRiotRisk(object sender, EventArgs e) {
        if (!rioting && riotCooldown == 0) {
            List<string> reasonsToRiot = new List<string>();
            if (_foodSatisfactionOverTime * (1f - foodPriority) < 25) {
                reasonsToRiot.Add("food");
            }
            if (_religionSatisfactionOverTime * (1f - religionPriority) < 25) {
                reasonsToRiot.Add("religion");
            }
            if (_tavernSatisfactionOverTime * (1f - tavernPriority) < 25) {
                reasonsToRiot.Add("tavern");
            }
        
            if (reasonsToRiot.Count > 0 && _overallSatisfactionOverTime < 60) {
                int probabilityToRiot = Random.Range(reasonsToRiot.Count, 6);
                if (probabilityToRiot > 3) {
                    rioting = true;
                    riotingReasons.Clear();
                    riotingReasons = reasonsToRiot.ConvertAll(reason => reason);
                }
            }
            reasonsToRiot.Clear();
        }
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
    }

    public void RemoveTask(int index) {
        if (tasks.Count > 0) {
            tasks.RemoveAt(index);
        }
    }

    public void CompleteTask() {
        _previousCompletedTask = currentTask;
        currentTask = AvailableTasks.None;
        if (tasks.Count > 0) {
            tasks.RemoveAt(0);
        }
    }

    public void DisableResident() {
        // _navMeshAgent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        _navMeshAgent.enabled = false;
        _collider.enabled = false;
        skinnedMeshRenderer.enabled = false;
    }
    
    public void EnableResident() {
        // _navMeshAgent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        _navMeshAgent.enabled = true;
        _collider.enabled = true;
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
