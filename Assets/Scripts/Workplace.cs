using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Workplace : MonoBehaviour, IBuilding {
    private TimeController _timeController;

    private void Awake() {
        _timeController = TimeController.Instance;
    }

    public void BuildingPlaced() {
        throw new System.NotImplementedException();
    }

    public void ResidentEnter(Collider other) {
        Resident resident = other.GetComponent<Resident>();
        if (resident.tasks[0] == Resident.AvailableTasks.Work) {
            resident.DisableVisual();
            StartCoroutine(CompleteWorkTask(resident));
        }
    }

    private IEnumerator CompleteWorkTask(Resident resident) {
        yield return new WaitForSeconds(_timeController.workHours);
        resident.CompleteTask();
    }

    public void ResidentLeave(Collider other) {
        Resident resident = other.GetComponent<Resident>();
        resident.EnableVisual();
    }
}
