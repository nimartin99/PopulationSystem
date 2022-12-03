using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Church : MonoBehaviour, IBuilding
{
    private List<Transform> _visitors;
    [SerializeField] private int churchDuration;
    [SerializeField] private Transform entrance;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BuildingPlaced() {
        foreach(GameObject fooObj in GameObject.FindGameObjectsWithTag("Resident"))
        {
            Debug.Log("Found Resident: " + fooObj);
            fooObj.GetComponent<Resident>().AddTask(entrance);
        }
    }

    public void ResidentEnter(Collider other) {
        Debug.Log("Resident Enter");
        Resident resident = other.GetComponent<Resident>();
        if (resident.tasks[0] == entrance) {
            resident.DisableVisual();
            StartCoroutine(ResidentLeave(resident));
        }
    }

    IEnumerator ResidentLeave(Resident resident) {
        Debug.Log("Resident Leave start");
        yield return new WaitForSeconds(churchDuration);
        Debug.Log("Resident Leave Execute");
        resident.RemoveTask(0);
        resident.EnableVisual();
    }
}
