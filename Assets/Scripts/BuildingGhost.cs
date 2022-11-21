using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class BuildingGhost : MonoBehaviour {

    private Transform _visual;
    private BuildingObject _buildingObject;
    [SerializeField] private Camera camera;

    [SerializeField] private GridBuildingSystem gridBuildingSystem;
    [SerializeField] private LayerMask groundLayerMask;

    // Start is called before the first frame update
    void Start()
    {
        RefreshVisual();
        gridBuildingSystem.OnSelectedChanged += BuildingSystemOnSelectedChanged;
    }

    private void BuildingSystemOnSelectedChanged(object sender, EventArgs e) {
        Debug.Log("BuildingSystemOnSelectedChanged");
        RefreshVisual();
    }

    public void LateUpdate() {
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, groundLayerMask)) {
            gridBuildingSystem._grid.GetXZ(hit.point, out int x, out int z);
            Vector2Int rotationOffset = gridBuildingSystem.building.GetRotationOffset(gridBuildingSystem.direction);
            Vector3 targetPos = gridBuildingSystem._grid.GetWorldPosition(x, z) +
                                new Vector3(rotationOffset.x, 0, rotationOffset.y) * gridBuildingSystem.cellSize;
            transform.position = Vector3.Lerp(transform.position, targetPos,
                Time.deltaTime * 15f);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0 , gridBuildingSystem.building.GetRotationAngle(gridBuildingSystem.direction), 0),  Time.deltaTime * 15f);

        }
    }

    private void RefreshVisual() {
        if (_visual != null) {
            Destroy(_visual.gameObject);
            _visual = null;
        }

        BuildingObject currentBuildingObject = gridBuildingSystem.building;

        if (currentBuildingObject != null) {
            _visual = Instantiate(currentBuildingObject.visual, Vector3.zero, Quaternion.identity);
            _visual.parent = transform;
            _visual.localPosition = Vector3.zero;
            _visual.localEulerAngles = Vector3.zero;
            // SetLayerRecursive(_visual.gameObject, 11);
        }
    }

    private void SetLayerRecursive(GameObject go, int layer) {
        if (go == null) {
            return;
        }
        gameObject.layer = layer;
        foreach (Transform child in go.transform) {
            SetLayerRecursive(child.gameObject, layer);
        }
    }
}
