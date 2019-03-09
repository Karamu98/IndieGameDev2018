using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.SceneManagement;
using UnityEngine;
using System;

public class CS_GameManager : MonoBehaviour
{
    public static float fGridCellSize = 2f;
    public static string Seed = "";
    public static CS_GameManager instance = null;
    public static List<CS_Character> characters = new List<CS_Character>();
    public static PathNode[,] navMesh;
    public static CS_StoreManager IAPManager;
    

    private static List<PathNode> m_FinalPath;

    public static CS_Player player;

    // Session keeping
    public static int iSessionKills;
    public static int iCurrentFloor;

    // Save load for highscore
    public static int iBestKills;
    public static int iBestFloor;

    // Statistics
    private static int iEnemiesKilled;
    private static int iFloorsBeaten;

    public static int iRevivePotions;


    // Persistient data
    private static int iCurrentHealth;
    public static bool bGameEnd;


	// Use this for initialization
	void Awake ()
    {
		if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);


        IAPManager = new CS_StoreManager();
        LoadSave();
        
        m_FinalPath = new List<PathNode>();

        if (Seed == null || Seed == "")
        {
            Seed = DateTime.Now.ToString();
        }
    }

    public static void NewGame()
    {
        bGameEnd = false;
        characters.Clear();
        m_FinalPath.Clear();
        iSessionKills = 0;
        iCurrentFloor = 0;
        iCurrentHealth = 100;
        SceneManager.LoadScene(1);
    }

    public static void SetPlayer(CS_Player a_character)
    {
        player = a_character;
        player.SetHealth(iCurrentHealth);
    }


    public static void PlayerDied()
    {
        foreach (CS_Character chara in characters)
        {
            chara.OnGameEnd();
        }
    }

    public static void AddCharacter(CS_Character a_character)
    {
        characters.Add(a_character);
    }

    public static void CharacterKilled(CS_Character a_character)
    {
        iSessionKills++;
        iEnemiesKilled++;
        characters.Remove(a_character);
    }

    public static void LevelComplete()
    {
        iCurrentHealth = player.GetHealth();
        foreach(CS_Character chara in characters)
        {
            chara.OnGameEnd();
        }
        characters.Clear();
        Seed = DateTime.Now.ToString();
        iCurrentFloor++;
        iFloorsBeaten++;

        // Reset input so that the player wont move straight away
        CrossPlatformInputManager.SetButtonUp("Forward");
        CrossPlatformInputManager.SetButtonUp("Left");
        CrossPlatformInputManager.SetButtonUp("Right");
        CrossPlatformInputManager.SetButtonUp("Backwards");

        // If current floor is divisible by 5
        if (iCurrentFloor % 3 == 0)
        {
            SceneManager.LoadScene(2);
        }
        else
        {
            SceneManager.LoadScene(1);
        }
    }

    private static void LoadSave()
    {
        iRevivePotions = PlayerPrefs.GetInt("Potions");

        iBestFloor = PlayerPrefs.GetInt("BestFloor");
        iBestKills = PlayerPrefs.GetInt("HighestKillCount");

        iEnemiesKilled = PlayerPrefs.GetInt("TotalKills");
        iFloorsBeaten = PlayerPrefs.GetInt("TotalFloors");
    }
	
	public static void StartGame(Cell[,] a_cells, Vector3 a_start, Vector3 a_end)
    {
        InitialiseNavData(a_cells);

        /// PATCH FIX TODO ---------------------------------------------------------------------------------------
        // This is a quick unoptimal way of testing if the level is completeable due to a unfixed bug TODO
        Vector4 path = FindPath(a_start, a_end);

        // If path is invalid to end
        if (path.w == -1)
        {
            // Retry
            SceneManager.LoadScene(1);
        }
        /// --------------------------------------------------------------------------------------------------------

        // After level gen, start game
        foreach (CS_Character chara in characters)
        {
            TakeSpace(chara.gameObject.transform.position);
            chara.OnGameStart();
        }
    }

    public static void ContinueGame()
    {
        characters.Clear();
        PlayerPrefs.SetInt("Potions", iRevivePotions);
        SceneManager.LoadScene(1);
    }

    public static void EndGame()
    {
        UpdateScores();
        bGameEnd = true;
        SceneManager.LoadScene(2);
    }

    private static void UpdateScores()
    {
        if(iCurrentFloor > iBestFloor)
        {
            iBestFloor = iCurrentFloor;
            CS_PlayServices.AddScoreToLeaderboard(KaramuResources.leaderboard_deepest_delver, iBestFloor);
        }

        if(iSessionKills > iBestKills)
        {
            iBestKills = iSessionKills;
            CS_PlayServices.AddScoreToLeaderboard(KaramuResources.leaderboard_killer, iBestKills);
        }

        iSessionKills = 0;
        iCurrentFloor = 0;

        PlayerPrefs.SetInt("HighestKillCount", iBestKills);
        PlayerPrefs.SetInt("BestFloor", iBestFloor);

        PlayerPrefs.SetInt("TotalKills", iEnemiesKilled);
        PlayerPrefs.SetInt("TotalFloors", iFloorsBeaten);
    }


    // Navigation grid and helper functions

    public static bool TakeSpace(Vector3 a_position)
    {
        Vector2Int pos = WorldToGrid(a_position);
        if(navMesh[pos.x, pos.y].bIsWall)
        {
            return false;
        }
        else
        {
            navMesh[pos.x, pos.y].bIsWall = true;
            return true;
        }
    }

    public static void ClearSpace(Vector3 a_position)
    {
        Vector2Int pos = WorldToGrid(a_position);
        navMesh[pos.x, pos.y].bIsWall = false;        
    }

    public static bool IsClear(Vector3 a_position)
    {
        Vector2Int pos = WorldToGrid(a_position);

        if(pos.x < 0 || pos.x > navMesh.GetLength(0) || pos.y < 0 || pos.y > navMesh.GetLength(1))
        {
            return false;
        }

        if (!navMesh[pos.x, pos.y].bIsWall)
        {
            return true;
        }

        return false;
    }

    private static void InitialiseNavData(Cell[,] a_cells)
    {
        navMesh = new PathNode[a_cells.GetLength(0), a_cells.GetLength(1)];

        for(int y = 0; y < navMesh.GetLength(1); y++)
        {
            for(int x = 0; x < navMesh.GetLength(0); x++)
            {
                navMesh[x, y] = new PathNode();
                navMesh[x, y].iXPos = x;
                navMesh[x, y].iYPos = y;
                navMesh[x, y].worldPos = new Vector3(fGridCellSize * x, -(fGridCellSize * 0.5f), fGridCellSize * y);

                if (a_cells[x, y].type != CellType.EMPTY)
                {
                    navMesh[x, y].bIsWall = true;
                }
            }
        }
    }

    // Returns a Vector3 with pos and a value giving steps away
    public static Vector4 FindPath(Vector3 a_startPosition, Vector3 a_targetDest)
    {
        Vector2Int startGridPos = WorldToGrid(a_startPosition);
        Vector2Int destGridPos = WorldToGrid(a_targetDest);
        PathNode startNode = navMesh[startGridPos.x, startGridPos.y];
        PathNode endNode = navMesh[destGridPos.x, destGridPos.y];

        List<PathNode> OpenList = new List<PathNode>();
        HashSet<PathNode> ClosedList = new HashSet<PathNode>();

        OpenList.Add(startNode);

        while(OpenList.Count > 0)
        {
            PathNode currentNode = OpenList[0];
            for(int i = 1; i < OpenList.Count; i++)
            {
                if(OpenList[i].iFCost < currentNode.iFCost || OpenList[i].iFCost == currentNode.iFCost && OpenList[i].iHCost < currentNode.iHCost)
                {
                    currentNode = OpenList[i];
                }
            }

            OpenList.Remove(currentNode);
            ClosedList.Add(currentNode);

            if(currentNode == endNode)
            {
                List<PathNode> path = GetFinalPath(startNode, endNode);
                if(path.Count <= 0)
                {
                    Vector3 pos = NodeToWorld(endNode);
                    return new Vector4(pos.x, -1, pos.z, endNode.iFCost);
                }
                return NodeToWorld(path[0]);
            }

            List<PathNode> Neighbors = GetNeighboringNodes(currentNode);
            foreach(PathNode NeighborNode in Neighbors)
            {
                if(NeighborNode == endNode && endNode.bIsWall)
                {
                    List<PathNode> path = GetFinalPath(startNode, currentNode);
                    if (path.Count <= 0)
                    {
                        Vector3 pos = NodeToWorld(endNode);
                        return new Vector4(pos.x, -1, pos.z, endNode.iFCost);
                    }
                    return NodeToWorld(path[0]);
                }

                if (NeighborNode.bIsWall || ClosedList.Contains(NeighborNode))
                {
                    continue;
                }

                int moveCost = currentNode.iGCost + GetManDistance(currentNode, NeighborNode);

                if(moveCost < NeighborNode.iGCost || !OpenList.Contains(NeighborNode))
                {
                    NeighborNode.iGCost = moveCost;
                    NeighborNode.iHCost = GetManDistance(NeighborNode, endNode);
                    NeighborNode.Parent = currentNode;

                    if(!OpenList.Contains(NeighborNode))
                    {
                        OpenList.Add(NeighborNode);
                    }
                }
            }
        }


        return new Vector4(-1, -1, -1, -1);
    }

    private static int GetManDistance(PathNode a_nodeA, PathNode a_nodeB)
    {
        int ix = Math.Abs(a_nodeA.iXPos - a_nodeB.iXPos);
        int iy = Math.Abs(a_nodeA.iYPos - a_nodeB.iYPos);

        return ix + iy;
    }

    private static List<PathNode> GetNeighboringNodes(PathNode a_start)
    {
        List<PathNode> NeighboringNodes = new List<PathNode>();

        int xCheck, yCheck;

        // Right Side
        xCheck = a_start.iXPos + 1;
        yCheck = a_start.iYPos;
        if(xCheck >= 0 && xCheck < navMesh.GetLength(0))
        {
            if(yCheck >= 0 && yCheck < navMesh.GetLength(1))
            {
                NeighboringNodes.Add(navMesh[xCheck, yCheck]);
            }
        }

        // Left Side
        xCheck = a_start.iXPos - 1;
        yCheck = a_start.iYPos;
        if (xCheck >= 0 && xCheck < navMesh.GetLength(0))
        {
            if (yCheck >= 0 && yCheck < navMesh.GetLength(1))
            {
                NeighboringNodes.Add(navMesh[xCheck, yCheck]);
            }
        }

        // Above
        xCheck = a_start.iXPos;
        yCheck = a_start.iYPos + 1;
        if (xCheck >= 0 && xCheck < navMesh.GetLength(0))
        {
            if (yCheck >= 0 && yCheck < navMesh.GetLength(1))
            {
                NeighboringNodes.Add(navMesh[xCheck, yCheck]);
            }
        }

        // Below
        xCheck = a_start.iXPos;
        yCheck = a_start.iYPos - 1;
        if (xCheck >= 0 && xCheck < navMesh.GetLength(0))
        {
            if (yCheck >= 0 && yCheck < navMesh.GetLength(1))
            {
                NeighboringNodes.Add(navMesh[xCheck, yCheck]);
            }
        }


        return NeighboringNodes;
    }

    private static List<PathNode> GetFinalPath(PathNode a_start, PathNode a_end)
    {
        List<PathNode> FinalPath = new List<PathNode>();
        PathNode CurrentNode = a_end;

        while(CurrentNode != a_start)
        {
            FinalPath.Add(CurrentNode);
            CurrentNode = CurrentNode.Parent;
        }

        FinalPath.Reverse();

        m_FinalPath = FinalPath;
        return FinalPath;
    }

    private static Vector4 NodeToWorld(PathNode a_node)
    {
        Vector4 result = new Vector4();

        int ixPos = Mathf.RoundToInt(a_node.iXPos * fGridCellSize);
        int iyPos = Mathf.RoundToInt(a_node.iYPos * fGridCellSize);


        result.x = ixPos;
        result.y = -1;
        result.z = iyPos;
        result.w = a_node.iFCost;

        return result;
    }

    private static Vector2Int WorldToGrid(Vector3 a_pos)
    {
        Vector2Int gridPos = new Vector2Int();

        // Convert world "Grid" to game grid
        gridPos.x = (int)Mathf.Round((a_pos.x / (navMesh.GetLength(0) * fGridCellSize)) * navMesh.GetLength(0));
        gridPos.y = (int)Mathf.Round((a_pos.z / (navMesh.GetLength(1) * fGridCellSize)) * navMesh.GetLength(1));

        return gridPos;
    }

    private void OnDrawGizmos()
    {

        Gizmos.DrawWireCube(transform.position, new Vector3(navMesh.GetLength(0), 1, navMesh.GetLength(1)));//Draw a wire cube with the given dimensions from the Unity inspector

        if (navMesh != null)//If the grid is not empty
        {
            foreach (PathNode n in navMesh)//Loop through every node in the grid
            {
                if (n.bIsWall)//If the current node is a wall node
                {
                    Gizmos.color = Color.white;//Set the color of the node
                }
                else
                {
                    Gizmos.color = Color.yellow;//Set the color of the node
                }


                if (m_FinalPath != null)//If the final path is not empty
                {
                    if (m_FinalPath.Contains(n))//If the current node is in the final path
                    {

                        Gizmos.color = Color.red;//Set the color of that node

                    }

                }

                Vector4 pos = NodeToWorld(n);
                Gizmos.DrawCube(new Vector3(pos.x, 1, pos.z), Vector3.one * fGridCellSize);//Draw the node at the position of the node.
            }
        }
    }
}
