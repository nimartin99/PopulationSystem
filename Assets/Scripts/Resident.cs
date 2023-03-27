using System;
using System.Collections.Generic;
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
    private bool _drowsy;
    public AvailableTasks currentTask = AvailableTasks.None;
    [SerializeField] public List<AvailableTasks> tasks;
    private NavMeshAgent _navMeshAgent;
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;
    private CapsuleCollider _collider;
    private House _home;
    [SerializeField] private Transform positionIndicator;
    [SerializeField] private Transform drowsyIndicator;
    [SerializeField] private Workplace workplace;
    private AvailableTasks _previousCompletedTask = AvailableTasks.None;
    
    // Animation
    [SerializeField] private Animator animator;

    public float satisfaction = 0f;
    public float happiness = 0f;
    public float religionSatisfaction = 0f;
    public float foodSatisfaction = 0f;
    public float tavernSatisfaction = 0f;
    private TimeController _timeController;
    private UIControl _uiControl;

    // Priorities
    public float lowestPriority = 1f;
    // Overall priority
    public float workPriority;
    // Needs
    public float foodPriority;
    public float religionPriority;
    // Happiness
    public float tavernPriority;
    public string role = "";

    public List<float> overallSatisfactionMeasurements = new List<float>();
    public List<float> foodSatisfactionMeasurements = new List<float>();
    public List<float> religionSatisfactionMeasurements = new List<float>();
    public List<float> tavernSatisfactionMeasurements = new List<float>();

    public float averageOverallSatisfaction = 100f;
    public float averageFoodSatisfaction = 100f;
    public float averageReligionSatisfaction = 100f;
    public float averageTavernSatisfaction = 100f;
    
    // Riot
    public List<string> riotingReasons = new List<string>();
    public bool rioting;
    [SerializeField] private Transform riotPrefab;
    [HideInInspector] public int riotCooldown = 16;

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
            if (currentTask == AvailableTasks.None && !_home.residentsCurrentlyInHome.Contains(transform)) {
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
        } else if(currentTask == AvailableTasks.None && !_drowsy) {
            if (tasks.Count > 0) {
                ExecuteTask();
            } else if (tasks.Count == 0) {
                AvailableTasks nextTask = GetNextTask();
                if (nextTask == AvailableTasks.None) {
                    currentTask = AvailableTasks.None;
                } else {
                    AddTask(nextTask);
                }
            }
        }

        if (currentTask != AvailableTasks.None && _navMeshAgent.hasPath && _navMeshAgent.pathStatus == NavMeshPathStatus.PathPartial) {
            CantCompleteTask();
        }
        // Decrease satisfactions
        CalculateSatisfactions();
        // Rotate indicator
        positionIndicator.Rotate(0, 60 * Time.deltaTime, 0);
        if (drowsyIndicator.gameObject.activeSelf) {
            drowsyIndicator.GetChild(0).Rotate(0, 60 * Time.deltaTime, 0);
            drowsyIndicator.GetChild(1).Rotate(0, 60 * Time.deltaTime, 0);
            drowsyIndicator.GetChild(2).Rotate(0, 60 * Time.deltaTime, 0);
        }
        
        if (_navMeshAgent.velocity.x != 0f || _navMeshAgent.velocity.z != 0f) {
            animator.SetFloat("Speed_f", 1f);
        }
        else {
            animator.SetFloat("Speed_f", 0f);
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

    private AvailableTasks GetNextTask() {
        Dictionary<AvailableTasks, float> needs = new Dictionary<AvailableTasks, float>();
        if (!workedToday && _home.PathFromHomeAvailable(workplace.transform.position, this)) {
            needs.Add(AvailableTasks.Work, (workPriority + Random.Range(-0.15f, 0.15f)) * 100);
        }
        if (_home.FindNextMarket(this) != null && _uiControl.currentFoodPortion != "No Food") {
            needs.Add(AvailableTasks.Market, (100f - foodSatisfaction) * foodPriority + (foodSatisfaction < 25 ? 10 : Random.Range(-10, 10)));
        }
        if (_home.FindNextChurch(this) != null) {
            needs.Add(AvailableTasks.Church, (100f - religionSatisfaction) * religionPriority + (religionSatisfaction < 25 ? 10 : Random.Range(-10, 10)));
        }
        if (_home.FindNextTavern(this) != null) {
            needs.Add(AvailableTasks.Tavern, (100f - tavernSatisfaction) * tavernPriority + Random.Range(-10, 10));
        }

        AvailableTasks mostImportantNeedTask = AvailableTasks.None;
        float mostImportantNeedValue = 0f;
        foreach (KeyValuePair<AvailableTasks, float> need in needs) {
            if (need.Value > mostImportantNeedValue && need.Key != _previousCompletedTask) {
                mostImportantNeedTask = need.Key;
                mostImportantNeedValue = need.Value;
            }
        }
        if (mostImportantNeedValue < lowestPriority * 100) {
            return _home.residentsCurrentlyInHome.Contains(transform) ? AvailableTasks.None : AvailableTasks.Home;
        }

        if (mostImportantNeedTask == AvailableTasks.None) {
            return _home.residentsCurrentlyInHome.Contains(transform) ? AvailableTasks.None : AvailableTasks.Home;
        } else {
            return mostImportantNeedTask;
        }
    }
    
    private void CalculateSatisfactions() {
        float divide = sleeping ? 2 : 1;
        
        if (religionSatisfaction > 0) {
            religionSatisfaction -= Time.deltaTime * 0.3f / divide;
            if (religionSatisfaction < 0) {
                religionSatisfaction = 0f;
            }
        }
        if (foodSatisfaction > 0) {
            foodSatisfaction -= Time.deltaTime * (0.75f - (role == "Religious" && religionSatisfaction > 65 ? 0.25f : 0f)) / divide;
            if (foodSatisfaction < 0) {
                foodSatisfaction = 0f;
            }
        }
        if (tavernSatisfaction > 0) {
            tavernSatisfaction -= Time.deltaTime * (0.2f - (role == "Religious" && religionSatisfaction > 65 ? 0.1f : 0f)) / divide;
            if (tavernSatisfaction < 0) {
                tavernSatisfaction = 0f;
            }
        }

        satisfaction = foodSatisfaction * 0.6f + religionSatisfaction * 0.4f;
        happiness = (100f - _uiControl.currentTaxes) * 0.3f + (8 - _timeController.workHours) / 8f * 0.3f * 100f + tavernSatisfaction * 0.4f;
    }

    private void MeasureSatisfactions(object sender, EventArgs e) {
        overallSatisfactionMeasurements.Add(satisfaction);
        foodSatisfactionMeasurements.Add(foodSatisfaction);
        religionSatisfactionMeasurements.Add(religionSatisfaction);
        tavernSatisfactionMeasurements.Add(tavernSatisfaction);
        if (overallSatisfactionMeasurements.Count >= 25) {
            overallSatisfactionMeasurements.RemoveAt(0);
            foodSatisfactionMeasurements.RemoveAt(0);
            religionSatisfactionMeasurements.RemoveAt(0);
            tavernSatisfactionMeasurements.RemoveAt(0);

            float summedUpOverallMeasurements = 0f;
            float summedUpFoodMeasurements = 0f;
            float summedUpReligionMeasurements = 0f;
            float summedUpTavernMeasurements = 0f;
            for (int i = 0; i < overallSatisfactionMeasurements.Count; i++) {
                summedUpOverallMeasurements += overallSatisfactionMeasurements[i];
                summedUpFoodMeasurements += foodSatisfactionMeasurements[i];
                summedUpReligionMeasurements += religionSatisfactionMeasurements[i];
                summedUpTavernMeasurements += tavernSatisfactionMeasurements[i];
            }

            averageOverallSatisfaction = summedUpOverallMeasurements / overallSatisfactionMeasurements.Count;
            averageFoodSatisfaction = summedUpFoodMeasurements / overallSatisfactionMeasurements.Count;
            averageReligionSatisfaction = summedUpReligionMeasurements / overallSatisfactionMeasurements.Count;
            averageTavernSatisfaction = summedUpTavernMeasurements / overallSatisfactionMeasurements.Count;
        }
    }

    private void CalculateRiotRisk(object sender, EventArgs e) {
        if (!rioting && riotCooldown == 0) {
            List<string> reasonsToRiot = new List<string>();
            if (averageFoodSatisfaction * (1f - foodPriority) < 25) {
                reasonsToRiot.Add("food");
            }
            if (averageReligionSatisfaction * (1f - religionPriority) < 25) {
                reasonsToRiot.Add("religion");
            }
            if (averageTavernSatisfaction * (1f - tavernPriority) < 25) {
                reasonsToRiot.Add("tavern");
            }
        
            if (reasonsToRiot.Count > 0 && averageOverallSatisfaction < 40) {
                int probabilityToRiot = Random.Range(0, 10 - reasonsToRiot.Count);
                if (probabilityToRiot < 4) {
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
        sleeping = TimeController.Instance.sleepTime;
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
        _navMeshAgent.enabled = false;
        _collider.enabled = false;
        skinnedMeshRenderer.enabled = false;
        DisableIndicator();
    }
    
    public void EnableResident() {
        drowsyIndicator.gameObject.SetActive(false);
        _navMeshAgent.enabled = true;
        _collider.enabled = true;
        skinnedMeshRenderer.enabled = true;
        if (_uiControl.currentlyInspectedResident == this) {
            EnableIndicator();
        }
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
        List<float> priorities = new List<float>();
        float priority = Random.Range(0.15f, 0.35f);
        float overallPriority = 1f - priority;
        priorities.Add(priority);
        priority = Random.Range(overallPriority * 0.33f, overallPriority * 0.66f);
        priorities.Add(priority);
        overallPriority -= priority;
        priority = Random.Range(overallPriority * 0.33f, overallPriority * 0.66f);
        priorities.Add(priority);
        overallPriority -= priority;
        priority = overallPriority;
        priorities.Add(priority);
        
        foreach (float prio in priorities) {
            if (lowestPriority > prio) {
                lowestPriority = prio;
            }
        }

        List<string> necessities = new List<string>();
        necessities.Add("work");
        necessities.Add("food");
        necessities.Add("religion");
        necessities.Add("tavern");
        for (int i = Random.Range(0, priorities.Count - 1); priorities.Count > 0; i = Random.Range(0, priorities.Count - 1)) {
            int j = Random.Range(0, necessities.Count - 1);
            switch (necessities[j]) {
                case "work": workPriority = priorities[i]; break;
                case "food": foodPriority = priorities[i]; break;
                case "religion": religionPriority = priorities[i]; break;
                case "tavern": tavernPriority = priorities[i]; break;
            }
            necessities.RemoveAt(j);
            priorities.RemoveAt(i);
        }

        const float roleThreshold = 0.32f;
        if (workPriority > foodPriority && workPriority > religionPriority && workPriority > tavernPriority) {
            if (workPriority > roleThreshold) {
                role = "Workaholic";
            }
        } else if (foodPriority > workPriority && foodPriority > religionPriority && foodPriority > tavernPriority) {
            if (foodPriority > roleThreshold) {
                role = "Glutton";
                _navMeshAgent.speed = 1.25f;
            }
        } else if (religionPriority > workPriority && religionPriority > foodPriority && religionPriority > tavernPriority) {
            if (religionPriority > roleThreshold) {
                role = "Religious";
            }
        } else if (tavernPriority > workPriority && tavernPriority > religionPriority && tavernPriority > foodPriority) {
            if (tavernPriority > roleThreshold) {
                role = "Alcoholic";
            }
        }

        foodSatisfaction = Random.Range(20, 101);
        religionSatisfaction = Random.Range(0, 101);
        tavernSatisfaction = Random.Range(0, 101);
    }

    public void Drunk() {
        InvokeRepeating(nameof(RandomDrowsy), 1f, 3f);
    }

    private void RandomDrowsy() {
        if (tavernSatisfaction < 60f) {
            CancelInvoke(nameof(RandomDrowsy));
        } else {
            int rndNumber = Random.Range(0, 4);
            if (rndNumber <= 1 && _navMeshAgent.enabled) {
                CantCompleteTask();
                _navMeshAgent.enabled = false;
                drowsyIndicator.gameObject.SetActive(true);
                _drowsy = true;
                CancelInvoke(nameof(RandomDrowsy));
                Invoke(nameof(StopDrowsy), 10);
            }
        }
    }

    private void StopDrowsy() {
        _drowsy = false;
        EnableResident();
    }

    private void OnDestroy() {
        _timeController.OnNextHour -= MeasureSatisfactions;
        _timeController.OnNextHour -= CalculateRiotRisk;
        _timeController.OnSleepTimeStart -= EnterSleepMode;
        _timeController.OnSleepTimeEnd -= StopSleepMode;
    }
}
