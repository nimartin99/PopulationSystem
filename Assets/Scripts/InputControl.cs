using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputControl : MonoBehaviour {
    public InputModes currentMode = InputModes.ExploreMode;
    
    public enum InputModes {
        ExploreMode,
        BuildingMode,
        DeleteMode,
    }
    
    [SerializeField] private Camera currentCamera;
    [SerializeField] private LayerMask groundLayerMask;

    [SerializeField] private UIControl uiControl;
    [SerializeField] private GridBuildingSystem gridBuildingSystem;
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O)) {
            foreach(GameObject fooObj in GameObject.FindGameObjectsWithTag("Resident"))
            {
                Debug.Log("Found Resident: " + fooObj);
                fooObj.GetComponent<Resident>().AddTask(Resident.AvailableTasks.Church);
            }
        }
        if (Input.GetKeyDown(KeyCode.M)) {
            switch (currentMode) {
                case InputModes.ExploreMode:
                    currentMode = InputModes.BuildingMode;
                    break;
                case InputModes.BuildingMode:
                    currentMode = InputModes.DeleteMode;
                    break;
                case InputModes.DeleteMode:
                    currentMode = InputModes.ExploreMode;
                    break;
            }
        }

        if (currentMode == InputModes.ExploreMode) {
            if (Input.GetMouseButtonDown(0)) {
                Transform hitTransform = GetMouseObject(out bool hitSomething);
                if (hitSomething) {
                    if (hitTransform.GetComponent<Resident>() != null) {
                        uiControl.SetToInspector(hitTransform, "Resident");
                    }
                }
            }
            if (Input.GetMouseButtonDown(1)) {
                uiControl.HideInspector();
            }
        }

        if (currentMode == InputModes.BuildingMode) {
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
            
            if (Input.GetMouseButtonDown(1)) {
                ResetToExploreMode();
            }
        }

        if (currentMode == InputModes.DeleteMode) {
            if (Input.GetMouseButtonDown(0)) {
                Vector3 hitPoint = GetGroundPosition(out bool hitSomething);
                if (hitSomething) {
                    gridBuildingSystem.DestroyBuilding(hitPoint);
                }
            }
            if (Input.GetMouseButtonDown(1)) {
                ResetToExploreMode();
            }
        }
    }

    private Vector3 GetGroundPosition(out bool hitSomething) {
        Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, groundLayerMask)) {
            hitSomething = true;
        }
        else {
            hitSomething = false;
        }
        return hit.point;
    }
    
    private Transform GetMouseObject(out bool hitSomething) {
        Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue)) {
            hitSomething = true;
        }
        else {
            hitSomething = false;
        }
        return hit.transform;
    }

    private void ResetToExploreMode() {
        currentMode = InputModes.ExploreMode;
    }
}
