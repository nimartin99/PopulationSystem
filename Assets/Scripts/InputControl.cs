using UnityEngine;

public class InputControl : MonoBehaviour {
    public InputModes currentMode = InputModes.ExploreMode;
    [SerializeField] private BuildingGhost buildingGhost;
    
    public enum InputModes {
        ExploreMode,
        BuildingMode,
        UpgradeMode,
        DeleteMode,
    }
    
    [SerializeField] private Camera currentCamera;
    [SerializeField] private LayerMask groundLayerMask;

    [SerializeField] private UIControl uiControl;
    [SerializeField] private GridBuildingSystem gridBuildingSystem;
    
    void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.M)) {
            switch (currentMode) {
                case InputModes.ExploreMode:
                    uiControl.ChangeMode(InputModes.BuildingMode);
                    break;
                case InputModes.BuildingMode:
                    uiControl.ChangeMode(InputModes.DeleteMode);
                    break;
                case InputModes.DeleteMode:
                    uiControl.ChangeMode(InputModes.ExploreMode);
                    break;
            }
        }

        switch (currentMode) {
            default:
            case InputModes.ExploreMode:
                ExecuteExploreInput();
                break;
            case InputModes.BuildingMode:
                ExecuteBuildInput();
                break;
            case InputModes.UpgradeMode:
                ExecuteUpgradeInput();
                break;
            case InputModes.DeleteMode:
                ExecuteDeleteInput();
                break;
        }
    }

    private void ExecuteExploreInput() {
        if (Input.GetMouseButtonDown(0)) {
            Transform hitTransform = GetMouseObject(out bool hitSomething);
            if (hitSomething) {
                if (hitTransform.GetComponent<Resident>() != null) {
                    uiControl.SetToInspector(hitTransform, "Resident");
                } else if (hitTransform.GetComponent<House>() != null) {
                    uiControl.SetToInspector(hitTransform, "House");
                } else if (hitTransform.parent && 
                           (hitTransform.GetComponent<Riot>() != null || 
                            hitTransform.parent.GetComponent<Riot>() != null ||
                            hitTransform.parent.transform.parent.GetComponent<Riot>() != null)) {
                    uiControl.SetToInspector(hitTransform, "Riot");
                }
            }
        }
        if (Input.GetMouseButtonDown(1)) {
            uiControl.HideInspector();
        }
    }
    
    private void ExecuteBuildInput() {
        if (Input.GetMouseButtonDown(0)) {
            Vector3 hitPoint = GetGroundPosition(out bool hitSomething);
            if (hitSomething) {
                gridBuildingSystem.PlaceBuilding(hitPoint);
            }
        }
        if (Input.GetKeyDown(KeyCode.R)) {
            gridBuildingSystem.RotateBuilding();
        }
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            gridBuildingSystem.SelectBuilding(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)) {
            gridBuildingSystem.SelectBuilding(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3)) {
            gridBuildingSystem.SelectBuilding(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4)) {
            gridBuildingSystem.SelectBuilding(3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5)) {
            gridBuildingSystem.SelectBuilding(4);
        }
            
        if (Input.GetMouseButtonDown(1)) {
            buildingGhost.DestroyVisual();
            uiControl.ChangeMode(InputModes.ExploreMode);
        }
    }
    
    private void ExecuteUpgradeInput() {
        if (Input.GetMouseButtonDown(0)) {
            Transform hitTransform = GetMouseObject(out bool hitSomething);
            if (hitSomething) {
                House house = hitTransform.GetComponent<House>();
                if (house != null && house.upgradeable) {
                    house.UpgradeToNextGen();
                }
            }
        }
    }
    
    private void ExecuteDeleteInput() {
        if (Input.GetMouseButtonDown(0)) {
            Vector3 hitPoint = GetGroundPosition(out bool hitSomething);
            if (hitSomething) {
                gridBuildingSystem.DestroyBuilding(hitPoint);
            }
        }
        if (Input.GetMouseButtonDown(1)) {
            uiControl.ChangeMode(InputModes.ExploreMode);
        }
    }

    private Vector3 GetGroundPosition(out bool hitSomething) {
        if (uiControl.hitUI) {
            hitSomething = false;
            return Vector3.zero;
        }
        Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);
        hitSomething = Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, groundLayerMask);
        return hit.point;
    }
    
    private Transform GetMouseObject(out bool hitSomething) {
        Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);
        hitSomething = Physics.Raycast(ray, out RaycastHit hit, float.MaxValue);
        return hit.transform;
    }
}
