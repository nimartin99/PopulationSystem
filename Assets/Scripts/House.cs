using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class House : MonoBehaviour, IBuilding {
    [SerializeField] private List<Transform> residents;
    public List<Transform> _residentsCurrentlyInHome;
    [SerializeField] private Transform residentPrefab;
    [SerializeField] public Transform entrance;

    public Transform nextChurch;
    
    // Start is called before the first frame update
    void Start() {
        
    }

    private void Update() {
        
    }

    public void BuildingPlaced() {
        Transform residentTransform = Instantiate(residentPrefab, entrance.position, Quaternion.identity);
        residents.Add(residentTransform);
        Resident resident = residentTransform.GetComponent<Resident>();
        resident.ResidentConstructor(this);
    }
    
    public void ResidentEnter(Collider other) {
        Resident resident = other.GetComponent<Resident>();
        if (residents.Contains(resident.transform) && resident.currentTask == Resident.AvailableTasks.Home) {
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

    public Transform FindNextChurch() {
        NavMeshAgent anyResidentAgent = residents[0].GetComponent<NavMeshAgent>();
        Transform closestChurch = null;
        float closestTargetDistance = float.MaxValue;
        NavMeshPath Path = new NavMeshPath();
        foreach(GameObject church in FindObjectsOfType(typeof(Church))) {
            Transform churchTransform = church.transform;
            if (NavMesh.CalculatePath(transform.position, churchTransform.position, anyResidentAgent.areaMask, Path)) {
                float distanceToChurch = Vector3.Distance(transform.position, Path.corners[0]);
                for (int i = 1; i < Path.corners.Length; i++) {
                    distanceToChurch += Vector3.Distance(Path.corners[i - 1], Path.corners[i]);
                }

                if (distanceToChurch < Church.ChurchRange && distanceToChurch < closestTargetDistance) {
                    closestTargetDistance = distanceToChurch;
                    closestChurch = church.transform;
                }
            }
        }
        return closestChurch;
    }
}
