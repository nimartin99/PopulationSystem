using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour {
    [SerializeField] private GameObject buildingPrefab;
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private int cellSize;
    [SerializeField] private Camera camera;
    [SerializeField] private LayerMask groundLayerMask;
    private Grid<GridBuildingSystem.GridObject> _grid;
    
    // Start is called before the first frame update
    void Start()
    {
        _grid = new Grid<GridBuildingSystem.GridObject>(width, height, cellSize, Vector3.zero, (Grid<GridBuildingSystem.GridObject> g, int x, int z) => new GridBuildingSystem.GridObject(g, x, z));
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, groundLayerMask)) {
                // _grid.SetGridObject(hit.point, 0);
            }
        }
    }

    public GameObject PlaceBuilding(Vector3 worldPosition) {
        Vector3 prefabSize = buildingPrefab.GetComponent<MeshRenderer>().bounds.size;
        Vector3 adjustedPosition = new Vector3(worldPosition.x + prefabSize.x / 2, worldPosition.y + prefabSize.y / 2, worldPosition.z + prefabSize.z / 2);
        return Instantiate(buildingPrefab, adjustedPosition, Quaternion.identity);
    }

    public void DestroyBuilding(GameObject building) {
        Destroy(building);
    }
}
