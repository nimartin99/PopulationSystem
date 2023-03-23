using System.Collections;
using UnityEngine;

public class Tavern : MonoBehaviour, IBuilding {
    [SerializeField] public Transform entrance;
    [SerializeField] private int tavernDuration;
    public const float TavernRange = 25;

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
        if (resident.role == "Alcoholic") {
            resident.Drunk();
        }
    }

    public void ResidentLeave(Resident resident) {}
}
