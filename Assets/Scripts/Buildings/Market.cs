using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Market : MonoBehaviour, IBuilding {
    public Transform entrance;
    [SerializeField] public const float MarketRange = 25;
    [SerializeField] private int marketDuration;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void BuildingPlaced()
    {
        
    }

    public void ResidentEnter(Collider other) {
        Resident resident = other.GetComponent<Resident>();
        if (resident.tasks[0] == Resident.AvailableTasks.Market) {
            resident.DisableResident();
            StartCoroutine(CompleteMarketTask(resident));
        }
    }

    private IEnumerator CompleteMarketTask(Resident resident) {
        yield return new WaitForSeconds(marketDuration);
        resident.foodSatisfaction = 100f;
        resident.CompleteTask();
        resident.EnableResident();
    }

    public void ResidentLeave(Collider other) {
        
    }
}
