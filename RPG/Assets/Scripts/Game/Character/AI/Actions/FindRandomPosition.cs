using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

namespace Game.AI
{
    public class FindRandomPosition : BTAction
    {
        int m_wanderPercentage = 90;

        Vector3 m_position;
        Vector3 m_forward;
        Vector3 m_right;
        List<Vector3> m_openSpaces;
        Vector3 m_lastDir;

        public FindRandomPosition(Enemy owner) : base(owner)
        {
            m_openSpaces = new List<Vector3>();
        }

        private void TestDirection(Vector3 a_dir)
        {
            Vector3 newPos = m_position + (a_dir * GameManager.s_GridCellSize);
            if (GameManager.IsClear(newPos))
            {
                m_openSpaces.Add(a_dir);
            }
        }

        public override BEHAVIOUR_STATUS Update()
        {
            // Update cached
            m_position = GetOwner().gameObject.transform.position;
            m_forward = GetOwner().gameObject.transform.forward;
            m_right = GetOwner().gameObject.transform.right;

            // To stop last tests leaking into this test
            if (m_openSpaces.Count > 0)
            {
                m_openSpaces.Clear();
            }

            // Test all directions
            TestDirection(m_forward);
            TestDirection(-m_forward);
            TestDirection(m_right);
            TestDirection(-m_right);


            // If the agent can move
            if (m_openSpaces.Count <= 0)
            {
                Debug.Log("No Spaces");
                return BEHAVIOUR_STATUS.FAILURE;
            }


            // Test if the agent will follow its old direction
            if (m_openSpaces.Contains(m_lastDir) && Random.Range(0, 100) < m_wanderPercentage)
            {
                //GetOwner().SetDestination(GetOwner().transform.position + (new Vector3(Mathf.RoundToInt(m_lastDir.x), Mathf.RoundToInt(m_lastDir.y), Mathf.RoundToInt(m_lastDir.z)) * GameManager.s_GridCellSize));
            }
            else
            {
                // If the previous direction exists, remove it
                if (m_openSpaces.Contains(m_lastDir))
                {
                    m_openSpaces.Remove(m_lastDir);

                    if (m_openSpaces.Count <= 0)
                    {
                        return BEHAVIOUR_STATUS.FAILURE;
                    }
                }

                // Set new direction
                Vector3 dir = m_openSpaces[Random.Range(0, m_openSpaces.Count)];
                m_lastDir = dir;
                //GetOwner().SetDestination(GetOwner().transform.position + (new Vector3(Mathf.RoundToInt(dir.x), Mathf.RoundToInt(dir.y), Mathf.RoundToInt(dir.z)) * GameManager.s_GridCellSize));
            }

            return BEHAVIOUR_STATUS.SUCCESS;
        }
    }
}