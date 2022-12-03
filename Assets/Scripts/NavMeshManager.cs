using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class NavMeshManager : MonoBehaviour
{
    [SerializeField] private GridBuildingSystem gridBuildingSystem;
    private NavMeshSurface _navMeshSurface;
    private bool _buildNavMeshAfterUpdate;
    
    // Start is called before the first frame update
    void Start() {
        _navMeshSurface = GetComponent<NavMeshSurface>();
        gridBuildingSystem.OnGridChanged += OnGridChanged;
    }

    private void LateUpdate() {
        if (_buildNavMeshAfterUpdate) {
            _navMeshSurface.BuildNavMesh();
        }
    }

    private void OnGridChanged(object sender, EventArgs e) {
        _buildNavMeshAfterUpdate = true;
    }

    private void UpdateNavMesh() {
        _navMeshSurface.BuildNavMesh();
    }
}
