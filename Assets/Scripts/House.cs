using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class House : MonoBehaviour, IBuilding {
    [SerializeField] private List<Transform> residents;
    public List<Transform> _residentsCurrentlyInHome;
    [SerializeField] private Transform residentPrefab;
    [SerializeField] public Transform entrance;

    public Transform nextChurch;
    [SerializeField] private Transform sleepingIndicator;
    
    // Start is called before the first frame update
    void Start() {
        
    }

    private void Update() {
        if (sleepingIndicator.gameObject.activeSelf) {
            sleepingIndicator.GetChild(0).Rotate(0, 60 * Time.deltaTime, 0);
            sleepingIndicator.GetChild(1).Rotate(0, 60 * Time.deltaTime, 0);
            sleepingIndicator.GetChild(2).Rotate(0, 60 * Time.deltaTime, 0);
        }

        if (_residentsCurrentlyInHome.Any(resident => resident.GetComponent<Resident>().sleeping)) {
            sleepingIndicator.gameObject.SetActive(true);
        }
        else {
            sleepingIndicator.gameObject.SetActive(false);
        }
    }

    public void BuildingPlaced() {
        Transform residentTransform = Instantiate(residentPrefab, entrance.position, Quaternion.identity);
        residents.Add(residentTransform);
        _residentsCurrentlyInHome.Add(residentTransform);
        Resident resident = residentTransform.GetComponent<Resident>();
        resident.ResidentConstructor(this);
    }
    
    public void ResidentEnter(Collider other) {
        Resident resident = other.GetComponent<Resident>();
        if (residents.Contains(resident.transform) && (resident.currentTask == Resident.AvailableTasks.Home || resident.sleeping)) {
            _residentsCurrentlyInHome.Add(resident.transform);
            resident.DisableVisual();
            resident.CompleteTask();
        }
    }

    public void ResidentLeave(Collider other) {
        Resident resident = other.GetComponent<Resident>();
        _residentsCurrentlyInHome.Remove(resident.transform);
        resident.EnableVisual();
    }

    public Transform FindNextChurch(Resident resident) {
        NavMeshAgent anyResidentAgent = resident.GetComponent<NavMeshAgent>();
        Church closestChurch = null;
        float closestTargetDistance = float.MaxValue;
        NavMeshPath Path = new NavMeshPath();
        foreach(Church church in FindObjectsOfType<Church>()) {
            Transform churchTransform = church.transform;
            if (NavMesh.CalculatePath(entrance.transform.position, church.entrance.position, anyResidentAgent.areaMask, Path)) {
                float distanceToChurch = Vector3.Distance(transform.position, Path.corners[0]);
                for (int i = 1; i < Path.corners.Length; i++) {
                    distanceToChurch += Vector3.Distance(Path.corners[i - 1], Path.corners[i]);
                }
                Debug.Log("DistanceToChurch" + distanceToChurch);
                if (distanceToChurch < Church.ChurchRange && distanceToChurch < closestTargetDistance) {
                    closestTargetDistance = distanceToChurch;
                    closestChurch = church;
                }
            }
        }
        return closestChurch.entrance;
    }

    public bool PathFromHomeAvailable(Transform destination, Resident resident) {
        NavMeshPath Path = new NavMeshPath();
        NavMeshAgent anyResidentAgent = resident.GetComponent<NavMeshAgent>();
        return NavMesh.CalculatePath(entrance.transform.position, destination.position, anyResidentAgent.areaMask, Path);
    }
}
