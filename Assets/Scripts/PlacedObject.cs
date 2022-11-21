using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacedObject : MonoBehaviour {
    public static PlacedObject Create(Vector3 worldPosition, Vector2Int origin, BuildingObject.Direction direction, 
        BuildingObject placedObjectType) {
        Transform placedObjectTransform = Instantiate(placedObjectType.prefab, worldPosition,
            Quaternion.Euler(0, placedObjectType.GetRotationAngle(direction), 0));
        PlacedObject placedObject = placedObjectTransform.GetComponent<PlacedObject>();
        placedObject._buildingObjectType = placedObjectType;
        placedObject._origin = origin;
        placedObject._direction = direction;

        return placedObject;
    }

    public List<Vector2Int> GetGridPositionList() {
        return _buildingObjectType.GetGridPositionList(_origin, _direction);
    }

    public void DestroySelf() {
        Destroy(gameObject);
    }
    
    private BuildingObject _buildingObjectType;
    public Vector2Int _origin;
    private BuildingObject.Direction _direction;
}
