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
    private bool addTaskInNextUpdate = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (addTaskInNextUpdate)
        {
            foreach(GameObject fooObj in GameObject.FindGameObjectsWithTag("Resident"))
            {
                Debug.Log("found resident " + fooObj);
                fooObj.GetComponent<Resident>().AddTask(Resident.AvailableTasks.Church);
            }
            addTaskInNextUpdate = false;
        }
    }

    public void BuildingPlaced()
    {
        addTaskInNextUpdate = true;
    }

    public void ResidentEnter(Collider other) {
        Resident resident = other.GetComponent<Resident>();
        if (resident.tasks[0] == Resident.AvailableTasks.Church) {
            resident.DisableVisual();
            StartCoroutine(CompleteReligionTask(resident));
        }
    }

    private IEnumerator CompleteReligionTask(Resident resident) {
        yield return new WaitForSeconds(churchDuration);
        resident.religionSatisfaction = 100f;
        resident.CompleteTask();
        
    }

    public void ResidentLeave(Collider other) {
        Resident resident = other.GetComponent<Resident>();
        resident.EnableVisual();
    }
}
