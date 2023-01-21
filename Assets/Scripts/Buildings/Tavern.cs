using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tavern : MonoBehaviour, IBuilding {
    [SerializeField] public Transform entrance;
    [SerializeField] private int tavernDuration;
    [SerializeField] public const float TavernRange = 25;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BuildingPlaced() {
        
    }

    public void BuildingDestroyed() {
        
    }

    public void ResidentEnter(Collider other) {
        Resident resident = other.GetComponent<Resident>();
        if (resident.tasks[0] == Resident.AvailableTasks.Tavern) {
            resident.DisableResident();
            StartCoroutine(CompleteTavernTask(resident));
        }
    }
    
    private IEnumerator CompleteTavernTask(Resident resident) {
        yield return new WaitForSeconds(tavernDuration);
        resident.happiness = 100f;
        resident.CompleteTask();
        resident.EnableResident();
    }

    public void ResidentLeave(Collider other) {
        
    }
}
