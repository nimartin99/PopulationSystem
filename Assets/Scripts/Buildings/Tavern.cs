using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tavern : MonoBehaviour, IBuilding {
    [SerializeField] public Transform entrance;
    [SerializeField] private int tavernDuration;
    [SerializeField] public const float TavernRange = 25;

    public void BuildingPlaced() {}

    public void BuildingDestroyed() {}

    public void ResidentEnter(Resident resident) {
        if (resident && resident.currentTask == Resident.AvailableTasks.Tavern) {
            resident.DisableResident();
            StartCoroutine(CompleteTavernTask(resident));
        }
    }
    
    private IEnumerator CompleteTavernTask(Resident resident) {
        yield return new WaitForSeconds(tavernDuration);
        resident.tavernSatisfaction = 100f;
        resident.EnableResident();
        resident.CompleteTask();
    }

    public void ResidentLeave(Resident resident) {}
}
