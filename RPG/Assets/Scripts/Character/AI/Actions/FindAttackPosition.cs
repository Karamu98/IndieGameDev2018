using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class FindAttackPosition : BTAction
{
    List<Vector3> openSpaces;
    GameObject player;
    // Find path to enemy, set destination the next "Space"
    public FindAttackPosition(CS_Enemy owner) : base(owner)
    {
        player = CS_GameManager.player.gameObject;
        openSpaces = new List<Vector3>();
    }

    private void GetOpenPositions(Vector3 a_dir)
    {
        Vector3 newPos = player.transform.position + (a_dir * CS_GameManager.fGridCellSize);
        if (CS_GameManager.IsClear(newPos))
        {
            openSpaces.Add(newPos);
        }
    }

    public override BEHAVIOUR_STATUS Update()
    {
        openSpaces.Clear();
        GetOpenPositions(player.transform.forward);
        GetOpenPositions(-player.transform.forward);
        GetOpenPositions(player.transform.right);
        GetOpenPositions(-player.transform.right);

        // Stop if theres no spaces
        if(openSpaces.Count <= 0)
        {
            return BEHAVIOUR_STATUS.FAILURE;
        }

        Vector4 shortestRoute = new Vector4(2000, 2000, 2000, 2000);

        Vector4 test = new Vector4();

        // Find a shortest path to this cell
        foreach (Vector3 space in openSpaces)
        {
            if(space == GetOwner().transform.position)
            {
                return BEHAVIOUR_STATUS.FAILURE;
            }
            test = CS_GameManager.FindPath(GetOwner().gameObject.transform.position, space);

            if (test.w < shortestRoute.w)
            {
                shortestRoute = test;
            }
        }

        if(shortestRoute == new Vector4(-1, -1, -1, -1))
        {
            return BEHAVIOUR_STATUS.FAILURE;
        }
        Debug.Log(shortestRoute.x + ":" + shortestRoute.y + ":" + shortestRoute.z + ":" + shortestRoute.w);
        GetOwner().SetDestination(shortestRoute);
        return BEHAVIOUR_STATUS.SUCCESS;

    }
}
