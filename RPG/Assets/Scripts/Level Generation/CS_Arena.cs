using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_Arena : MonoBehaviour
{
    // Bottom left is 0, 0. Left to right Bottom to top.

    /// <summary>
    /// TODO: Improve dungeon generation.
    /// Find a way of connecting "Rooms" together
    /// Remove Inverse Rooms
    /// </summary>

    enum Direction
    {
        NORTH,
        SOUTH,
        EAST,
        WEST,
        NULL
    }


    // ======================== Dungeon Settings ===========================
    // ---------------------------------------------------------------------
    // ============ Prefabs ================
    [SerializeField] GameObject WallPrefab;
    [SerializeField] GameObject MiniFloorPrefab;
    [SerializeField] GameObject CeilingPrefab;
    [SerializeField] GameObject FloorPrefab;
    [SerializeField] GameObject ChestPrefab;
    [SerializeField] GameObject EndPrefab;
    [SerializeField] GameObject[] Enemies;

    // ========== Settings ====================
    [SerializeField] int iMaxCellsX = 31;
    [SerializeField] int iMaxCellsY = 31;
    [SerializeField] int RoomAmount = 50;
    [SerializeField] float fWindingPercentage = 0.5f;
    [Range(0, 100)] public float ChestSpawnPercent = 0.1f;
    [SerializeField] int iRoomMax = 6;
    [SerializeField] int iRoomMin = 4; // Cannot be less that 4 or will give hallway rooms
    

    // ============== Player ==============
    private GameObject player;
    Vector3 playerStart = new Vector3();

    // ============= Data ==============
    System.Random rand;
    private string Seed;
    GameObject[,] Arena;
    Cell[,] Cells;
    List<Room> RoomList = new List<Room>();

    // TEMPORARY BEFORE BUG FIX
    Vector3 start;
    Vector3 end;

    // ============= Required "Globals" =============
    int m_CurrentRegion = 0; // This is yet to be fully implemented
    int iCurrentEnemy = 0;
    private Vector2Int endCell;


    // Use this for initialization
    private void Start()
    {
        // Grab the player before the inspector is filled with gameobjects
        player = GameObject.FindGameObjectWithTag("Player");
        
        // Grab our seed
        Seed = CS_GameManager.Seed;
        rand = new System.Random(Seed.GetHashCode());

        // Initialise "Cells"
        Cells = new Cell[iMaxCellsX, iMaxCellsY];
        InitialiseCells();

        // Layout map in cell data
        PlaceRooms(RoomAmount, 200);
        GrowMaze();
        FindConnectors();
        ConnectRegions();
        RemoveDeadEnds();
        FindStart();
        FindEnd();
        PlaceEnemies();

        // Build
        Arena = new GameObject[iMaxCellsX, iMaxCellsY];
        BuildLevel();

        CS_GameManager.StartGame(Cells, start, end);

        Cells = null;
        Arena = null;
    }

    private void PlaceEnemies()
    {
        // Simple way of placing enemies in the way of the end point
        if(Enemies.Length <= 0)
        {
            return;
        }
        // Cache to save processing
        int halfX = (int)(iMaxCellsX * 0.5f);
        int halfY = (int)(iMaxCellsY * 0.5f);

        bool bPlaced = false;

        for(int i = 0; i < Enemies.Length; i++)
        {
            bPlaced = false;
            for (int y = endCell.y; y > halfY; y-= 2)
            {
                if(bPlaced)
                {
                    continue;
                }
                for (int x = endCell.x; x > halfX; x-=2)
                {
                    if (Cells[x, y].type == CellType.EMPTY)
                    {
                        Cells[x, y].type = CellType.ENEMY;
                        bPlaced = true;
                        break;
                    }
                }
            }
        }
    }

    private void FindStart()
    {
        // Quick simple way for finding a better start position (Find first open cell from bottom left)
        for (int y = 1; y < iMaxCellsY - 1; y++)
        {
            for (int x = 1; x < iMaxCellsX - 1; x++)
            {
                if (Cells[x, y].type == CellType.EMPTY)
                {
                    Cells[x, y].type = CellType.START;
                    return;
                }
            }
        }
    }

    private void FindEnd()
    {
        // Quick simple way for finding a better end position (Find first open cell from top right)
        for (int y = iMaxCellsY - 1; y > 1; y--)
        {
            for (int x = iMaxCellsX - 1; x > 1; x--)
            {
                if (Cells[x, y].type == CellType.EMPTY)
                {
                    Cells[x, y].type = CellType.END;
                    endCell = new Vector2Int(x, y);
                    return;
                }
            }
        }
    }

    private void ConnectRegions()
    {
        // Grab a room and break a connector
        Room currentRoom;
        int x, y;
        for (int i = 0; i < RoomList.Count; i++)
        {
            currentRoom = RoomList[i];
            Vector2 pos = currentRoom.GetConnector();
            x = (int)pos.x;
            y = (int)pos.y;

            Cells[x, y].type = CellType.EMPTY;
            Cells[x, y].iRegion = m_CurrentRegion;
        }
    }

    private void PlaceRooms(int a_amount, int a_maxAttempts)
    {
        for (int i = 0; i < a_amount; i++)
        {
            Room newRoom = new Room();
            for (int j = 0; j < a_maxAttempts; j++)
            {
                newRoom.CreateRandomSizes(iRoomMin, iRoomMax);

                // Get a random point in area to place room
                int startX = Random.Range(1, iMaxCellsX - 1);
                int startY = Random.Range(1, iMaxCellsY - 1);

                newRoom.StartPos = new Vector2(startX, startY);
                

                // Test if its in bounds
                if ((startX - 1) + (newRoom.iRoomXSize + 2) < iMaxCellsX && (startY - 1) + (newRoom.iRoomYSize + 2) < iMaxCellsY)
                {
                    // Test if it's overlapping other rooms
                    bool bIsValid = true;
                    for (int y = startY; y < startY + newRoom.iRoomYSize; y++)
                    {
                        for (int x = startX; x < startX + newRoom.iRoomXSize; x++)
                        {
                            if (Cells[x, y].type == CellType.EMPTY || Cells[x, y].type == CellType.BOARDER)
                            {
                                bIsValid = false;
                                continue;
                            }
                        }
                    }

                    if (bIsValid)
                    {
                        // If the room is valid, apply it to our map
                        newRoom.SetRoom(Cells, m_CurrentRegion);
                        RoomList.Add(newRoom);
                        m_CurrentRegion++;
                        j = a_maxAttempts;
                    }

                }

            }
        }


    }

    private void FindConnectors()
    {
        // Sets up all the connectors for rooms
        for(int i = 0; i < RoomList.Count; i++)
        {
            RoomList[i].FindConnectors(Cells);
        }
    }

    private void InitialiseCells()
    {       
        // Iterate over all cells and make them a "wall" or a "boarder" if the cell is on the edge
        for (int y = 0; y < iMaxCellsY; y++)
        {
            for (int x = 0; x < iMaxCellsX; x++)
            {
                if (x == 0 || x == iMaxCellsX - 1 || y == 0 || y == iMaxCellsY - 1)
                {
                    Cells[x, y] = new Cell
                    {
                        type = CellType.BOARDER
                    };
                }
                else
                {
                    Cells[x, y] = new Cell
                    {
                        type = CellType.WALL
                    };
                }
            }
        }
    }

    private void BuildLevel()
    {
        // Iterate over all the cells and place the appropiate block based on the cell types
        for (int y = 0; y < iMaxCellsY; y++)
        {
            for (int x = 0; x < iMaxCellsX; x++)
            {
                switch (Cells[x, y].type)
                {
                    case CellType.WALL:
                        {
                            Arena[x, y] = Instantiate(WallPrefab, gameObject.transform);
                            Arena[x, y].gameObject.transform.localScale = new Vector3(CS_GameManager.fGridCellSize, CS_GameManager.fGridCellSize, CS_GameManager.fGridCellSize);
                            Arena[x, y].gameObject.name = "Wall x: " + x + " Wall Y: " + y;
                            break;
                        }

                    case CellType.CHEST:
                        {
                            Arena[x, y] = Instantiate(ChestPrefab, gameObject.transform);
                            Arena[x, y].gameObject.name = "Chest x: " + x + " Chest Y: " + y;
                            break;
                        }
                    case CellType.EMPTY:
                        {
                            Arena[x, y] = new GameObject();
                            Arena[x, y].transform.parent = gameObject.transform;
                            Arena[x, y].gameObject.name = "Empty x: " + x + " Empty Y: " + y;
                            break;
                        }
                    case CellType.ENEMY:
                        {
                            Arena[x, y] = Instantiate(Enemies[iCurrentEnemy], gameObject.transform);
                            Arena[x, y].gameObject.name = "Enemy: " + (iCurrentEnemy + 1);
                            iCurrentEnemy++;
                            break;
                        }
                    case CellType.BOARDER:
                        {
                            Arena[x, y] = Instantiate(WallPrefab, gameObject.transform);
                            Arena[x, y].gameObject.transform.localScale = new Vector3(CS_GameManager.fGridCellSize, CS_GameManager.fGridCellSize, CS_GameManager.fGridCellSize);
                            Arena[x, y].gameObject.name = "Boarder x: " + x + " Boarder Y: " + y;
                            Arena[x, y].gameObject.transform.position = new Vector3(CS_GameManager.fGridCellSize * x, -(CS_GameManager.fGridCellSize * 0.5f), CS_GameManager.fGridCellSize * y);
                            break;
                        }
                    case CellType.END:
                        {
                            Arena[x, y] = Instantiate(EndPrefab, gameObject.transform);
                            Arena[x, y].gameObject.transform.localScale = new Vector3(CS_GameManager.fGridCellSize, CS_GameManager.fGridCellSize, CS_GameManager.fGridCellSize);
                            Arena[x, y].gameObject.name = "End x: " + x + " End Y: " + y;
                            Arena[x, y].gameObject.transform.position = new Vector3(CS_GameManager.fGridCellSize * x, -(CS_GameManager.fGridCellSize * 0.5f), CS_GameManager.fGridCellSize * y);
                            break;
                        }
                    case CellType.START:
                        {
                            Arena[x, y] = new GameObject();
                            Arena[x, y].transform.parent = gameObject.transform;
                            Arena[x, y].gameObject.name = "Start x: " + x + " Start Y: " + y;
                            break;
                        }
                    case CellType.ROOMCONNECTOR:
                        {
                            Arena[x, y] = Instantiate(WallPrefab, gameObject.transform);
                            Arena[x, y].gameObject.transform.localScale = new Vector3(CS_GameManager.fGridCellSize, CS_GameManager.fGridCellSize, CS_GameManager.fGridCellSize);
                            Arena[x, y].gameObject.name = "Room Connector x: " + x + " Room Connector Y: " + y;
                            break;
                        }
                    case CellType.ROOMWALL:
                        {
                            Arena[x, y] = Instantiate(WallPrefab, gameObject.transform);
                            Arena[x, y].gameObject.transform.localScale = new Vector3(CS_GameManager.fGridCellSize, CS_GameManager.fGridCellSize, CS_GameManager.fGridCellSize);
                            Arena[x, y].gameObject.name = "Room Wall x: " + x + " Room Wall Y: " + y;
                            break;
                        }
                }

                Arena[x, y].gameObject.name = Arena[x, y].gameObject.name + ": R = " + Cells[x, y].iRegion;
                Arena[x, y].gameObject.transform.position = new Vector3(CS_GameManager.fGridCellSize * x, -(CS_GameManager.fGridCellSize * 0.5f), CS_GameManager.fGridCellSize * y);
                if (Cells[x, y].type == CellType.START)
                {
                    playerStart = new Vector3(Arena[x, y].transform.position.x, player.transform.position.y, Arena[x, y].transform.position.z);
                    start = playerStart;
                }

                if(Cells[x, y].type == CellType.END)
                {
                    end = Arena[x, y].gameObject.transform.position;
                    Cells[x, y].type = CellType.EMPTY;
                }

                if(Cells[x, y].type == CellType.ENEMY)
                {
                    Cells[x, y].type = CellType.EMPTY;
                }
            }
        }

        // Adjust the floor to fit under the level
        FloorPrefab.transform.position = new Vector3(((iMaxCellsX * CS_GameManager.fGridCellSize) * 0.5f) - (CS_GameManager.fGridCellSize * 0.5f), -CS_GameManager.fGridCellSize, ((iMaxCellsY * CS_GameManager.fGridCellSize) * 0.5f) - (CS_GameManager.fGridCellSize * 0.5f));
        FloorPrefab.transform.localScale = new Vector3((iMaxCellsX * CS_GameManager.fGridCellSize) * 0.1f, 1, (iMaxCellsY * CS_GameManager.fGridCellSize) * 0.1f);

        // Fit the minimap floor to be the same
        MiniFloorPrefab.transform.position = FloorPrefab.transform.position;
        MiniFloorPrefab.transform.localScale = FloorPrefab.transform.localScale;

        // Adjust the ceiling to fit on top the level
        CeilingPrefab.transform.position = new Vector3(((iMaxCellsX * CS_GameManager.fGridCellSize) * 0.5f) - (CS_GameManager.fGridCellSize * 0.5f), 0, ((iMaxCellsY * CS_GameManager.fGridCellSize) * 0.5f) - (CS_GameManager.fGridCellSize * 0.5f));
        CeilingPrefab.transform.localScale = new Vector3((iMaxCellsX * CS_GameManager.fGridCellSize) * 0.1f, 1, (iMaxCellsY * CS_GameManager.fGridCellSize) * 0.1f);

        // Move the player to the start
        player.transform.position = playerStart;
    }

    private void PlaceLevelTriggers()
    {
        /// This is the start of choosing the start and end positions of the level in a more advanced way
        // Randomly find what quadrant we want to start in then *place end in opposite quadrent at a valid point* (Yet to be done)
        int newRand = rand.Next(0, 100);

        int iMinX = 0;
        int iMinY = 0;
        int iMaxX = 0;
        int iMaxY = 0;

        if (newRand <= 25)
        {
            // Bottom left
            iMinX = 1;
            iMinY = 1;
            iMaxX = Mathf.FloorToInt(iMaxCellsX * 0.20f);
            iMaxY = Mathf.FloorToInt(iMaxCellsY * 0.20f);
        }
        else if (newRand <= 50 && newRand > 25)
        {
            // Bottom Right
            iMinX = (iMaxCellsX - Mathf.FloorToInt(iMaxCellsX * 0.20f)) - 1;
            iMinY = 1;
            iMaxX = iMaxCellsX - 1;
            iMaxY = (Mathf.FloorToInt(iMaxCellsY * 0.20f)) + 1;
        }
        else if (newRand <= 75 && newRand > 50)
        {
            //Top left
            iMinX = 1;
            iMinY = (iMaxCellsY - Mathf.FloorToInt(iMaxCellsY * 0.20f)) - 1;
            iMaxX = Mathf.FloorToInt(iMaxCellsX * 0.20f);
            iMaxY = iMaxCellsY - 1;
        }
        else if (newRand <= 100 && newRand > 75)
        {
            // Top right
            iMinX = (iMaxCellsX - Mathf.FloorToInt(iMaxCellsX * 0.20f)) - 1;
            iMinY = (iMaxCellsY - Mathf.FloorToInt(iMaxCellsY * 0.20f)) - 1;
            iMaxX = iMaxCellsX - 1;
            iMaxY = iMaxCellsY - 1;
        }
        else
        {
            Debug.Log("Random out of range");
        }

        int startX = rand.Next(iMinX, iMaxX);
        int startY = rand.Next(iMinY, iMaxY);



        Cells[startX, startY].type = CellType.START;
        Cells[iMaxCellsX - startX, iMaxCellsY - startY].type = CellType.END;
    }

    private void RemoveDeadEnds()
    {
        // Iterate over all cells and see if the current cell is a dead end (Only has 1 exit)
        bool bComplete = false;

        while (!bComplete)
        {
            bComplete = true;

            for (int y = 1; y < iMaxCellsY - 2; y++)
            {
                for (int x = 1; x < iMaxCellsX - 2; x++)
                {
                    if (Cells[x, y].type == CellType.WALL || Cells[x, y].type == CellType.BOARDER)
                    {
                        continue;
                    }

                    int adjacentWalls = 0;

                    if (Cells[x + 1, y].type != CellType.EMPTY)
                    {
                        adjacentWalls++;
                    }

                    if (Cells[x - 1, y].type != CellType.EMPTY)
                    {
                        adjacentWalls++;
                    }

                    if (Cells[x, y + 1].type != CellType.EMPTY)
                    {
                        adjacentWalls++;
                    }

                    if (Cells[x, y - 1].type != CellType.EMPTY)
                    {
                        adjacentWalls++;
                    }

                    if (adjacentWalls < 3)
                    {
                        continue;
                    }

                    bComplete = false;
                    Cells[x, y].type = CellType.WALL;

                }
            }
        }
    }

    private void GrowMaze()
    {
        // Grow a maze using "Growing tree algorithm" on each possible cell
        for (int y = 0; y < iMaxCellsY - 1; y+=2)
        {
            for (int x = 0; x < iMaxCellsX - 1; x+=2)
            {
                if(Cells[x, y].type == CellType.WALL)
                {
                    GrowingTree(new Vector2(x, y));
                }
            }
        }
    }

    private void GrowingTree(Vector2 a_pos)
    {
        // Skip if this current cell isnt a wall
        if(Cells[(int)a_pos.x, (int)a_pos.y].type != CellType.WALL)
        {
            return;
        }

        // Set up the list of cells that need to be processed and initialise "Direction"
        List<Vector2> cells = new List<Vector2>();
        Direction lastDir = Direction.NULL;

        // Start Region
        Cells[(int)a_pos.x, (int)a_pos.y].type = CellType.EMPTY;
        cells.Add(a_pos);

        // While "Cells to be processed" is not empty
        while (cells.Count > 0)
        {
            // Get the newest cell added to the list
            Vector2 newestCell = cells[cells.Count - 1];

            List<Direction> unmade = new List<Direction>();

            for (int i = 0; i < 4; i++)
            {
                // Add every direction we can "tunnel" to unmade
                if(bCanCarve(newestCell, (Direction)i))
                {
                    unmade.Add((Direction)i);
                }
            }

            // If we have directions we can go, pick on or remove it and declare dead end
            if(unmade.Count > 0)
            {
                // Randomly test if the tunnels that are made are less windy
                Direction testDir;
                if (unmade.Contains(lastDir) && Random.Range(0f, 1f) < fWindingPercentage)
                {
                    testDir = lastDir;
                }
                else
                {
                    testDir = unmade[Random.Range(0, unmade.Count)];
                }

                lastDir = testDir;


                Carve(newestCell, testDir, 1);
                cells.Add(Carve(newestCell, testDir, 2));
            }
            else
            {
                cells.Remove(newestCell);
                lastDir = Direction.NULL;
            }
        }
    }

    private Vector2 Carve(Vector2 a_startPos, Direction a_dir, int a_count)
    {
        int x = (int)a_startPos.x;
        int y = (int)a_startPos.y;
        switch (a_dir)
        {
            case Direction.NORTH:
                {
                    Cells[x, y + a_count].type = CellType.EMPTY;
                    return new Vector2(x, y + a_count);
                }
            case Direction.SOUTH:
                {
                    Cells[x, y - a_count].type = CellType.EMPTY;
                    return new Vector2(x, y - a_count);
                }
            case Direction.EAST:
                {
                    Cells[x + a_count, y].type = CellType.EMPTY;
                    return new Vector2(x + a_count, y);
                }
            case Direction.WEST:
                {
                    Cells[x - a_count, y].type = CellType.EMPTY;
                    return new Vector2(x - a_count, y);
                }
        }
        return new Vector2(-1, -1);

    }

    bool bCanCarve(Vector2 a_pos, Direction a_dir)
    {
        int x = (int)a_pos.x;
        int y = (int)a_pos.y;

        switch(a_dir)
        {
            case Direction.NORTH:
                {
                    if((y + 3) > iMaxCellsY - 1)
                    {
                        return false;
                    }
                    if(Cells[x, y + 2].type != CellType.EMPTY)
                    {
                        return true;
                    }
                    break;
                }
            case Direction.SOUTH:
                {
                    if ((y - 3) <= 1)
                    {
                        return false;
                    }
                    if (Cells[x, y - 2].type != CellType.EMPTY)
                    {
                        return true;
                    }
                    break;
                }
            case Direction.EAST:
                {
                    if ((x + 3) > iMaxCellsX - 1)
                    {
                        return false;
                    }
                    if(Cells[x + 2, y].type != CellType.EMPTY)
                    {
                        return true;
                    }
                    break;
                }
            case Direction.WEST:
                {
                    if((x - 3) <= 1)
                    {
                        return false;
                    }
                    if(Cells[x - 2, y].type != CellType.EMPTY)
                    {
                        return true;
                    }
                    break;
                }
        }
        return false;
    }
}