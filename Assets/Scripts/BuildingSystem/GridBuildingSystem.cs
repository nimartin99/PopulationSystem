using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class GridBuildingSystem : MonoBehaviour {
    // Singleton
    public static GridBuildingSystem Instance { get; private set; }
    
    // Grid related stuff
    [SerializeField] public int width;
    [SerializeField] public int height;
    [SerializeField] public float cellSize;
    [SerializeField] private GridHelper gridHelper;

    // Buildings
    [SerializeField] private List<TwoDimensionalListHelper> buildingList;
    [FormerlySerializedAs("_building")] public BuildingObject building;
    [FormerlySerializedAs("defaultDirection")] [SerializeField] public BuildingObject.Direction direction = BuildingObject.Direction.Down;
    
    [SerializeField] private Transform environmentParent;
    
    public event EventHandler OnSelectedChanged;
    public event Action<bool> OnGridChanged;
    public event Action<string> OnBuildingPlaced;

    public Grid<GridObject> _grid;

    private void Awake() {
        if (Instance != null && Instance != this) { 
            Destroy(this); 
        } 
        else { 
            Instance = this; 
        } 
        
        _grid = new Grid<GridObject>(width, height, cellSize, Vector3.zero, gridHelper,
            (Grid<GridObject> g, int x, int z) => new GridObject(g, x, z));
        building = buildingList[0].buildingVariants[0];
    }

    public void PlaceBuilding(Vector3 mousePosition, bool dontRegenerateNavMesh = false) {
        // Get the position where the Ray hits the grid
        _grid.GetXZ(mousePosition, out int x, out int z);
        // Gets the list of x and z coordinates in the grid the building needs to be placed
        List<Vector2Int> occupyingGridObjectCoordinates = building.GetGridPositionList(new Vector2Int(x, z), direction);
        bool gridCoordinatesFree = true;
        // Checks if the needed grid coordinates are occupied
        foreach (Vector2Int position in occupyingGridObjectCoordinates) {
            GridObject gridObject = _grid.GetGridObject(position.x, position.y);
            if (!gridObject.CanBuild()) {
                // Debug.Log("Cannot build here!");
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
                building,
                environmentParent
            );
            // Scale the building to cellSize
            // spawnedBuilding.transform.localScale = spawnedBuilding.transform.localScale * cellSize;
            // Set the building to every Grid coordinate
            foreach (Vector2Int position in occupyingGridObjectCoordinates) {
                GridObject gridObject = _grid.GetGridObject(position.x, position.y);
                gridObject.SetPlacedObject(placedObject);
            }
            OnBuildingPlaced?.Invoke(building.nameString);
            // Trigger OnGridChanged event
            OnGridChanged?.Invoke(dontRegenerateNavMesh);
        }
    }

    public void DestroyBuilding(Vector3 mousePosition, bool dontRegenerateNavMesh = false) {
        // Get the position where the Ray hits the grid
        PlacedObject placedObject = _grid.GetGridObject(mousePosition).GetPlacedObject();
        if (placedObject != null) {
            IBuilding buildingToDestroy = placedObject.gameObject.GetComponent<IBuilding>();
            if (buildingToDestroy != null) {
                buildingToDestroy.BuildingDestroyed();
            }
            placedObject.DestroySelf();
            List<Vector2Int> occupyingGridObjectCoordinates = placedObject.GetGridPositionList();
            foreach (Vector2Int position in occupyingGridObjectCoordinates) {
                _grid.GetGridObject(position.x, position.y).ClearPlacedObject();
            }
            // Trigger OnGridChanged event
            OnGridChanged?.Invoke(dontRegenerateNavMesh);
        }
    }

    public void RotateBuilding() {
        direction = building.GetNextDirection(direction);
    }

    public void SelectBuilding(int typeIndex) {
        if (building.nameString != buildingList[typeIndex].buildingVariants[0].nameString) {
            building = buildingList[typeIndex].buildingVariants[Random.Range(0, buildingList[typeIndex].buildingVariants.Count)];
        }
        else {
            int nextBuildingVariant = 0;
            if (building.variant != buildingList[typeIndex].buildingVariants.Count - 1) {
                nextBuildingVariant = building.variant + 1;
            }
            building = buildingList[typeIndex].buildingVariants[nextBuildingVariant];
        }
        OnSelectedChanged?.Invoke(this, EventArgs.Empty);
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
            return "(" + _x + ", " + _z + ")";
            // return "(" + _x + ", " + _z + "): " + _placedObject;
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

        public string GetPlacedObjectType() {
            return _placedObject ? _placedObject.buildingObject.nameString : null;
        }
    }
}
