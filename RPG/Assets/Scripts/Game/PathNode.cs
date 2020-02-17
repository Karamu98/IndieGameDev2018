using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
    public int iXPos;
    public int iYPos;

    public Vector3 worldPos;

    public bool IsWall;

    public PathNode Parent;

    public int iGCost; // Distance from start
    public int iHCost; // Manhatten distance from end node
    public int iFCost { get { return iGCost + iHCost; } } // "Cost"
}
