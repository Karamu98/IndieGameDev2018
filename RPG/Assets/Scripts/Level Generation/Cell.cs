using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CellType
{
    WALL,
    EMPTY,
    CHEST,
    ENEMY,
    BOARDER,
    ROOMCONNECTOR,
    ROOMWALL,
    START,
    END,
    INVALID
}

public class Cell
{
    public Cell()
    {
        type = CellType.INVALID;
        iRegion = -1; // -1 is a wall
    }
    public CellType type;
    public int iRegion;
}
