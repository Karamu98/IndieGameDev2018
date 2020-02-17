using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using System;

namespace Game
{
    public class GameManager : MonoBehaviour
    {
        public static float s_GridCellSize = 2f;
        public static string s_Seed = "";
        public static GameManager s_instance = null;
        public static List<Character> s_characters = new List<Character>();
        public static PathNode[,] s_navMesh;


        static List<PathNode> s_finalPath;

        public static Player s_Player;

        // Session keeping
        public static int s_SessionKills;
        public static int s_CurrentFloor;

        // Save load for highscore
        public static int s_BestKills;
        public static int s_BestFloor;

        // Statistics
        static int s_enemiesKilled;
        static int s_floorsBeaten;

        public static int s_RevivePotions;


        // Persistient data
        static int s_currentHealth;
        public static bool m_HasGameEnded;


        // Use this for initialization
        void Awake()
        {
            if (s_instance == null)
            {
                s_instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            DontDestroyOnLoad(gameObject);


            LoadSave();

            s_finalPath = new List<PathNode>();

            if (s_Seed == null || s_Seed == "")
            {
                s_Seed = DateTime.Now.ToString();
            }
        }

        public static void NewGame()
        {
            m_HasGameEnded = false;
            s_characters.Clear();
            s_finalPath.Clear();
            s_SessionKills = 0;
            s_CurrentFloor = 0;
            s_currentHealth = 100;
            SceneManager.LoadScene(1);
        }

        public static void SetPlayer(Player a_character)
        {
            s_Player = a_character;
            s_Player.SetHealth(s_currentHealth);
        }


        public static void PlayerDied()
        {
            foreach (Character chara in s_characters)
            {
                chara.OnGameEnd();
            }
        }

        public static void AddCharacter(Character a_character)
        {
            s_characters.Add(a_character);
        }

        public static void CharacterKilled(Character a_character)
        {
            s_SessionKills++;
            s_enemiesKilled++;
            s_characters.Remove(a_character);
        }

        public static void LevelComplete()
        {
            s_currentHealth = s_Player.GetHealth();
            foreach (Character chara in s_characters)
            {
                chara.OnGameEnd();
            }
            s_characters.Clear();
            s_Seed = DateTime.Now.ToString();
            s_CurrentFloor++;
            s_floorsBeaten++;

            // If current floor is divisible by 5
            if (s_CurrentFloor % 3 == 0)
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
            s_RevivePotions = PlayerPrefs.GetInt("Potions");

            s_BestFloor = PlayerPrefs.GetInt("BestFloor");
            s_BestKills = PlayerPrefs.GetInt("HighestKillCount");

            s_enemiesKilled = PlayerPrefs.GetInt("TotalKills");
            s_floorsBeaten = PlayerPrefs.GetInt("TotalFloors");
        }

        public static void StartGame(Game.LevelGeneration.Cell[,] a_cells, Vector3 a_start, Vector3 a_end)
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
            foreach (Character chara in s_characters)
            {
                TakeSpace(chara.gameObject.transform.position);
                chara.OnGameStart();
            }
        }

        public static void ContinueGame()
        {
            s_characters.Clear();
            PlayerPrefs.SetInt("Potions", s_RevivePotions);
            SceneManager.LoadScene(1);
        }

        public static void EndGame()
        {
            UpdateScores();
            m_HasGameEnded = true;
            SceneManager.LoadScene(2);
        }

        private static void UpdateScores()
        {
            //if (iCurrentFloor > iBestFloor)
            //{
            //    iBestFloor = iCurrentFloor;
            //    CS_PlayServices.AddScoreToLeaderboard(KaramuResources.leaderboard_deepest_delver, iBestFloor);
            //}

            //if (iSessionKills > iBestKills)
            //{
            //    iBestKills = iSessionKills;
            //    CS_PlayServices.AddScoreToLeaderboard(KaramuResources.leaderboard_killer, iBestKills);
            //}

            s_SessionKills = 0;
            s_CurrentFloor = 0;

            PlayerPrefs.SetInt("HighestKillCount", s_BestKills);
            PlayerPrefs.SetInt("BestFloor", s_BestFloor);

            PlayerPrefs.SetInt("TotalKills", s_enemiesKilled);
            PlayerPrefs.SetInt("TotalFloors", s_floorsBeaten);
        }


        // Navigation grid and helper functions

        public static bool TakeSpace(Vector3 a_position)
        {
            Vector2Int pos = WorldToGrid(a_position);
            if (s_navMesh[pos.x, pos.y].IsWall)
            {
                return false;
            }
            else
            {
                s_navMesh[pos.x, pos.y].IsWall = true;
                return true;
            }
        }

        public static Vector2Int GetGridPos(Vector3 position)
        {
            return WorldToGrid(position);
        }

        public static void ClearSpace(Vector3 a_position)
        {
            Vector2Int pos = WorldToGrid(a_position);
            s_navMesh[pos.x, pos.y].IsWall = false;
        }

        public static bool IsClear(Vector3 a_position)
        {
            Vector2Int pos = WorldToGrid(a_position);

            if (pos.x < 0 || pos.x > s_navMesh.GetLength(0) || pos.y < 0 || pos.y > s_navMesh.GetLength(1))
            {
                return false;
            }

            if (!s_navMesh[pos.x, pos.y].IsWall)
            {
                return true;
            }

            return false;
        }

        private static void InitialiseNavData(LevelGeneration.Cell[,] a_cells)
        {
            s_navMesh = new PathNode[a_cells.GetLength(0), a_cells.GetLength(1)];

            for (int y = 0; y < s_navMesh.GetLength(1); y++)
            {
                for (int x = 0; x < s_navMesh.GetLength(0); x++)
                {
                    s_navMesh[x, y] = new PathNode
                    {
                        iXPos = x,
                        iYPos = y,
                        worldPos = new Vector3(s_GridCellSize * x, -(s_GridCellSize * 0.5f), s_GridCellSize * y)
                    };

                    if (a_cells[x, y].Type != LevelGeneration.CellType.EMPTY)
                    {
                        s_navMesh[x, y].IsWall = true;
                    }
                }
            }
        }

        // Returns a Vector3 with pos and a value giving steps away
        public static Vector4 FindPath(Vector3 a_startPosition, Vector3 a_targetDest)
        {
            Vector2Int startGridPos = WorldToGrid(a_startPosition);
            Vector2Int destGridPos = WorldToGrid(a_targetDest);
            PathNode startNode = s_navMesh[startGridPos.x, startGridPos.y];
            PathNode endNode = s_navMesh[destGridPos.x, destGridPos.y];

            List<PathNode> OpenList = new List<PathNode>();
            HashSet<PathNode> ClosedList = new HashSet<PathNode>();

            OpenList.Add(startNode);

            while (OpenList.Count > 0)
            {
                PathNode currentNode = OpenList[0];
                for (int i = 1; i < OpenList.Count; i++)
                {
                    if (OpenList[i].iFCost < currentNode.iFCost || OpenList[i].iFCost == currentNode.iFCost && OpenList[i].iHCost < currentNode.iHCost)
                    {
                        currentNode = OpenList[i];
                    }
                }

                OpenList.Remove(currentNode);
                ClosedList.Add(currentNode);

                if (currentNode == endNode)
                {
                    List<PathNode> path = GetFinalPath(startNode, endNode);
                    if (path.Count <= 0)
                    {
                        Vector3 pos = NodeToWorld(endNode);
                        return new Vector4(pos.x, -1, pos.z, endNode.iFCost);
                    }
                    return NodeToWorld(path[0]);
                }

                List<PathNode> Neighbors = GetNeighboringNodes(currentNode);
                foreach (PathNode NeighborNode in Neighbors)
                {
                    if (NeighborNode == endNode && endNode.IsWall)
                    {
                        List<PathNode> path = GetFinalPath(startNode, currentNode);
                        if (path.Count <= 0)
                        {
                            Vector3 pos = NodeToWorld(endNode);
                            return new Vector4(pos.x, -1, pos.z, endNode.iFCost);
                        }
                        return NodeToWorld(path[0]);
                    }

                    if (NeighborNode.IsWall || ClosedList.Contains(NeighborNode))
                    {
                        continue;
                    }

                    int moveCost = currentNode.iGCost + GetManDistance(currentNode, NeighborNode);

                    if (moveCost < NeighborNode.iGCost || !OpenList.Contains(NeighborNode))
                    {
                        NeighborNode.iGCost = moveCost;
                        NeighborNode.iHCost = GetManDistance(NeighborNode, endNode);
                        NeighborNode.Parent = currentNode;

                        if (!OpenList.Contains(NeighborNode))
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
            if (xCheck >= 0 && xCheck < s_navMesh.GetLength(0))
            {
                if (yCheck >= 0 && yCheck < s_navMesh.GetLength(1))
                {
                    NeighboringNodes.Add(s_navMesh[xCheck, yCheck]);
                }
            }

            // Left Side
            xCheck = a_start.iXPos - 1;
            yCheck = a_start.iYPos;
            if (xCheck >= 0 && xCheck < s_navMesh.GetLength(0))
            {
                if (yCheck >= 0 && yCheck < s_navMesh.GetLength(1))
                {
                    NeighboringNodes.Add(s_navMesh[xCheck, yCheck]);
                }
            }

            // Above
            xCheck = a_start.iXPos;
            yCheck = a_start.iYPos + 1;
            if (xCheck >= 0 && xCheck < s_navMesh.GetLength(0))
            {
                if (yCheck >= 0 && yCheck < s_navMesh.GetLength(1))
                {
                    NeighboringNodes.Add(s_navMesh[xCheck, yCheck]);
                }
            }

            // Below
            xCheck = a_start.iXPos;
            yCheck = a_start.iYPos - 1;
            if (xCheck >= 0 && xCheck < s_navMesh.GetLength(0))
            {
                if (yCheck >= 0 && yCheck < s_navMesh.GetLength(1))
                {
                    NeighboringNodes.Add(s_navMesh[xCheck, yCheck]);
                }
            }


            return NeighboringNodes;
        }

        private static List<PathNode> GetFinalPath(PathNode a_start, PathNode a_end)
        {
            List<PathNode> FinalPath = new List<PathNode>();
            PathNode CurrentNode = a_end;

            while (CurrentNode != a_start)
            {
                FinalPath.Add(CurrentNode);
                CurrentNode = CurrentNode.Parent;
            }

            FinalPath.Reverse();

            s_finalPath = FinalPath;
            return FinalPath;
        }

        private static Vector4 NodeToWorld(PathNode a_node)
        {
            Vector4 result = new Vector4();

            int ixPos = Mathf.RoundToInt(a_node.iXPos * s_GridCellSize);
            int iyPos = Mathf.RoundToInt(a_node.iYPos * s_GridCellSize);


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
            gridPos.x = Mathf.RoundToInt((a_pos.x / (s_navMesh.GetLength(0) * s_GridCellSize)) * s_navMesh.GetLength(0));
            gridPos.y = Mathf.RoundToInt((a_pos.z / (s_navMesh.GetLength(1) * s_GridCellSize)) * s_navMesh.GetLength(1));

            return gridPos;
        }

        private void OnDrawGizmos()
        {

            Gizmos.DrawWireCube(transform.position, new Vector3(s_navMesh.GetLength(0), 1, s_navMesh.GetLength(1)));//Draw a wire cube with the given dimensions from the Unity inspector

            if (s_navMesh != null)//If the grid is not empty
            {
                foreach (PathNode n in s_navMesh)//Loop through every node in the grid
                {
                    if (n.IsWall)//If the current node is a wall node
                    {
                        Gizmos.color = Color.white;//Set the color of the node
                    }
                    else
                    {
                        Gizmos.color = Color.yellow;//Set the color of the node
                    }


                    if (s_finalPath != null)//If the final path is not empty
                    {
                        if (s_finalPath.Contains(n))//If the current node is in the final path
                        {

                            Gizmos.color = Color.red;//Set the color of that node

                        }

                    }

                    Vector4 pos = NodeToWorld(n);
                    Gizmos.DrawCube(new Vector3(pos.x, 1, pos.z), Vector3.one * s_GridCellSize);//Draw the node at the position of the node.
                }
            }
        }
    }
}