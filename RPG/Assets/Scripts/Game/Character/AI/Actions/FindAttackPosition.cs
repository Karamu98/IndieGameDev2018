using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

namespace Game.AI
{
    public class FindAttackPosition : BTAction
    {
        List<Vector3> m_openSpaces;
        GameObject m_player;
        // Find path to enemy, set destination the next "Space"
        public FindAttackPosition(Enemy owner) : base(owner)
        {
            m_player = GameManager.s_Player.gameObject;
            m_openSpaces = new List<Vector3>();
        }

        private void GetOpenPositions(Vector3 a_dir)
        {
            Vector3 newPos = m_player.transform.position + (a_dir * GameManager.s_GridCellSize);
            if (GameManager.IsClear(newPos))
            {
                m_openSpaces.Add(newPos);
            }
        }

        public override BEHAVIOUR_STATUS Update()
        {
            m_openSpaces.Clear();
            GetOpenPositions(m_player.transform.forward);
            GetOpenPositions(-m_player.transform.forward);
            GetOpenPositions(m_player.transform.right);
            GetOpenPositions(-m_player.transform.right);

            // Stop if theres no spaces
            if (m_openSpaces.Count <= 0)
            {
                return BEHAVIOUR_STATUS.FAILURE;
            }

            Vector4 shortestRoute = new Vector4(2000, 2000, 2000, 2000);

            Vector4 test = new Vector4();

            // Find a shortest path to this cell
            foreach (Vector3 space in m_openSpaces)
            {
                if (space == GetOwner().transform.position)
                {
                    return BEHAVIOUR_STATUS.FAILURE;
                }
                test = GameManager.FindPath(GetOwner().gameObject.transform.position, space);

                if (test.w < shortestRoute.w)
                {
                    shortestRoute = test;
                }
            }

            if (shortestRoute == new Vector4(-1, -1, -1, -1))
            {
                return BEHAVIOUR_STATUS.FAILURE;
            }
            Debug.Log(shortestRoute.x + ":" + shortestRoute.y + ":" + shortestRoute.z + ":" + shortestRoute.w);
            //GetOwner().SetDestination(shortestRoute);
            return BEHAVIOUR_STATUS.SUCCESS;

        }
    }
}

