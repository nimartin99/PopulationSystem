using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildingObject", menuName = "ScriptableObjects/BuildingObject")]
public class BuildingObject : ScriptableObject {
    public string nameString;
    public Transform prefab;
    public Transform visual;
    public int width;
    public int height;

    public enum Direction {
        Down,
        Left,
        Up,
        Right,
    }

    public Direction GetNextDirection(Direction currentDirection) {
        switch (currentDirection) {
            case Direction.Down: return Direction.Left;
            case Direction.Left: return Direction.Up;
            case Direction.Up: return Direction.Right;
            default:
            case Direction.Right: return Direction.Down;
        }
    }

    public int GetRotationAngle(Direction direction) {
        switch (direction) {
            default:
            case Direction.Down: return 0;
            case Direction.Left: return 90;
            case Direction.Up: return 180;
            case Direction.Right: return 270;
        }
    }

    public Vector2Int GetRotationOffset(Direction direction) {
        switch (direction) {
            default:
            case Direction.Down: return new Vector2Int(0, 0);
            case Direction.Left: return new Vector2Int(0, width);
            case Direction.Up: return new Vector2Int(width, height);
            case Direction.Right: return new Vector2Int(height, 0);
        }
    }

    public List<Vector2Int> GetGridPositionList(Vector2Int offset, Direction direction) {
        List<Vector2Int> gridPositionList = new List<Vector2Int>();
        switch (direction) {
            default:
                case Direction.Down:
                case Direction.Up:
                    for (int x = 0; x < width; x++) {
                        for (int y = 0; y < height; y++) {
                            gridPositionList.Add(offset + new Vector2Int(x, y));
                        }
                    }
                    break;
                case Direction.Left:
                case Direction.Right:
                    for (int x = 0; x < height; x++) {
                        for (int y = 0; y < width; y++) {
                            gridPositionList.Add(offset + new Vector2Int(x, y));
                        }
                    }
                    break;
        }
        return gridPositionList;
    }
}
