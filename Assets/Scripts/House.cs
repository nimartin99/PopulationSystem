using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House : MonoBehaviour, IBuilding
{
    [SerializeField] private List<Transform> residents;
    [SerializeField] private Transform residentPrefab;
    [SerializeField] public Transform entrance;
    
    // Start is called before the first frame update
    void Start() {
        
    }

    public void BuildingPlaced() {
        Transform residentTransform = Instantiate(residentPrefab, entrance.position, Quaternion.identity);
        residents.Add(residentTransform);
        Resident resident = residentTransform.GetComponent<Resident>();
        resident.ResidentConstructor(this);
    }
    
    public void ResidentEnter(Collider other) {
        
    }
}
