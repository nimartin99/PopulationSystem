using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PresettingGenerator : MonoBehaviour {
    private GridBuildingSystem _gridBuildingSystem;
    private int _width;
    private int _height;
    [SerializeField] private NavMeshManager navMeshManager;
    [SerializeField] private InputControl inputControl;
    [SerializeField] private BuildingGhost buildingGhost;

// public int counter = 100;
    private void Start() {
        _gridBuildingSystem = GridBuildingSystem.Instance;
        _width = _gridBuildingSystem.width;
        _height = _gridBuildingSystem.height;
    }

    public void GeneratePresettingOne() {
        _gridBuildingSystem.SelectBuilding(0);
        for (int i = 0; i < _width; i++) {
            for (int j = 0; j < _height; j++) {
                switch (j) {
                    case 2 when i <= 12:
                    case 10 when i < 17:
                    case 15 when i < 17:
                    case 26 when i < 13:
                    case 31 when i < 13:
                        _gridBuildingSystem.PlaceBuilding(new Vector3(0.5f + j, 0, 0.5f + i), true);
                        break;
                }
            }
        }
        for (int j = 0; j < _height; j++) {
            for (int i = 0; i < _width; i++) {
                switch (j) {
                    case 2 when i < 40:
                    case 7 when i < 40:
                    case 12 when i < 11:
                    case 12 when i >= 15 && i <= 31:
                    case 16 when i >= 10 && i <= 15:
                        _gridBuildingSystem.PlaceBuilding(new Vector3(0.5f + i, 0, 0.5f + j), true);
                        break;
                }
            }
        }
        _gridBuildingSystem.SelectBuilding(3);
        _gridBuildingSystem.PlaceBuilding(new Vector3(11, 0, 3), true);
        _gridBuildingSystem.PlaceBuilding(new Vector3(27, 0, 3), true);
        _gridBuildingSystem.SelectBuilding(2);
        _gridBuildingSystem.RotateBuilding();
        _gridBuildingSystem.RotateBuilding();
        _gridBuildingSystem.PlaceBuilding(new Vector3(11, 0, 8), true);
        _gridBuildingSystem.RotateBuilding();
        _gridBuildingSystem.PlaceBuilding(new Vector3(32, 0, 3), true);
        
        _gridBuildingSystem.SelectBuilding(1);
        _gridBuildingSystem.RotateBuilding();
        for (int j = 0; j < _height; j++) {
            for (int i = 0; i < _width; i++) {
                switch (j) {
                    case 0 when i == 0:
                        _gridBuildingSystem.PlaceBuilding(new Vector3(0.5f + i, 0, 0.5f + j), true);
                        break;
                    case 0 or 5 or 10 when i >= 3 && i < 10:
                        _gridBuildingSystem.PlaceBuilding(new Vector3(0.5f + i, 0, 0.5f + j), true);
                        i++;
                        break;
                    case 0 or 5 or 10 when i > 10 && i < 15:
                        _gridBuildingSystem.PlaceBuilding(new Vector3(0.5f + i, 0, 0.5f + j), true);
                        i++;
                        break;
                    case 0 or 5 or 10 when i > 15 && i < 26:
                        _gridBuildingSystem.PlaceBuilding(new Vector3(0.5f + i, 0, 0.5f + j), true);
                        i++;
                        break;
                    case 0 or 5 or 10 when i > 26 && i < 31:
                        _gridBuildingSystem.PlaceBuilding(new Vector3(0.5f + i, 0, 0.5f + j), true);
                        break;
                }
                _gridBuildingSystem.SelectBuilding(0);
                _gridBuildingSystem.SelectBuilding(1);
            }
        }
        _gridBuildingSystem.RotateBuilding();
        _gridBuildingSystem.RotateBuilding();
        for (int j = 0; j < _height; j++) {
            for (int i = 0; i < _width; i++) {
                switch (j) {
                    case 0 when i == 0:
                        _gridBuildingSystem.PlaceBuilding(new Vector3(0.5f + i, 0, 0.5f + j), true);
                        break;
                    case 3 or 8 when i >= 3 && i < 10:
                        _gridBuildingSystem.PlaceBuilding(new Vector3(0.5f + i, 0, 0.5f + j), true);
                        i++;
                        break;
                    case 3 or 8 when i > 10 && i < 15:
                        _gridBuildingSystem.PlaceBuilding(new Vector3(0.5f + i, 0, 0.5f + j), true);
                        i++;
                        break;
                    case 3 or 8 when i > 15 && i < 26:
                        _gridBuildingSystem.PlaceBuilding(new Vector3(0.5f + i, 0, 0.5f + j), true);
                        i++;
                        break;
                    case 3 or 8 when i > 26 && i < 31:
                        _gridBuildingSystem.PlaceBuilding(new Vector3(0.5f + i, 0, 0.5f + j), true);
                        break;
                }
                _gridBuildingSystem.SelectBuilding(0);
                _gridBuildingSystem.SelectBuilding(1);
            }
        }
        
        AfterGenerate();
    }
    
    public void GeneratePresettingTwo() {
        _gridBuildingSystem.SelectBuilding(0);
        for (int j = 0; j < _height; j++) {
            for (int i = 0; i < _width; i++) {
                switch (j) {
                    case 2 when i < 20:
                        _gridBuildingSystem.PlaceBuilding(new Vector3(0.5f + i, 0, 0.5f + j));
                        break;
                }
            }
        }
        _gridBuildingSystem.SelectBuilding(2);
        _gridBuildingSystem.RotateBuilding();
        _gridBuildingSystem.RotateBuilding();
        _gridBuildingSystem.PlaceBuilding(new Vector3(0.5f + 0f, 0f, 0.5f + 3f));
        _gridBuildingSystem.SelectBuilding(3);
        _gridBuildingSystem.PlaceBuilding(new Vector3(0.5f + 4f, 0f, 0.5f + 3f));
        _gridBuildingSystem.SelectBuilding(4);
        _gridBuildingSystem.RotateBuilding();
        _gridBuildingSystem.PlaceBuilding(new Vector3(0.5f + 8f, 0f, 0.5f + 3f));
        _gridBuildingSystem.SelectBuilding(1);
        _gridBuildingSystem.RotateBuilding();
        _gridBuildingSystem.RotateBuilding();
        _gridBuildingSystem.RotateBuilding();
        _gridBuildingSystem.PlaceBuilding(new Vector3(0.5f + 18f, 0f, 0.5f + 3f));
        
        AfterGenerate();
    }

    public void GeneratePresettingThree() {
        _gridBuildingSystem.SelectBuilding(0);
        for (int j = 0; j < _height; j++) {
            for (int i = 0; i < _width; i++) {
                switch (j) {
                    case 2 when i < 30:
                        _gridBuildingSystem.PlaceBuilding(new Vector3(0.5f + i, 0, 0.5f + j));
                        break;
                }
            }
        }
        _gridBuildingSystem.SelectBuilding(2);
        _gridBuildingSystem.RotateBuilding();
        _gridBuildingSystem.RotateBuilding();
        _gridBuildingSystem.PlaceBuilding(new Vector3(0.5f + 0f, 0f, 0.5f + 3f));
        _gridBuildingSystem.SelectBuilding(3);
        _gridBuildingSystem.PlaceBuilding(new Vector3(0.5f + 4f, 0f, 0.5f + 3f));
        _gridBuildingSystem.SelectBuilding(4);
        _gridBuildingSystem.RotateBuilding();
        _gridBuildingSystem.PlaceBuilding(new Vector3(0.5f + 8f, 0f, 0.5f + 3f));
        _gridBuildingSystem.SelectBuilding(1);
        _gridBuildingSystem.RotateBuilding();
        _gridBuildingSystem.RotateBuilding();
        _gridBuildingSystem.RotateBuilding();
        _gridBuildingSystem.PlaceBuilding(new Vector3(0.5f + 18f, 0f, 0.5f + 3f));
        Resident firstResident = FindObjectOfType<Resident>();
        firstResident.riotingReasons.Add("food");
        firstResident.riotingReasons.Add("religion");
        firstResident.riotingReasons.Add("tavern");
        firstResident.rioting = true;
        _gridBuildingSystem.PlaceBuilding(new Vector3(0.5f + 20f, 0f, 0.5f + 3f));
        _gridBuildingSystem.PlaceBuilding(new Vector3(0.5f + 22f, 0f, 0.5f + 3f));
        // _gridBuildingSystem.PlaceBuilding(new Vector3(0.5f + 24f, 0f, 0.5f + 3f));
        foreach (Resident resident in FindObjectsOfType<Resident>()) {
            resident.rioting = true;
        }
        
        AfterGenerate();
    }
    
    public void GeneratePresettingFour() {
        _gridBuildingSystem.SelectBuilding(0);
        for (int j = 0; j < _height; j++) {
            for (int i = 0; i < _width; i++) {
                switch (j) {
                    case 2 when i is < 30 and > 2:
                        _gridBuildingSystem.PlaceBuilding(new Vector3(0.5f + i, 0, 0.5f + j));
                        break;
                }
            }
        }
        _gridBuildingSystem.SelectBuilding(1);
        _gridBuildingSystem.RotateBuilding();
        _gridBuildingSystem.RotateBuilding();
        _gridBuildingSystem.RotateBuilding();
        _gridBuildingSystem.PlaceBuilding(new Vector3(0.5f + 18f, 0f, 0.5f + 3f));
        _gridBuildingSystem.PlaceBuilding(new Vector3(0.5f + 20f, 0f, 0.5f + 3f));
        _gridBuildingSystem.PlaceBuilding(new Vector3(0.5f + 22f, 0f, 0.5f + 3f));
        _gridBuildingSystem.PlaceBuilding(new Vector3(0.5f + 24f, 0f, 0.5f + 3f));
        foreach (Resident resident in FindObjectsOfType<Resident>()) {
            resident.averageOverallSatisfaction = 0f;
            resident.averageReligionSatisfaction = 0f;
            resident.averageTavernSatisfaction = 0f;
            resident.averageFoodSatisfaction = 0f;
        }
        
        AfterGenerate();
    }
    
    public void GeneratePresettingFive() {
        _gridBuildingSystem.SelectBuilding(3);
        _gridBuildingSystem.PlaceBuilding(new Vector3(8, 0, 8), true);
        _gridBuildingSystem.PlaceBuilding(new Vector3(18, 0, 18), true);
        _gridBuildingSystem.PlaceBuilding(new Vector3(28, 0, 28), true);
        _gridBuildingSystem.PlaceBuilding(new Vector3(38, 0, 38), true);
        _gridBuildingSystem.PlaceBuilding(new Vector3(38, 0, 8), true);
        _gridBuildingSystem.PlaceBuilding(new Vector3(28, 0, 18), true);
        _gridBuildingSystem.PlaceBuilding(new Vector3(18, 0, 28), true);
        _gridBuildingSystem.PlaceBuilding(new Vector3(8, 0, 38), true);
        _gridBuildingSystem.SelectBuilding(2);
        _gridBuildingSystem.PlaceBuilding(new Vector3(8, 0, 18), true);
        _gridBuildingSystem.PlaceBuilding(new Vector3(38, 0, 18), true);
        _gridBuildingSystem.RotateBuilding();
        _gridBuildingSystem.PlaceBuilding(new Vector3(18, 0, 8), true);
        _gridBuildingSystem.PlaceBuilding(new Vector3(18, 0, 38), true);
        _gridBuildingSystem.RotateBuilding();
        _gridBuildingSystem.RotateBuilding();
        _gridBuildingSystem.RotateBuilding();
        
        _gridBuildingSystem.SelectBuilding(0);
        for (int i = 0; i < _width; i++) {
            for (int j = 0; j < _height; j++) {
                switch (j) {
                    case 2 when i < 50:
                    case 7 when i < 50:
                    case 12 when i < 50:
                    case 17 when i < 50:
                    case 22 when i < 50:
                    case 27 when i < 50:
                    case 32 when i < 50:
                    case 37 when i < 50:
                    case 42 when i < 50:
                    case 47 when i < 50:
                        _gridBuildingSystem.PlaceBuilding(new Vector3(0.5f + j, 0, 0.5f + i), true);
                        break;
                }
            }
        }
        for (int j = 0; j < _height; j++) {
            for (int i = 0; i < _width; i++) {
                switch (j) {
                    case 2 when i < 50:
                    case 7 when i < 50:
                    case 12 when i < 50:
                    case 17 when i < 50:
                    case 22 when i < 50:
                    case 27 when i < 50:
                    case 32 when i < 50:
                    case 37 when i < 50:
                    case 42 when i < 50:
                    case 47 when i < 50:
                        _gridBuildingSystem.PlaceBuilding(new Vector3(0.5f + i, 0, 0.5f + j), true);
                        break;
                }
            }
        }
        _gridBuildingSystem.SelectBuilding(1);
        for (int j = 0; j < _height; j++) {
            for (int i = 0; i < _width; i++) {
                _gridBuildingSystem.PlaceBuilding(new Vector3(0.5f + i, 0, 0.5f + j), true);
            }
        }

        AfterGenerate();
    }

    private void AfterGenerate() {
        navMeshManager.buildNavMeshAfterUpdate = true;
    }

    // private void Update() {
    //     if (counter == 1000) {
    //         for (var x = 0; x < _width; x++) {
    //             for (var z = 0; z < _height; z++) {
    //                 _gridBuildingSystem.DestroyBuilding(new Vector3(x, 0, z), true);
    //             }
    //         }
    //
    //         GeneratePresettingFive();
    //         counter = 0;
    //     } else {
    //         counter++;
    //     }
    // }
}
