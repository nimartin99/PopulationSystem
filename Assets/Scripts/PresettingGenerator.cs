using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PresettingGenerator : MonoBehaviour {
    private GridBuildingSystem _gridBuildingSystem;
    private int _width;
    private int _height;
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
                        _gridBuildingSystem.PlaceBuilding(new Vector3(0.5f + j, 0, 0.5f + i));
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
                        _gridBuildingSystem.PlaceBuilding(new Vector3(0.5f + i, 0, 0.5f + j));
                        break;
                }
            }
        }
        _gridBuildingSystem.SelectBuilding(3);
        _gridBuildingSystem.PlaceBuilding(new Vector3(11, 0, 3));
        _gridBuildingSystem.PlaceBuilding(new Vector3(27, 0, 3));
        _gridBuildingSystem.SelectBuilding(2);
        _gridBuildingSystem.RotateBuilding();
        _gridBuildingSystem.RotateBuilding();
        _gridBuildingSystem.PlaceBuilding(new Vector3(11, 0, 8));
        _gridBuildingSystem.RotateBuilding();
        _gridBuildingSystem.PlaceBuilding(new Vector3(32, 0, 3));
        
        _gridBuildingSystem.SelectBuilding(1);
        _gridBuildingSystem.RotateBuilding();
        for (int j = 0; j < _height; j++) {
            for (int i = 0; i < _width; i++) {
                switch (j) {
                    case 0 when i == 0:
                        _gridBuildingSystem.PlaceBuilding(new Vector3(0.5f + i, 0, 0.5f + j));
                        break;
                    case 0 or 5 or 10 when i >= 3 && i < 10:
                        _gridBuildingSystem.PlaceBuilding(new Vector3(0.5f + i, 0, 0.5f + j));
                        i++;
                        break;
                    case 0 or 5 or 10 when i > 10 && i < 15:
                        _gridBuildingSystem.PlaceBuilding(new Vector3(0.5f + i, 0, 0.5f + j));
                        i++;
                        break;
                    case 0 or 5 or 10 when i > 15 && i < 26:
                        _gridBuildingSystem.PlaceBuilding(new Vector3(0.5f + i, 0, 0.5f + j));
                        i++;
                        break;
                    case 0 or 5 or 10 when i > 26 && i < 31:
                        _gridBuildingSystem.PlaceBuilding(new Vector3(0.5f + i, 0, 0.5f + j));
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
                        _gridBuildingSystem.PlaceBuilding(new Vector3(0.5f + i, 0, 0.5f + j));
                        break;
                    case 3 or 8 when i >= 3 && i < 10:
                        _gridBuildingSystem.PlaceBuilding(new Vector3(0.5f + i, 0, 0.5f + j));
                        i++;
                        break;
                    case 3 or 8 when i > 10 && i < 15:
                        _gridBuildingSystem.PlaceBuilding(new Vector3(0.5f + i, 0, 0.5f + j));
                        i++;
                        break;
                    case 3 or 8 when i > 15 && i < 26:
                        _gridBuildingSystem.PlaceBuilding(new Vector3(0.5f + i, 0, 0.5f + j));
                        i++;
                        break;
                    case 3 or 8 when i > 26 && i < 31:
                        _gridBuildingSystem.PlaceBuilding(new Vector3(0.5f + i, 0, 0.5f + j));
                        break;
                }
                _gridBuildingSystem.SelectBuilding(0);
                _gridBuildingSystem.SelectBuilding(1);
            }
        }
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
    }
    
    public void GeneratePresettingThree() {
        
    }
}
