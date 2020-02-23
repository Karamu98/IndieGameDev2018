using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
    public Vector2Int GridPosition;

    public Vector3 WorldPos;

    public bool IsWall;

    public PathNode Parent;

    public int GCost; // Distance from start
    public int HCost; // Manhatten distance from end node
    public int FCost { get { return GCost + HCost; } } // "Cost"
}
