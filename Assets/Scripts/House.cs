using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House : MonoBehaviour, IBuilding
{
    [SerializeField] private List<Transform> residents;
    [SerializeField] private Transform residentPrefab;
    [SerializeField] public Transform entrance;
    
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
        resident.OnTaskAdded += ResidentsTaskAdded;
    }
    
    public void ResidentEnter(Collider other) {
        Resident resident = other.GetComponent<Resident>();
        if ((residents.Contains(resident.transform) && resident.tasks.Count == 0) ||
            (resident.tasks.Count > 0 && resident.tasks[0] == entrance)) {
            resident.DisableVisual();
        }
    }

    private void ResidentsTaskAdded(object sender, EventArgs e) {
        foreach (Transform residentTransform in residents) {
            Resident resident = residentTransform.GetComponent<Resident>();
            bool isLeaving = resident.tasks.Count > 0 && !(resident.tasks.Count == 1 && resident.tasks[0] == entrance);
            if (isLeaving) {
                resident.EnableVisual();
            }
        }
    }
}
