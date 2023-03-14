using System.Collections.Generic;
using UnityEngine;

public class PlacedObject : MonoBehaviour {
    public BuildingObject buildingObject;
    public Vector2Int originPosition;
    private BuildingObject.Direction _direction;

    public static PlacedObject Create(Vector3 worldPosition, Vector2Int gridPosition, BuildingObject.Direction direction, 
        BuildingObject placedObjectType, Transform parent) {
        Transform placedObjectTransform = Instantiate(placedObjectType.prefab, worldPosition,
            Quaternion.Euler(0, placedObjectType.GetRotationAngle(direction), 0), parent);
        PlacedObject placedObject = placedObjectTransform.GetComponent<PlacedObject>();
        placedObject.buildingObject = placedObjectType;
        placedObject.originPosition = gridPosition;
        placedObject._direction = direction;
        switch (placedObject.buildingObject.nameString) {
            case "House":
                placedObject.GetComponent<House>().enabled = true;
                break;
            case "Church":
                placedObject.GetComponent<Church>().enabled = true;
                break;
            case "Market":
                placedObject.GetComponent<Market>().enabled = true;
                break;
            case "Tavern":
                placedObject.GetComponent<Tavern>().enabled = true;
                break;
        }
        IBuilding building = placedObject.gameObject.GetComponent<IBuilding>();
        if (building != null) {
            building.BuildingPlaced();
        }
        return placedObject;
    }

    public List<Vector2Int> GetGridPositionList() {
        return buildingObject.GetGridPositionList(originPosition, _direction);
    }
    
    public void DestroySelf() {
        Destroy(gameObject);
    }
}
