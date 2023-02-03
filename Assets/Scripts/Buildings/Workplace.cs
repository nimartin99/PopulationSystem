using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Workplace : MonoBehaviour, IBuilding {
    private TimeController _timeController;

    private void Start() {
        _timeController = TimeController.Instance;
    }

    public void BuildingPlaced() {
        throw new System.NotImplementedException();
    }

    public void BuildingDestroyed() {
        
    }

    public void ResidentEnter(Collider other) {
        Resident resident = other.GetComponent<Resident>();
        if (resident && resident.currentTask == Resident.AvailableTasks.Work) {
            resident.DisableResident();
            StartCoroutine(CompleteWorkTask(resident));
        }
    }

    private IEnumerator CompleteWorkTask(Resident resident) {
        yield return new WaitForSeconds(_timeController.workHours);
        resident.workedToday = true;
        resident.EnableResident();
        resident.CompleteTask();
    }

    public void ResidentLeave(Collider other) {
    }
}
