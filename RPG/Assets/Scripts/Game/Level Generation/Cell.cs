using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.LevelGeneration
{
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
            Type = CellType.INVALID;
            Region = -1; // -1 is a wall
        }
        public CellType Type;
        public int Region;
    }
}

