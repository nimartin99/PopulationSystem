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

    public void ResidentEnter(Collider other) {
        Resident resident = other.GetComponent<Resident>();
        if (resident.currentTask == Resident.AvailableTasks.Work) {
            resident.DisableResident();
            StartCoroutine(CompleteWorkTask(resident));
        }
    }

    private IEnumerator CompleteWorkTask(Resident resident) {
        yield return new WaitForSeconds(_timeController.workHours);
        resident.CompleteTask();
        resident.EnableResident();
        resident.workedToday = true;
    }

    public void ResidentLeave(Collider other) {
    }
}
