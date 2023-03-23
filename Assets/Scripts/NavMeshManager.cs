using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class NavMeshManager : MonoBehaviour
{
    [SerializeField] private GridBuildingSystem gridBuildingSystem;
    private NavMeshSurface _navMeshSurface;
    public bool buildNavMeshAfterUpdate;

    // Start is called before the first frame update
    void Start() {
        _navMeshSurface = GetComponent<NavMeshSurface>();
        gridBuildingSystem.OnGridChanged += OnGridChanged;
    }

    private void LateUpdate() {
        if (buildNavMeshAfterUpdate) {
            DateTime beforeRegenerate = DateTime.UtcNow;
            _navMeshSurface.BuildNavMesh();
            DateTime afterRegenerate = DateTime.UtcNow;
            TimeSpan timeSpan = afterRegenerate - beforeRegenerate;
            Debug.Log("Regenerating NavMesh took " + timeSpan.Milliseconds);
            buildNavMeshAfterUpdate = false;
        }
    }

    public void OnGridChanged(bool dontRegenerateNavMesh) {
        buildNavMeshAfterUpdate = !dontRegenerateNavMesh;
    }

    private void UpdateNavMesh() {
        _navMeshSurface.BuildNavMesh();
    }
}
