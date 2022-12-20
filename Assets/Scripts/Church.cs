using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Church : MonoBehaviour, IBuilding
{
    private List<Transform> _visitors;
    [SerializeField] private int churchDuration;
    [SerializeField] private Transform entrance;

    [SerializeField] public static float ChurchRange = 20;
    
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

    public void ResidentEnter(Collider other) {
        Debug.Log("Resident Enter");
        Resident resident = other.GetComponent<Resident>();
        if (resident.tasks[0] == Resident.AvailableTasks.Church) {
            resident.DisableVisual();
            StartCoroutine(CompleteReligionTask(resident));
        }
    }

    IEnumerator CompleteReligionTask(Resident resident) {
        yield return new WaitForSeconds(churchDuration);
        resident.religionSatisfaction = 100f;
        resident.CompleteTask();
        
    }

    public void ResidentLeave(Collider other) {
        Resident resident = other.GetComponent<Resident>();
        resident.EnableVisual();
    }
}
