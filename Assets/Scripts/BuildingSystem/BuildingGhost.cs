using System;
using UnityEngine;

public class BuildingGhost : MonoBehaviour {

    public Transform visual;
    private BuildingObject _buildingObject;
    [SerializeField] private Camera mainCamera;

    [SerializeField] private GridBuildingSystem gridBuildingSystem;
    [SerializeField] private LayerMask groundLayerMask;

    // Start is called before the first frame update
    void Start()
    {
        gridBuildingSystem.OnSelectedChanged += BuildingSystemOnSelectedChanged;
    }

    private void BuildingSystemOnSelectedChanged(object sender, EventArgs e) {
        RefreshVisual();
    }

    public void LateUpdate() {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, groundLayerMask)) {
            gridBuildingSystem.Grid.GetXZ(hit.point, out int x, out int z);
            Vector2Int rotationOffset = gridBuildingSystem.building.GetRotationOffset(gridBuildingSystem.direction);
            Vector3 targetPos = gridBuildingSystem.Grid.GetWorldPosition(x, z) +
                                new Vector3(rotationOffset.x, 0, rotationOffset.y) * gridBuildingSystem.cellSize;
            transform.position = Vector3.Lerp(transform.position, targetPos,
                Time.deltaTime * 15f);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0 , gridBuildingSystem.building.GetRotationAngle(gridBuildingSystem.direction), 0),  Time.deltaTime * 15f);

        }
    }

    public void RefreshVisual() {
        DestroyVisual();
        BuildingObject currentBuildingObject = gridBuildingSystem.building;
        if (currentBuildingObject != null) {
            visual = Instantiate(currentBuildingObject.prefab, Vector3.zero, Quaternion.identity);
            visual.parent = transform;
            SetLayerRecursively(gameObject, 8);
            visual.localPosition = Vector3.zero;
            visual.localEulerAngles = Vector3.zero;
        }
    }

    public void DestroyVisual() {
        if (visual != null) {
            Destroy(visual.gameObject);
            visual = null;
        }
    }

    private void SetLayerRecursively(GameObject obj, int newLayer) {
        if (null == obj) {
            return;
        }
        obj.layer = newLayer;
        foreach (Transform child in obj.transform) {
            if (null == child) {
                continue;
            }
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }

}
