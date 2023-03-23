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

    public void BuildingDestroyed() {}

    public void ResidentEnter(Resident resident) {
        if (resident && resident.currentTask == Resident.AvailableTasks.Work) {
            resident.DisableResident();
            StartCoroutine(CompleteWorkTask(resident));
        }
    }

    private IEnumerator CompleteWorkTask(Resident resident) {
        if (resident.role == "Workaholic") {
            yield return new WaitForSeconds(_timeController.realSecondsPerInGameDay / 24 * _timeController.workHours + 3);
        } else {
            yield return new WaitForSeconds(_timeController.realSecondsPerInGameDay / 24 * _timeController.workHours);
        }
        resident.workedToday = true;
        resident.CompleteTask();
        resident.EnableResident();
    }

    public void ResidentLeave(Resident resident) {}
}
