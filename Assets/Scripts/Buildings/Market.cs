using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Market : MonoBehaviour, IBuilding {
    public Transform entrance;
    [SerializeField] public const float MarketRange = 25;
    [SerializeField] private int marketDuration;
    private UIControl _uiControl;

    private void Start() {
        _uiControl = UIControl.Instance;
    }

    public void BuildingPlaced() {}

    public void BuildingDestroyed() {}

    public void ResidentEnter(Resident resident) {
        if (resident && resident.currentTask == Resident.AvailableTasks.Market) {
            resident.DisableResident();
            StartCoroutine(CompleteMarketTask(resident));
        }
    }

    private IEnumerator CompleteMarketTask(Resident resident) {
        yield return new WaitForSeconds(marketDuration);
        switch (_uiControl.currentFoodPortion) {
            case "No Food":
                break;
            case "1/2 Portion":
                float currentResidentSatisfaction = resident.foodSatisfaction;
                currentResidentSatisfaction += 40f;
                resident.foodSatisfaction = Math.Clamp(currentResidentSatisfaction, 0f, 100f);
                break;
            case "Full Portion":
                resident.foodSatisfaction = 100f;
                break;
        }
        resident.EnableResident();
        resident.CompleteTask();
    }

    public void ResidentLeave(Resident resident) {}
}
