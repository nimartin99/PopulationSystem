using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour {
    [SerializeField] private Transform straightPath;
    [SerializeField] private Transform endPath;
    [SerializeField] private Transform fourWayIntersection;
    [SerializeField] private Transform threeWayIntersection;

    private PlacedObject _placedObject;
    private GridBuildingSystem _gridBuildingSystem;
    private Grid<GridBuildingSystem.GridObject> _grid;

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
}
