using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    // Minimum is 4
    public int iRoomXSize = 0;
    public int iRoomYSize = 0;


    public int iRoomRegion = -1;

    List<Vector2> Walls = new List<Vector2>();
    List<Vector2> Connectors = new List<Vector2>();

    public Vector2 StartPos = new Vector2(); // Bottom left of room


    public void CreateRandomSizes(int a_iMin, int a_iMax)
    {

        iRoomXSize = Random.Range(a_iMin, a_iMax);
        iRoomYSize = Random.Range(a_iMin, a_iMax);
    }

    public Vector2 GetConnector()
    {
        if(Connectors.Count <= 0)
        {
            return new Vector2(5, 5);

        }
        return Connectors[Random.Range(0, Connectors.Count)];
    }

    public void FindConnectors(Cell[,] a_cells)
    {
        int x;
        int y;
        for(int i = 0; i < Walls.Count; i++)
        {
            x = (int)Walls[i].x;
            y = (int)Walls[i].y;
            // Test as another room may have already processed this cell
            if (a_cells[x, y].type == CellType.ROOMWALL)
            {
                if (a_cells[x + 1, y].type == CellType.EMPTY && a_cells[x - 1, y].type == CellType.EMPTY)
                {
                    a_cells[x, y].type = CellType.ROOMCONNECTOR;
                    Connectors.Add(new Vector2(x, y));
                }
                else if (a_cells[x, y + 1].type == CellType.EMPTY && a_cells[x, y - 1].type == CellType.EMPTY)
                {
                    a_cells[x, y].type = CellType.ROOMCONNECTOR;
                    Connectors.Add(new Vector2(x, y));
                }

            }
        }
    }

    public void SetRoom(Cell[,] a_cells, int a_iRegion)
    {
        iRoomRegion = a_iRegion;
        for (int y = (int)StartPos.y; y < (int)StartPos.y + iRoomYSize; y++)
        {
            for (int x = (int)StartPos.x; x < (int)StartPos.x + iRoomXSize; x++)
            {
                if (x == StartPos.x)
                {
                    a_cells[x, y].type = CellType.ROOMWALL;
                    Walls.Add(new Vector2(x, y));
                }
                else if (y == StartPos.y)
                {
                    a_cells[x, y].type = CellType.ROOMWALL;
                    Walls.Add(new Vector2(x, y));
                }
                else if (x == StartPos.x + iRoomXSize - 1)
                {
                    a_cells[x, y].type = CellType.ROOMWALL;
                    Walls.Add(new Vector2(x, y));
                }
                else if (y == StartPos.y + iRoomYSize - 1)
                {
                    a_cells[x, y].type = CellType.ROOMWALL;
                    Walls.Add(new Vector2(x, y));
                }
                else
                {
                    a_cells[x, y].type = CellType.EMPTY;
                    a_cells[x, y].iRegion = a_iRegion;
                }
            }
        }
    }
}
