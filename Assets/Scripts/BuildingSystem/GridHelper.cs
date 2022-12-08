using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridHelper : MonoBehaviour
{
    [SerializeField] private Transform gridCrossLine;
    [SerializeField] private Transform gridCornerLine;
    [SerializeField] private Transform gridBorderLine;

    public void DrawLine(int x, int z, Vector3 position, int lengthX, int lengthZ) {
        if (x == 0) {
            if (z == 0) {
                DrawCornerLine(position, 0);
            }
            else if (z == lengthZ - 1) {
                DrawCornerLine(position, 90);
            }
            else {
                DrawBorderLine(position, 90);
            }
        } else if (x == lengthX - 1) {
            if (z == 0) {
                DrawCornerLine(position, -90);
            }
            else if (z == lengthZ - 1) {
                DrawCornerLine(position, 180);
            }
            else {
                DrawBorderLine(position, -90);
            }
        } else if (z == 0) {
            DrawBorderLine(position, 0);
        } else if (z == lengthZ - 1) {
            DrawBorderLine(position, 180);
        }
        else {
            DrawCrossLine(position);
        }
    }

    private void DrawCrossLine(Vector3 position) {
        Instantiate(gridCrossLine, position, Quaternion.identity, transform);
    }

    private void DrawCornerLine(Vector3 position, int rotation) {
        Instantiate(gridCornerLine, position, Quaternion.Euler(new Vector3(0, rotation, 0)), transform);
    }
    
    private void DrawBorderLine(Vector3 position, int rotation) {
        Instantiate(gridBorderLine, position, Quaternion.Euler(new Vector3(0, rotation, 0)), transform);
    }
}
