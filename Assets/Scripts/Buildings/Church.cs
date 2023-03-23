using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Church : MonoBehaviour, IBuilding
{
    private List<Transform> _visitors;
    [SerializeField] private int churchDuration;
    [SerializeField] public Transform entrance;
    public const float ChurchRange = 25;

    public void BuildingPlaced() {}

    public void BuildingDestroyed() {}

    public void ResidentEnter(Resident resident) {
        if (resident && resident.currentTask == Resident.AvailableTasks.Church) {
            resident.DisableResident();
            StartCoroutine(CompleteReligionTask(resident));
        }
    }

    private IEnumerator CompleteReligionTask(Resident resident) {
        yield return new WaitForSeconds(churchDuration);
        resident.religionSatisfaction = 100f;
        resident.EnableResident();
        resident.CompleteTask();
    }

    public void ResidentLeave(Resident resident) {}
}
