using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlacedObject : MonoBehaviour {
    private BuildingObject _buildingObjectType;
    public Vector2Int gridPosition;
    private BuildingObject.Direction _direction;

    public static PlacedObject Create(Vector3 worldPosition, Vector2Int gridPosition, BuildingObject.Direction direction, 
        BuildingObject placedObjectType, Transform parent) {
        Transform placedObjectTransform = Instantiate(placedObjectType.prefab, worldPosition,
            Quaternion.Euler(0, placedObjectType.GetRotationAngle(direction), 0), parent);
        PlacedObject placedObject = placedObjectTransform.GetComponent<PlacedObject>();
        placedObject._buildingObjectType = placedObjectType;
        placedObject.gridPosition = gridPosition;
        placedObject._direction = direction;
        IBuilding building = placedObject.gameObject.GetComponent<IBuilding>();
        if (building != null) {
            building.BuildingPlaced();
        }
        return placedObject;
    }

    public List<Vector2Int> GetGridPositionList() {
        return _buildingObjectType.GetGridPositionList(gridPosition, _direction);
    }
    
    public void DestroySelf() {
        Destroy(gameObject);
    }
}
