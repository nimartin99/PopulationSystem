using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid<TGridObject> {
    private readonly int _width;
    private readonly int _height;
    private readonly float _cellSize;
    private readonly Vector3 _originPosition;
    private LineRenderer _lineRenderer;
    private readonly TGridObject[,] _gridArray; // 2D Array
    private TextMesh[,] _gridTextArray;
    private bool debug = false;

    public Grid(int width, int height, float cellSize, Vector3 originPosition, GridHelper gridHelper,
        Func<Grid<TGridObject>, int, int, TGridObject> createGridObject) {
        _width = width;
        _height = height;
        _cellSize = cellSize;
        _originPosition = originPosition;
        _gridArray = new TGridObject[width, height];
        _gridTextArray = new TextMesh[width, height];
        
        for (var x = 0; x < _gridArray.GetLength(0); x++) {
            for (var z = 0; z < _gridArray.GetLength(1); z++) {
                // Create Objects
                _gridArray[x, z] = createGridObject(this, x, z);

                if (debug) {
                    _gridTextArray[x, z] = Utils.CreateWorldText("GridObject\n" + _gridArray[x, z].ToString(), null,
                        GetWorldPosition(x, z) + new Vector3(cellSize / 2, 0, cellSize / 2), 20, Color.red,
                        TextAnchor.MiddleCenter);
                }

                gridHelper.DrawLine(x, z, GetWorldPosition(x, z), _gridArray.GetLength(0), _gridArray.GetLength(1));
            }
        }
    }

    /// <summary>
    /// Gets the Vector 3 world position of a given x and z coordinate in the grid
    /// </summary>
    /// <param name="x">The x coordinate from which the position will be retrieved</param>
    /// <param name="z">The z coordinate from which the position will be retrieved</param>
    /// <returns>The Vector 3 worldPosition from the x and z value</returns>
    public Vector3 GetWorldPosition(int x, int z) {
        return new Vector3(x, 0, z) * _cellSize + _originPosition;
    }
    
    /// <summary>
    /// Gets the x and z coordinate of the grid by a Vector 3 position
    /// </summary>
    /// <param name="worldPosition">The world position from which the x and z will be retrieved</param>
    /// <param name="x">The x coordinate of the Grid at the give position</param>
    /// <param name="z">The z coordinate of the Grid at the give position</param>
    public void GetXZ(Vector3 worldPosition, out int x, out int z) {
        x = Mathf.FloorToInt((worldPosition - _originPosition).x / _cellSize);
        z = Mathf.FloorToInt((worldPosition - _originPosition).z / _cellSize);
    }
    
    /// <summary>
    /// Sets a value to the grid by a given x and z coordinate of the grid
    /// </summary>
    /// <param name="x">The x position</param>
    /// <param name="z">The z position</param>
    /// <param name="value">The value that should be set at the coordinates</param>
    private void SetGridObject(int x, int z, TGridObject value) {
        if (x >= 0 && x < _width && z < _height) {
            _gridArray[x, z] = value;
        }
    }
    
    /// <summary>
    /// Sets a value to the grid by a given Vector 3 position
    /// </summary>
    /// <param name="worldPosition">The position where the value should be set</param>
    /// <param name="value">The value that should be set</param>
    public void SetGridObject(Vector3 worldPosition, TGridObject value) {
        GetXZ(worldPosition, out var x, out var z);
        SetGridObject(x, z, value);
    }

    public TGridObject GetGridObject(int x, int z) {
        if (x >= 0 && z >= 0 && x < _width && z < _height) {
            return _gridArray[x, z];
        }
        return default(TGridObject);
    }

    public TGridObject GetGridObject(Vector3 worldPosition) {
        GetXZ(worldPosition, out int x, out int z);
        return GetGridObject(x, z);
    }

    public void TriggerGridObjectChanged(int x, int z) {
        if (debug) {
            _gridTextArray[x, z].text = _gridArray[x, z].ToString();
        }
    }
}