using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Riot : MonoBehaviour {
    private GridBuildingSystem _gridBuildingSystem;
    [SerializeField] private Transform riotIndicator;
    [SerializeField] private Transform protestCircle;

    [HideInInspector] public List<Resident> residents = new List<Resident>();
    [HideInInspector] public bool movingRiot = false;
    
    [SerializeField] private Transform rioter;
    [SerializeField] private Transform riotTargetPrefab;
    private Transform _moveTarget;
    private readonly List<Transform> _riotingDummyResidents = new List<Transform>();

    [HideInInspector] public string riotCurrentStatus = "Protesting";
    public float riotProgress = 0f;
    public List<string> riotingReasons = new List<string>();

    private int _hoursSinceStart = 0;
    [SerializeField] private int minimumHourDuration = 5;

    private void Start() {
        _gridBuildingSystem = GridBuildingSystem.Instance;
        _gridBuildingSystem.OnBuildingPlaced += CheckReasonsToRiot;
        TimeController.Instance.OnNextHour += (obj, EventArgs) => { _hoursSinceStart++; };
    }

    // Update is called once per frame
    void Update() {
        riotIndicator.Rotate(0, 30 * Time.deltaTime, 0);
        if (!movingRiot) {
            transform.Rotate(0, 30 * Time.deltaTime, 0);
        } else {
            if (riotProgress < 100) {
                riotProgress += 1f * Time.deltaTime;
                riotProgress = Math.Clamp(riotProgress, 0f, 100f);
            } else if (riotProgress == 100f) {
                riotCurrentStatus = "Full blown revolution";
            }
        }
        
        if (riotingReasons.Count == 0 && _hoursSinceStart >= minimumHourDuration) {
            StopRiot();
            
        }
    }

    private void OnTriggerEnter(Collider other) {
        Resident resident = other.gameObject.GetComponent<Resident>();
        RiotingResident riotingResident = other.gameObject.GetComponent<RiotingResident>();
        if (resident && resident.currentTask == Resident.AvailableTasks.Riot) {
            resident.DisableResident();
            residents.Add(resident);
            if (residents.Count == 4) {
                protestCircle.gameObject.SetActive(false);
                for (int i = 0; i < 4; i++) {
                    AddNewRioterToMoving();
                }
                riotIndicator.parent = _riotingDummyResidents[0];
                movingRiot = true;
                _moveTarget = Instantiate(riotTargetPrefab, GetNextPosition(), Quaternion.identity, transform);
                foreach (Transform rioterTransform in _riotingDummyResidents) {
                    Vector2 destinationVariation = new Vector2(Random.Range(-0.4f, 0.4f), Random.Range(-0.4f, 0.4f));
                    rioterTransform.GetComponent<NavMeshAgent>().destination = 
                        new Vector3(_moveTarget.position.x + destinationVariation.x, _moveTarget.position.y, _moveTarget.position.z + destinationVariation.y);
                }

                riotCurrentStatus = "Riot intensifies...";
            } else if(residents.Count < 4) {
                protestCircle.GetChild(residents.Count - 1).gameObject.SetActive(true);
                protestCircle.GetChild(residents.Count - 1).GetComponent<Animator>()
                    .SetBool("Protesting", true);
                riotIndicator.gameObject.SetActive(true);
            } else {
                AddNewRioterToMoving();
            }
        } else if (riotingResident) {
            Vector2 destinationVariation = new Vector2(Random.Range(-0.4f, 0.4f), Random.Range(-0.4f, 0.4f));
            riotingResident.GetComponent<NavMeshAgent>().destination = 
                    new Vector3(_moveTarget.position.x + destinationVariation.x, _moveTarget.position.y, _moveTarget.position.z + destinationVariation.y);
        }
    }
    
    private void CheckReasonsToRiot(string buildingType) {
        switch (buildingType) {
            case "Market":
                riotingReasons.Remove("food");
                break;
            case "Church":
                riotingReasons.Remove("religion");
                break;
            default:
                riotingReasons.Remove(buildingType.ToLower());
                break;
        }
    }

    private Vector3 GetNextPosition() {
        RaycastHit[] hitsAround = Physics.BoxCastAll(transform.position, new Vector3(7.5f, 7.5f, 7.5f), transform.up);
        RaycastHit pathFurthestAway = hitsAround[0];
        foreach (RaycastHit hit in hitsAround) {
            if (hit.transform.CompareTag("Path") && hit.distance > pathFurthestAway.distance && IsPathAvailable(hit.transform.position)) {
                pathFurthestAway = hit;
            }
        }
        Vector3 pathPosition = pathFurthestAway.transform.position;
        switch (pathFurthestAway.transform.eulerAngles.y) {
            case 0:
            default:
                return new Vector3(pathPosition.x + 0.5f, pathPosition.y + 0.5f, pathPosition.z + 0.5f);
            case 90:
                return new Vector3(pathPosition.x + 0.5f, pathPosition.y + 0.5f, pathPosition.z - 0.5f);
            case 180:
                return new Vector3(pathPosition.x - 0.5f, pathPosition.y + 0.5f, pathPosition.z - 0.5f);
            case 270:
                return new Vector3(pathPosition.x - 0.5f, pathPosition.y + 0.5f, pathPosition.z + 0.5f);
        }
    }

    private bool IsPathAvailable(Vector3 destination) {
        NavMeshPath Path = new NavMeshPath();
        NavMeshAgent anyResidentAgent = _riotingDummyResidents[0].GetComponent<NavMeshAgent>();
        NavMesh.CalculatePath(transform.position, destination, anyResidentAgent.areaMask, Path);
        return Path.status == NavMeshPathStatus.PathComplete;
    }

    private void AddNewRioterToMoving() {
        Transform newRiotingResident = Instantiate(rioter, transform.position,
            Quaternion.identity, transform);
        _riotingDummyResidents.Add(newRiotingResident);
        foreach (Transform riotingResident in _riotingDummyResidents) {
            riotingResident.GetComponent<RiotingResident>().riotRange = _riotingDummyResidents.Count;
        }
        StartCoroutine(StartAsyncAnimation(newRiotingResident.GetChild(0)));
    }

    private IEnumerator StartAsyncAnimation(Transform riotingResident) {
        yield return new WaitForSeconds(Random.Range(0f, 1f));
        riotingResident.GetComponent<Animator>().SetBool("Protesting", true);
    }

    private void StopRiot() {
        foreach (Resident resident in residents) {
            resident.riotCooldown = 16;
        }
        if (!movingRiot) {
            for (int i = 0; i < residents.Count; i++) {
                residents[i].transform.position = protestCircle.GetChild(i).transform.position;
                residents[i].transform.rotation = protestCircle.GetChild(i).transform.rotation;
                residents[i].EnableResident();
                residents[i].rioting = false;
                residents[i].currentTask = Resident.AvailableTasks.None;
            }
        } else {
            for (int i = 0; i < residents.Count; i++) {
                residents[i].transform.position = _riotingDummyResidents[i].transform.position;
                residents[i].transform.rotation = _riotingDummyResidents[i].transform.rotation;
                residents[i].EnableResident();
                residents[i].rioting = false;
                residents[i].currentTask = Resident.AvailableTasks.None;
            }
        }

        UIControl uiControl = UIControl.Instance;
        if (uiControl.currentlyInspectedType == "Riot") {
            uiControl.HideInspector();
        }
        Destroy(gameObject);
    }
}
