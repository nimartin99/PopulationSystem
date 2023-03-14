using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class House : MonoBehaviour, IBuilding {
    [SerializeField] public List<Resident> residents;
    public List<Transform> residentsCurrentlyInHome;
    [SerializeField] private Transform residentPrefab;
    [SerializeField] public Transform entrance;

    [SerializeField] private Transform positionIndicator;
    [SerializeField] private Transform sleepingIndicator;
    [SerializeField] private Transform upgradeIndicator;
    private GridBuildingSystem _gridBuildingSystem;

    private TimeController _timeController;
    private UIControl _uiControl;
    [HideInInspector] public float allResidentsSatisfaction = 0f;
    [HideInInspector] public float allResidentsHappiness = 0f;
    [SerializeField] private float upgradeThreshold;
    [HideInInspector] public bool upgradeable = false;

    [SerializeField] private Transform farmerGenModel;
    [SerializeField] private List<Transform> settlerGenModel = new List<Transform>();

// Start is called before the first frame update
    void Start() {
        _gridBuildingSystem = GridBuildingSystem.Instance;
        _timeController = TimeController.Instance;
        _uiControl = UIControl.Instance;
        _timeController.OnNextHour += CalculateAllResidentsValuesOverTime;
        _uiControl.OnModeSwitch += UpgradeVisual;
    }

    private void Update() {
        float sumSatisfaction = 0f;
        float sumHappiness = 0f;
        
        foreach (Resident resident in residents) {
            sumSatisfaction += resident.satisfaction;
            sumHappiness += resident.happiness;
        }

        allResidentsSatisfaction = sumSatisfaction / residents.Count;
        allResidentsHappiness = sumHappiness / residents.Count;
        
        if (sleepingIndicator.gameObject.activeSelf) {
            sleepingIndicator.GetChild(0).Rotate(0, 60 * Time.deltaTime, 0);
            sleepingIndicator.GetChild(1).Rotate(0, 60 * Time.deltaTime, 0);
            sleepingIndicator.GetChild(2).Rotate(0, 60 * Time.deltaTime, 0);
        }
        positionIndicator.Rotate(0, 60 * Time.deltaTime, 0);
        upgradeIndicator.Rotate(0, 60 * Time.deltaTime, 0);
        
        if (residentsCurrentlyInHome.Any(resident => resident.GetComponent<Resident>().sleeping) 
            && !upgradeIndicator.gameObject.activeSelf && !_uiControl.currentlyInspectedHouse == this) {
            sleepingIndicator.gameObject.SetActive(true);
        } else {
            sleepingIndicator.gameObject.SetActive(false);
        }
        
    }

    public void UpgradeToNextGen() {
        farmerGenModel.gameObject.SetActive(false);
        settlerGenModel[Random.Range(0, settlerGenModel.Count)].gameObject.SetActive(true);
    }

    public void BuildingPlaced() {
        Transform residentTransform = Instantiate(residentPrefab, entrance.position, Quaternion.identity);
        Resident resident = residentTransform.GetComponent<Resident>();
        residents.Add(resident);
        residentsCurrentlyInHome.Add(residentTransform);
        resident.ResidentConstructor(this);
    }

    public void BuildingDestroyed() {
        foreach (Resident resident in residents) {
            Destroy(resident.gameObject);
        }
    }

    public void ResidentEnter(Resident resident) {
        if (resident && residents.Contains(resident) && (resident.currentTask == Resident.AvailableTasks.Home || resident.sleeping)) {
            residentsCurrentlyInHome.Add(resident.transform);
            resident.DisableResident();
            resident.CompleteTask();
        }
    }

    public void ResidentLeave(Resident resident) {
        if (resident) {
            residentsCurrentlyInHome.Remove(resident.transform);
        }
    }

    public Transform FindNextChurch(Resident resident) {
        Church closestChurch = null;
        float closestTargetDistance = float.MaxValue;
        NavMeshPath path = new NavMeshPath();
        foreach(Church church in FindObjectsOfType<Church>()) {
            if (PathFromHomeAvailable(church.entrance.position, resident, path)) {
                float distanceToChurch = Vector3.Distance(entrance.transform.position, path.corners[0]);
                for (int i = 1; i < path.corners.Length; i++) {
                    distanceToChurch += Vector3.Distance(path.corners[i - 1], path.corners[i]);
                }
                if (distanceToChurch < Church.ChurchRange && distanceToChurch < closestTargetDistance) {
                    closestTargetDistance = distanceToChurch;
                    closestChurch = church;
                }
            }
        }
        return closestChurch ? closestChurch.entrance : null;
    }
    
    public Transform FindNextMarket(Resident resident) {
        Market closestMarket = null;
        float closestTargetDistance = float.MaxValue;
        NavMeshPath path = new NavMeshPath();
        foreach(Market market in FindObjectsOfType<Market>()) {
            if (PathFromHomeAvailable(market.entrance.position, resident, path)) {
                float distanceToMarket = Vector3.Distance(entrance.transform.position, path.corners[0]);
                for (int i = 1; i < path.corners.Length; i++) {
                    distanceToMarket += Vector3.Distance(path.corners[i - 1], path.corners[i]);
                }
                if (distanceToMarket < Market.MarketRange && distanceToMarket < closestTargetDistance) {
                    closestTargetDistance = distanceToMarket;
                    closestMarket = market;
                }
            }
        }
        return closestMarket ? closestMarket.entrance : null;
    }
    
    public Transform FindNextTavern(Resident resident) {
        Tavern closestTavern = null;
        float closestTargetDistance = float.MaxValue;
        NavMeshPath path = new NavMeshPath();
        foreach(Tavern tavern in FindObjectsOfType<Tavern>()) {
            if (PathFromHomeAvailable(tavern.entrance.position, resident, path)) {
                float distanceToTavern = Vector3.Distance(entrance.transform.position, path.corners[0]);
                for (int i = 1; i < path.corners.Length; i++) {
                    distanceToTavern += Vector3.Distance(path.corners[i - 1], path.corners[i]);
                }
                if (distanceToTavern < Tavern.TavernRange && distanceToTavern < closestTargetDistance) {
                    closestTargetDistance = distanceToTavern;
                    closestTavern = tavern;
                }
            }
        }
        
        return closestTavern ? closestTavern.entrance : null;
    }

    public bool PathFromHomeAvailable(Vector3 destination, Resident resident, NavMeshPath path = null) {
        if (path == null) {
            path = new NavMeshPath();
        }
        NavMeshAgent anyResidentAgent = resident.GetComponent<NavMeshAgent>();
        NavMesh.CalculatePath(entrance.transform.position, destination, anyResidentAgent.areaMask, path);
        return path.status == NavMeshPathStatus.PathComplete;
    }

    public Vector3 FindNextPathCrossroad(Resident resident) {
        Vector3 topLeftPosition = new Vector3(0, 0 , 0);
        Vector3 transformPosition = transform.position;
        switch (transform.eulerAngles.y) {
            case 0:
                topLeftPosition = new Vector3(transformPosition.x, 0, transformPosition.z + 1);
                break;
            case 90:
                topLeftPosition = new Vector3(transformPosition.x, 0, transformPosition.z - 1);
                break;
            case 180:
                topLeftPosition = new Vector3(transformPosition.x - 2, 0, transformPosition.z - 1);
                break;
            case 270:
                topLeftPosition = new Vector3(transformPosition.x - 2, 0, transformPosition.z + 1);
                break;
        }

        Vector3 bestCrossroad = new Vector3(topLeftPosition.x - 1, 0, topLeftPosition.z + 1);
        int bestCrossroadDirections = 0;
        
        for (int i = 0; i < 10; i++) {
            Vector3 currentSquare = new Vector3(topLeftPosition.x - 1 - 1 * i, 0, topLeftPosition.z + 1 + 1 * i);;
            for (int j = 0; j < 4 + 2 * i; j++) {
                Vector3 topRowPosition = new Vector3(currentSquare.x + j, 0, currentSquare.z);
                GridBuildingSystem.GridObject topRowGridObject =
                    _gridBuildingSystem._grid.GetGridObject(topRowPosition);
                if (topRowGridObject != null && topRowGridObject.GetPlacedObject() != null && topRowGridObject.GetPlacedObjectType() == "Path") {
                    int thisPathDirections = CountNeighbourPaths(topRowPosition);
                    if (thisPathDirections > bestCrossroadDirections && 
                        PathFromHomeAvailable(new Vector3(topRowPosition.x + 0.5f, topRowPosition.y, topRowPosition.z + 0.5f), resident)) {
                        if (thisPathDirections == 4) {
                            return topRowPosition;
                        }
                        bestCrossroad = topRowPosition;
                        bestCrossroadDirections = thisPathDirections;
                    }
                }
                
                Vector3 bottomRowPosition = new Vector3(currentSquare.x + j, 0, currentSquare.z - 3 - i * 2);
                GridBuildingSystem.GridObject bottomRowGridObject =
                    _gridBuildingSystem._grid.GetGridObject(bottomRowPosition);
                if (bottomRowGridObject != null && bottomRowGridObject.GetPlacedObject() != null && bottomRowGridObject.GetPlacedObjectType() == "Path") {
                    int thisPathDirections = CountNeighbourPaths(bottomRowPosition);
                    if (thisPathDirections > bestCrossroadDirections && 
                        PathFromHomeAvailable(new Vector3(bottomRowPosition.x + 0.5f, bottomRowPosition.y, bottomRowPosition.z + 0.5f), resident)) {
                        if (thisPathDirections == 4) {
                            return bottomRowPosition;
                        }
                        bestCrossroad = bottomRowPosition;
                        bestCrossroadDirections = thisPathDirections;
                    }
                }

                if (j > 0 && j < 3 + 2 * i) {
                    Vector3 leftColumnPosition = new Vector3(currentSquare.x, 0, currentSquare.z - j);
                    GridBuildingSystem.GridObject leftColumnGridObject =
                        _gridBuildingSystem._grid.GetGridObject(leftColumnPosition);
                    if (leftColumnGridObject != null && leftColumnGridObject.GetPlacedObject() != null && leftColumnGridObject.GetPlacedObjectType() == "Path") {
                        int thisPathDirections = CountNeighbourPaths(leftColumnPosition);
                        if (thisPathDirections > bestCrossroadDirections && 
                            PathFromHomeAvailable(new Vector3(leftColumnPosition.x + 0.5f, leftColumnPosition.y, leftColumnPosition.z + 0.5f), resident)) {
                            if (thisPathDirections == 4) {
                                return leftColumnPosition;
                            }
                            bestCrossroad = leftColumnPosition;
                            bestCrossroadDirections = thisPathDirections;
                        }
                    }
                
                    Vector3 rightColumnPosition = new Vector3(currentSquare.x + 3 + 2 * i, 0, currentSquare.z - j);
                    GridBuildingSystem.GridObject rightColumnGridObject =
                        _gridBuildingSystem._grid.GetGridObject(rightColumnPosition);
                    if (rightColumnGridObject != null && rightColumnGridObject.GetPlacedObject() != null && rightColumnGridObject.GetPlacedObjectType() == "Path") {
                        int thisPathDirections = CountNeighbourPaths(rightColumnPosition);
                        if (thisPathDirections > bestCrossroadDirections && 
                            PathFromHomeAvailable(new Vector3(rightColumnPosition.x + 0.5f, rightColumnPosition.y, rightColumnPosition.z + 0.5f), resident)) {
                            if (thisPathDirections == 4) {
                                return rightColumnPosition;
                            }
                            bestCrossroad = rightColumnPosition;
                            bestCrossroadDirections = thisPathDirections;
                        }
                    }
                }
            }
        }
        
        return bestCrossroad;
    }

    private int CountNeighbourPaths(Vector3 pos) {
        int thisPathDirections = 0;
        if (_gridBuildingSystem._grid.GetGridObject(new Vector3(pos.x + 1, pos.y, pos.z)) != null &&
            _gridBuildingSystem._grid.GetGridObject(new Vector3(pos.x + 1, pos.y, pos.z)).GetPlacedObjectType() != null &&
            _gridBuildingSystem._grid.GetGridObject(new Vector3(pos.x + 1, pos.y, pos.z)).GetPlacedObjectType() == "Path") {
            thisPathDirections++;
        }
        if (_gridBuildingSystem._grid.GetGridObject(new Vector3(pos.x - 1, pos.y, pos.z)) != null &&
            _gridBuildingSystem._grid.GetGridObject(new Vector3(pos.x - 1, pos.y, pos.z)).GetPlacedObjectType() != null &&
            _gridBuildingSystem._grid.GetGridObject(new Vector3(pos.x - 1, pos.y, pos.z)).GetPlacedObjectType() == "Path") {
            thisPathDirections++;
        }
        if (_gridBuildingSystem._grid.GetGridObject(new Vector3(pos.x, pos.y, pos.z + 1)) != null &&
            _gridBuildingSystem._grid.GetGridObject(new Vector3(pos.x, pos.y, pos.z + 1)).GetPlacedObjectType() != null &&
            _gridBuildingSystem._grid.GetGridObject(new Vector3(pos.x, pos.y, pos.z + 1)).GetPlacedObjectType() == "Path") {
            thisPathDirections++;
        }
        if (_gridBuildingSystem._grid.GetGridObject(new Vector3(pos.x, pos.y, pos.z - 1)) != null &&
            _gridBuildingSystem._grid.GetGridObject(new Vector3(pos.x, pos.y, pos.z - 1)).GetPlacedObjectType() != null &&
            _gridBuildingSystem._grid.GetGridObject(new Vector3(pos.x, pos.y, pos.z - 1)).GetPlacedObjectType() == "Path") {
            thisPathDirections++;
        }

        return thisPathDirections;
    }

    private void CalculateAllResidentsValuesOverTime(object sender, EventArgs args) {
        float sumSatisfactionOverTime = 0f;
        foreach (Resident resident in residents) {
            sumSatisfactionOverTime += resident.averageOverallSatisfaction;
        }
        upgradeable = sumSatisfactionOverTime / residents.Count > upgradeThreshold;
    }
    
    public void EnableIndicator() {
        positionIndicator.gameObject.SetActive(true);
    }
    
    public void DisableIndicator() {
        positionIndicator.gameObject.SetActive(false);
    }

    private void UpgradeVisual(InputControl.InputModes type) {
        if (type == InputControl.InputModes.UpgradeMode && upgradeable) {
            DisableIndicator();
            upgradeIndicator.gameObject.SetActive(true);
        } else {
            upgradeIndicator.gameObject.SetActive(false);
        }
    }
}
