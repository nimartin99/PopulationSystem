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

    public void BuildingPlaced()
    {
        
    }

    public void BuildingDestroyed() {
        
    }

    public void ResidentEnter(Collider other) {
        Resident resident = other.GetComponent<Resident>();
        if (resident.currentTask == Resident.AvailableTasks.Market) {
            resident.DisableResident();
            StartCoroutine(CompleteMarketTask(resident));
        }
    }

    private IEnumerator CompleteMarketTask(Resident resident) {
        yield return new WaitForSeconds(marketDuration);
        float currentResidentSatisfaction = resident.foodSatisfaction;
        switch (_uiControl.currentFoodPortion) {
            case "No Food":
                break;
            case "1/2 Portion":
                currentResidentSatisfaction += 40f;
                if (currentResidentSatisfaction > 100f) {
                    resident.foodSatisfaction = 100f;
                }
                else {
                    resident.foodSatisfaction = currentResidentSatisfaction;
                }
                break;
            case "Full Portion":
                resident.foodSatisfaction = 100f;
                break;
        }
        resident.CompleteTask();
        resident.EnableResident();
    }

    public void ResidentLeave(Collider other) {
        
    }
}
