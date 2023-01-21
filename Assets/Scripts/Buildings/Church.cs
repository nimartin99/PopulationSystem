using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Church : MonoBehaviour, IBuilding
{
    private List<Transform> _visitors;
    [SerializeField] private int churchDuration;
    [SerializeField] public Transform entrance;

    [SerializeField] public const float ChurchRange = 25;

    public void BuildingPlaced()
    {
        
    }

    public void BuildingDestroyed() {
        
    }

    public void ResidentEnter(Collider other) {
        Resident resident = other.GetComponent<Resident>();
        if (resident.tasks[0] == Resident.AvailableTasks.Church) {
            resident.DisableResident();
            StartCoroutine(CompleteReligionTask(resident));
        }
    }

    private IEnumerator CompleteReligionTask(Resident resident) {
        yield return new WaitForSeconds(churchDuration);
        resident.religionSatisfaction = 100f;
        resident.CompleteTask();
        resident.EnableResident();
        
    }

    public void ResidentLeave(Collider other) {
        
    }
}
