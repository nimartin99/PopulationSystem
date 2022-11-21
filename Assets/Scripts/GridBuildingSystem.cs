using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GridBuildingSystem : MonoBehaviour {
    // Grid related stuff
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] public float cellSize;
    
    // Buildings
    [SerializeField] private List<BuildingObject> buildingList;
    [FormerlySerializedAs("_building")] public BuildingObject building;
    [FormerlySerializedAs("defaultDirection")] [SerializeField] public BuildingObject.Direction direction = BuildingObject.Direction.Down;
    
    [SerializeField] private Camera camera;
    [SerializeField] private LayerMask groundLayerMask;
    
    public event EventHandler OnSelectedChanged;

    public Grid<GridObject> _grid;

    private void Awake() {
        _grid = new Grid<GridObject>(width, height, cellSize, Vector3.zero,
            (Grid<GridObject> g, int x, int z) => new GridObject(g, x, z));
        building = buildingList[0];
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, groundLayerMask)) {
                // Get the position where the Ray hits the grid
                _grid.GetXZ(hit.point, out int x, out int z);
                // Gets the list of x and z coordinates in the grid the building needs to be placed
                List<Vector2Int> occupyingGridObjectCoordinates = building.GetGridPositionList(new Vector2Int(x, z), direction);
                bool gridCoordinatesFree = true;
                // Checks if the needed grid coordinates are occupied
                foreach (Vector2Int position in occupyingGridObjectCoordinates) {
                    GridObject gridObject = _grid.GetGridObject(position.x, position.y);
                    if (!gridObject.CanBuild()) {
                        Debug.Log("Cannot build here!");
                        gridCoordinatesFree = false;
                        break;
                    }
                }

                if (gridCoordinatesFree) {
                    Vector2Int rotationOffset = building.GetRotationOffset(direction);
                    Vector3 worldPosition = _grid.GetWorldPosition(x, z) +
                                            new Vector3(rotationOffset.x, 0, rotationOffset.y) * cellSize;
                    PlacedObject placedObject = PlacedObject.Create(
                        worldPosition,
                        new Vector2Int(x, z),
                        direction,
                        building
                    );
                    // Scale the building to cellSize
                    // spawnedBuilding.transform.localScale = spawnedBuilding.transform.localScale * cellSize;
                    // Set the building to every Grid coordinate
                    foreach (Vector2Int position in occupyingGridObjectCoordinates) {
                        GridObject gridObject = _grid.GetGridObject(position.x, position.y);
                        gridObject.SetPlacedObject(placedObject);
                    }
                }
            }
        }

        if (Input.GetMouseButtonDown(1)) {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, groundLayerMask)) {
                // Get the position where the Ray hits the grid
                PlacedObject placedObject = _grid.GetGridObject(hit.point).GetPlacedObject();
                if (placedObject != null) {
                    placedObject.DestroySelf();
                    List<Vector2Int> occupyingGridObjectCoordinates = placedObject.GetGridPositionList();
                    foreach (Vector2Int position in occupyingGridObjectCoordinates) {
                        _grid.GetGridObject(position.x, position.y).ClearPlacedObject();
                    }
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.R)) {
            direction = building.GetNextDirection(direction);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            building = buildingList[0];
            OnSelectedChanged?.Invoke(this, EventArgs.Empty);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)) {
            building = buildingList[1];
            OnSelectedChanged?.Invoke(this, EventArgs.Empty);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3)) {
            building = buildingList[2];
            OnSelectedChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public class GridObject {
        private readonly Grid<GridObject> _grid;
        private readonly int _x;
        private readonly int _z;
        private PlacedObject _placedObject;

        public GridObject(Grid<GridObject> grid, int x, int z) {
            _grid = grid;
            _x = x;
            _z = z;
        }

        public override string ToString() {
            // return "(" + _x + ", " + _z + ")";
            return "(" + _x + ", " + _z + "): " + _placedObject;
        }

        public void SetPlacedObject(PlacedObject placedObject) {
            _placedObject = placedObject;
            _grid.TriggerGridObjectChanged(_x, _z);
        }

        public PlacedObject GetPlacedObject() {
            return _placedObject;
        }

        public void ClearPlacedObject() {
            _placedObject = null;
            _grid.TriggerGridObjectChanged(_x, _z);
        }

        public bool CanBuild() {
            return _placedObject == null;
        }
    }
}
