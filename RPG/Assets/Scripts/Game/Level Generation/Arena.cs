using System.Collections.Generic;
using UnityEngine;

namespace Game.LevelGeneration
{
    public class Arena : MonoBehaviour
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
        [SerializeField] GameObject m_wallPrefab        = default;
        [SerializeField] GameObject m_miniFloorPrefab   = default;
        [SerializeField] GameObject m_ceilingPrefab     = default;
        [SerializeField] GameObject m_floorPrefab       = default;
        [SerializeField] GameObject m_chestPrefab       = default;
        [SerializeField] GameObject m_endPrefab         = default;
        [SerializeField] GameObject[] m_enemies         = default;

        // ========== Settings ====================
        [Range(0, 100)] public float ChestSpawnPercent  = 0.1f;
        [SerializeField] int m_maxCellsX                = 31;
        [SerializeField] int m_maxCellsY                = 31;
        [SerializeField] int m_roomAmount               = 50;
        [SerializeField] float m_windingPercentage      = 0.5f;
        [SerializeField] int m_roomMax                  = 6;
        [SerializeField] int m_roomMin                  = 4; // Cannot be less that 4 or will give hallway rooms


        // ============== Player ==============
        GameObject m_player;
        Vector3 m_playerStart = new Vector3();

        // ============= Data ==============
        System.Random rand;
        string m_seed;
        GameObject[,] m_arena;
        Cell[,] m_cells;
        List<Room> m_roomList = new List<Room>();

        // TEMPORARY BEFORE BUG FIX
        Vector3 m_start;
        Vector3 m_end;

        // ============= Required "Globals" =============
        int m_currentRegion = 0; // This is yet to be fully implemented
        int m_currentEnemy = 0;
        Vector2Int m_endCell;


        // Use this for initialization
        void Start()
        {
            // Grab the player before the inspector is filled with gameobjects
            m_player = GameObject.FindGameObjectWithTag("Player");

            // Grab our seed
            m_seed = GameManager.s_Seed;
            rand = new System.Random(m_seed.GetHashCode());

            // Initialise "Cells"
            m_cells = new Cell[m_maxCellsX, m_maxCellsY];
            InitialiseCells();

            // Layout map in cell data
            PlaceRooms(m_roomAmount, 200);
            GrowMaze();
            FindConnectors();
            ConnectRegions();
            RemoveDeadEnds();
            FindStart();
            FindEnd();
            PlaceEnemies();

            // Build
            m_arena = new GameObject[m_maxCellsX, m_maxCellsY];
            BuildLevel();

            GameManager.StartGame(m_cells, m_start, m_end);

            m_cells = null;
            m_arena = null;
        }

        void PlaceEnemies()
        {
            // Simple way of placing enemies in the way of the end point
            if (m_enemies.Length <= 0)
            {
                return;
            }
            // Cache to save processing
            int halfX = (int)(m_maxCellsX * 0.5f);
            int halfY = (int)(m_maxCellsY * 0.5f);

            bool bPlaced = false;

            for (int i = 0; i < m_enemies.Length; i++)
            {
                bPlaced = false;
                for (int y = m_endCell.y; y > halfY; y -= 2)
                {
                    if (bPlaced)
                    {
                        continue;
                    }
                    for (int x = m_endCell.x; x > halfX; x -= 2)
                    {
                        if (m_cells[x, y].Type == CellType.EMPTY)
                        {
                            m_cells[x, y].Type = CellType.ENEMY;
                            bPlaced = true;
                            break;
                        }
                    }
                }
            }
        }

        void FindStart()
        {
            // Quick simple way for finding a better start position (Find first open cell from bottom left)
            for (int y = 1; y < m_maxCellsY - 1; y++)
            {
                for (int x = 1; x < m_maxCellsX - 1; x++)
                {
                    if (m_cells[x, y].Type == CellType.EMPTY)
                    {
                        m_cells[x, y].Type = CellType.START;
                        return;
                    }
                }
            }
        }

        void FindEnd()
        {
            // Quick simple way for finding a better end position (Find first open cell from top right)
            for (int y = m_maxCellsY - 1; y > 1; y--)
            {
                for (int x = m_maxCellsX - 1; x > 1; x--)
                {
                    if (m_cells[x, y].Type == CellType.EMPTY)
                    {
                        m_cells[x, y].Type = CellType.END;
                        m_endCell = new Vector2Int(x, y);
                        return;
                    }
                }
            }
        }

        void ConnectRegions()
        {
            // Grab a room and break a connector
            Room currentRoom;
            int x, y;
            for (int i = 0; i < m_roomList.Count; i++)
            {
                currentRoom = m_roomList[i];
                Vector2 pos = currentRoom.GetConnector();
                x = (int)pos.x;
                y = (int)pos.y;

                m_cells[x, y].Type = CellType.EMPTY;
                m_cells[x, y].Region = m_currentRegion;
            }
        }

        void PlaceRooms(int a_amount, int a_maxAttempts)
        {
            for (int i = 0; i < a_amount; i++)
            {
                Room newRoom = new Room();
                for (int j = 0; j < a_maxAttempts; j++)
                {
                    newRoom.CreateRandomSizes(m_roomMin, m_roomMax);

                    // Get a random point in area to place room
                    int startX = Random.Range(1, m_maxCellsX - 1);
                    int startY = Random.Range(1, m_maxCellsY - 1);

                    newRoom.StartPos = new Vector2(startX, startY);


                    // Test if its in bounds
                    if ((startX - 1) + (newRoom.iRoomXSize + 2) < m_maxCellsX && (startY - 1) + (newRoom.iRoomYSize + 2) < m_maxCellsY)
                    {
                        // Test if it's overlapping other rooms
                        bool bIsValid = true;
                        for (int y = startY; y < startY + newRoom.iRoomYSize; y++)
                        {
                            for (int x = startX; x < startX + newRoom.iRoomXSize; x++)
                            {
                                if (m_cells[x, y].Type == CellType.EMPTY || m_cells[x, y].Type == CellType.BOARDER)
                                {
                                    bIsValid = false;
                                    continue;
                                }
                            }
                        }

                        if (bIsValid)
                        {
                            // If the room is valid, apply it to our map
                            newRoom.SetRoom(m_cells, m_currentRegion);
                            m_roomList.Add(newRoom);
                            m_currentRegion++;
                            j = a_maxAttempts;
                        }

                    }

                }
            }


        }

        void FindConnectors()
        {
            // Sets up all the connectors for rooms
            for (int i = 0; i < m_roomList.Count; i++)
            {
                m_roomList[i].FindConnectors(m_cells);
            }
        }

        void InitialiseCells()
        {
            // Iterate over all cells and make them a "wall" or a "boarder" if the cell is on the edge
            for (int y = 0; y < m_maxCellsY; y++)
            {
                for (int x = 0; x < m_maxCellsX; x++)
                {
                    if (x == 0 || x == m_maxCellsX - 1 || y == 0 || y == m_maxCellsY - 1)
                    {
                        m_cells[x, y] = new Cell
                        {
                            Type = CellType.BOARDER
                        };
                    }
                    else
                    {
                        m_cells[x, y] = new Cell
                        {
                            Type = CellType.WALL
                        };
                    }
                }
            }
        }

        void BuildLevel()
        {
            // Iterate over all the cells and place the appropiate block based on the cell types
            for (int y = 0; y < m_maxCellsY; y++)
            {
                for (int x = 0; x < m_maxCellsX; x++)
                {
                    switch (m_cells[x, y].Type)
                    {
                        case CellType.WALL:
                            {
                                m_arena[x, y] = Instantiate(m_wallPrefab, gameObject.transform);
                                m_arena[x, y].gameObject.transform.localScale = new Vector3(GameManager.s_GridCellSize, GameManager.s_GridCellSize, GameManager.s_GridCellSize);
                                m_arena[x, y].gameObject.name = "Wall x: " + x + " Wall Y: " + y;
                                break;
                            }

                        case CellType.CHEST:
                            {
                                m_arena[x, y] = Instantiate(m_chestPrefab, gameObject.transform);
                                m_arena[x, y].gameObject.name = "Chest x: " + x + " Chest Y: " + y;
                                break;
                            }
                        case CellType.EMPTY:
                            {
                                m_arena[x, y] = new GameObject();
                                m_arena[x, y].transform.parent = gameObject.transform;
                                m_arena[x, y].gameObject.name = "Empty x: " + x + " Empty Y: " + y;
                                break;
                            }
                        case CellType.ENEMY:
                            {
                                m_arena[x, y] = Instantiate(m_enemies[m_currentEnemy], gameObject.transform);
                                m_arena[x, y].gameObject.name = "Enemy: " + (m_currentEnemy + 1);
                                m_currentEnemy++;
                                break;
                            }
                        case CellType.BOARDER:
                            {
                                m_arena[x, y] = Instantiate(m_wallPrefab, gameObject.transform);
                                m_arena[x, y].gameObject.transform.localScale = new Vector3(GameManager.s_GridCellSize, GameManager.s_GridCellSize, GameManager.s_GridCellSize);
                                m_arena[x, y].gameObject.name = "Boarder x: " + x + " Boarder Y: " + y;
                                m_arena[x, y].gameObject.transform.position = new Vector3(GameManager.s_GridCellSize * x, -(GameManager.s_GridCellSize * 0.5f), GameManager.s_GridCellSize * y);
                                break;
                            }
                        case CellType.END:
                            {
                                m_arena[x, y] = Instantiate(m_endPrefab, gameObject.transform);
                                m_arena[x, y].gameObject.transform.localScale = new Vector3(GameManager.s_GridCellSize, GameManager.s_GridCellSize, GameManager.s_GridCellSize);
                                m_arena[x, y].gameObject.name = "End x: " + x + " End Y: " + y;
                                m_arena[x, y].gameObject.transform.position = new Vector3(GameManager.s_GridCellSize * x, -(GameManager.s_GridCellSize * 0.5f), GameManager.s_GridCellSize * y);
                                break;
                            }
                        case CellType.START:
                            {
                                m_arena[x, y] = new GameObject();
                                m_arena[x, y].transform.parent = gameObject.transform;
                                m_arena[x, y].gameObject.name = "Start x: " + x + " Start Y: " + y;
                                break;
                            }
                        case CellType.ROOMCONNECTOR:
                            {
                                m_arena[x, y] = Instantiate(m_wallPrefab, gameObject.transform);
                                m_arena[x, y].gameObject.transform.localScale = new Vector3(GameManager.s_GridCellSize, GameManager.s_GridCellSize, GameManager.s_GridCellSize);
                                m_arena[x, y].gameObject.name = "Room Connector x: " + x + " Room Connector Y: " + y;
                                break;
                            }
                        case CellType.ROOMWALL:
                            {
                                m_arena[x, y] = Instantiate(m_wallPrefab, gameObject.transform);
                                m_arena[x, y].gameObject.transform.localScale = new Vector3(GameManager.s_GridCellSize, GameManager.s_GridCellSize, GameManager.s_GridCellSize);
                                m_arena[x, y].gameObject.name = "Room Wall x: " + x + " Room Wall Y: " + y;
                                break;
                            }
                    }

                    m_arena[x, y].gameObject.name = m_arena[x, y].gameObject.name + ": R = " + m_cells[x, y].Region;
                    m_arena[x, y].gameObject.transform.position = new Vector3(GameManager.s_GridCellSize * x, -(GameManager.s_GridCellSize * 0.5f), GameManager.s_GridCellSize * y);
                    if (m_cells[x, y].Type == CellType.START)
                    {
                        m_playerStart = new Vector3(m_arena[x, y].transform.position.x, m_player.transform.position.y, m_arena[x, y].transform.position.z);
                        m_start = m_playerStart;
                    }

                    if (m_cells[x, y].Type == CellType.END)
                    {
                        m_end = m_arena[x, y].gameObject.transform.position;
                        m_cells[x, y].Type = CellType.EMPTY;
                    }

                    if (m_cells[x, y].Type == CellType.ENEMY)
                    {
                        m_cells[x, y].Type = CellType.EMPTY;
                    }
                }
            }

            // Adjust the floor to fit under the level
            m_floorPrefab.transform.position = new Vector3(((m_maxCellsX * GameManager.s_GridCellSize) * 0.5f) - (GameManager.s_GridCellSize * 0.5f), -GameManager.s_GridCellSize, ((m_maxCellsY * GameManager.s_GridCellSize) * 0.5f) - (GameManager.s_GridCellSize * 0.5f));
            m_floorPrefab.transform.localScale = new Vector3((m_maxCellsX * GameManager.s_GridCellSize) * 0.1f, 1, (m_maxCellsY * GameManager.s_GridCellSize) * 0.1f);

            // Fit the minimap floor to be the same
            m_miniFloorPrefab.transform.position = m_floorPrefab.transform.position;
            m_miniFloorPrefab.transform.localScale = m_floorPrefab.transform.localScale;

            // Adjust the ceiling to fit on top the level
            m_ceilingPrefab.transform.position = new Vector3(((m_maxCellsX * GameManager.s_GridCellSize) * 0.5f) - (GameManager.s_GridCellSize * 0.5f), 0, ((m_maxCellsY * GameManager.s_GridCellSize) * 0.5f) - (GameManager.s_GridCellSize * 0.5f));
            m_ceilingPrefab.transform.localScale = new Vector3((m_maxCellsX * GameManager.s_GridCellSize) * 0.1f, 1, (m_maxCellsY * GameManager.s_GridCellSize) * 0.1f);

            // Move the player to the start
            m_player.transform.position = m_playerStart;
        }

        void PlaceLevelTriggers()
        {
            /// This is the start of choosing the start and end positions of the level in a more advanced way
            // Randomly find what quadrant we want to start in then *place end in opposite quadrent at a valid point* (Yet to be done)
            int newRand = rand.Next(0, 100);

            int minX = 0;
            int minY = 0;
            int maxX = 0;
            int maxY = 0;

            if (newRand <= 25)
            {
                // Bottom left
                minX = 1;
                minY = 1;
                maxX = Mathf.FloorToInt(m_maxCellsX * 0.20f);
                maxY = Mathf.FloorToInt(m_maxCellsY * 0.20f);
            }
            else if (newRand <= 50 && newRand > 25)
            {
                // Bottom Right
                minX = (m_maxCellsX - Mathf.FloorToInt(m_maxCellsX * 0.20f)) - 1;
                minY = 1;
                maxX = m_maxCellsX - 1;
                maxY = (Mathf.FloorToInt(m_maxCellsY * 0.20f)) + 1;
            }
            else if (newRand <= 75 && newRand > 50)
            {
                //Top left
                minX = 1;
                minY = (m_maxCellsY - Mathf.FloorToInt(m_maxCellsY * 0.20f)) - 1;
                maxX = Mathf.FloorToInt(m_maxCellsX * 0.20f);
                maxY = m_maxCellsY - 1;
            }
            else if (newRand <= 100 && newRand > 75)
            {
                // Top right
                minX = (m_maxCellsX - Mathf.FloorToInt(m_maxCellsX * 0.20f)) - 1;
                minY = (m_maxCellsY - Mathf.FloorToInt(m_maxCellsY * 0.20f)) - 1;
                maxX = m_maxCellsX - 1;
                maxY = m_maxCellsY - 1;
            }
            else
            {
                Debug.Log("Random out of range");
            }

            int startX = rand.Next(minX, maxX);
            int startY = rand.Next(minY, maxY);



            m_cells[startX, startY].Type = CellType.START;
            m_cells[m_maxCellsX - startX, m_maxCellsY - startY].Type = CellType.END;
        }

        void RemoveDeadEnds()
        {
            // Iterate over all cells and see if the current cell is a dead end (Only has 1 exit)
            bool isComplete = false;

            while (!isComplete)
            {
                isComplete = true;

                for (int y = 1; y < m_maxCellsY - 2; y++)
                {
                    for (int x = 1; x < m_maxCellsX - 2; x++)
                    {
                        if (m_cells[x, y].Type == CellType.WALL || m_cells[x, y].Type == CellType.BOARDER)
                        {
                            continue;
                        }

                        int adjacentWalls = 0;

                        if (m_cells[x + 1, y].Type != CellType.EMPTY)
                        {
                            adjacentWalls++;
                        }

                        if (m_cells[x - 1, y].Type != CellType.EMPTY)
                        {
                            adjacentWalls++;
                        }

                        if (m_cells[x, y + 1].Type != CellType.EMPTY)
                        {
                            adjacentWalls++;
                        }

                        if (m_cells[x, y - 1].Type != CellType.EMPTY)
                        {
                            adjacentWalls++;
                        }

                        if (adjacentWalls < 3)
                        {
                            continue;
                        }

                        isComplete = false;
                        m_cells[x, y].Type = CellType.WALL;

                    }
                }
            }
        }

        void GrowMaze()
        {
            // Grow a maze using "Growing tree algorithm" on each possible cell
            for (int y = 0; y < m_maxCellsY - 1; y += 2)
            {
                for (int x = 0; x < m_maxCellsX - 1; x += 2)
                {
                    if (m_cells[x, y].Type == CellType.WALL)
                    {
                        GrowingTree(new Vector2(x, y));
                    }
                }
            }
        }

        void GrowingTree(Vector2 a_pos)
        {
            // Skip if this current cell isnt a wall
            if (m_cells[(int)a_pos.x, (int)a_pos.y].Type != CellType.WALL)
            {
                return;
            }

            // Set up the list of cells that need to be processed and initialise "Direction"
            List<Vector2> cells = new List<Vector2>();
            Direction lastDir = Direction.NULL;

            // Start Region
            m_cells[(int)a_pos.x, (int)a_pos.y].Type = CellType.EMPTY;
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
                    if (CanCarve(newestCell, (Direction)i))
                    {
                        unmade.Add((Direction)i);
                    }
                }

                // If we have directions we can go, pick on or remove it and declare dead end
                if (unmade.Count > 0)
                {
                    // Randomly test if the tunnels that are made are less windy
                    Direction testDir;
                    if (unmade.Contains(lastDir) && Random.Range(0f, 1f) < m_windingPercentage)
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

        Vector2 Carve(Vector2 a_startPos, Direction a_dir, int a_count)
        {
            int x = (int)a_startPos.x;
            int y = (int)a_startPos.y;
            switch (a_dir)
            {
                case Direction.NORTH:
                    {
                        m_cells[x, y + a_count].Type = CellType.EMPTY;
                        return new Vector2(x, y + a_count);
                    }
                case Direction.SOUTH:
                    {
                        m_cells[x, y - a_count].Type = CellType.EMPTY;
                        return new Vector2(x, y - a_count);
                    }
                case Direction.EAST:
                    {
                        m_cells[x + a_count, y].Type = CellType.EMPTY;
                        return new Vector2(x + a_count, y);
                    }
                case Direction.WEST:
                    {
                        m_cells[x - a_count, y].Type = CellType.EMPTY;
                        return new Vector2(x - a_count, y);
                    }
            }
            return new Vector2(-1, -1);

        }

        bool CanCarve(Vector2 a_pos, Direction a_dir)
        {
            int x = (int)a_pos.x;
            int y = (int)a_pos.y;

            switch (a_dir)
            {
                case Direction.NORTH:
                    {
                        if ((y + 3) > m_maxCellsY - 1)
                        {
                            return false;
                        }
                        if (m_cells[x, y + 2].Type != CellType.EMPTY)
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
                        if (m_cells[x, y - 2].Type != CellType.EMPTY)
                        {
                            return true;
                        }
                        break;
                    }
                case Direction.EAST:
                    {
                        if ((x + 3) > m_maxCellsX - 1)
                        {
                            return false;
                        }
                        if (m_cells[x + 2, y].Type != CellType.EMPTY)
                        {
                            return true;
                        }
                        break;
                    }
                case Direction.WEST:
                    {
                        if ((x - 3) <= 1)
                        {
                            return false;
                        }
                        if (m_cells[x - 2, y].Type != CellType.EMPTY)
                        {
                            return true;
                        }
                        break;
                    }
            }
            return false;
        }

        public class Pathfinding
        {

        }
    }
}
