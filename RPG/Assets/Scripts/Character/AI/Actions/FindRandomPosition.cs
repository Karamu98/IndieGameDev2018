using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class FindRandomPosition : BTAction
{
    int iWanderPercentage = 90;

    Vector3 position; 
    Vector3 forward;
    Vector3 right;
    List<Vector3> openSpaces;
    Vector3 lastDir;

    public FindRandomPosition(CS_Enemy owner) : base(owner)
    {
        openSpaces = new List<Vector3>();
    }

    private void TestDirection(Vector3 a_dir)
    {
        Vector3 newPos = position + (a_dir * CS_GameManager.fGridCellSize);
        if(CS_GameManager.IsClear(newPos))
        {
            openSpaces.Add(a_dir);
        }
    }

    public override BEHAVIOUR_STATUS Update()
    {
        // Update cached
        position = GetOwner().gameObject.transform.position;
        forward = GetOwner().gameObject.transform.forward;
        right = GetOwner().gameObject.transform.right;

        // To stop last tests leaking into this test
        if(openSpaces.Count > 0)
        {
            openSpaces.Clear();
        }

        // Test all directions
        TestDirection(forward);
        TestDirection(-forward);
        TestDirection(right);
        TestDirection(-right);


        // If the agent can move
        if(openSpaces.Count <= 0)
        {
            Debug.Log("No Spaces");
            return BEHAVIOUR_STATUS.FAILURE;
        }


        // Test if the agent will follow its old direction
        if(openSpaces.Contains(lastDir) && Random.Range(0, 100) < iWanderPercentage)
        {
            GetOwner().SetDestination(GetOwner().transform.position + (new Vector3(Mathf.RoundToInt(lastDir.x), Mathf.RoundToInt(lastDir.y), Mathf.RoundToInt(lastDir.z)) * CS_GameManager.fGridCellSize));
        }
        else
        {
            // If the previous direction exists, remove it
            if(openSpaces.Contains(lastDir))
            {
                openSpaces.Remove(lastDir);

                if(openSpaces.Count <= 0)
                {
                    return BEHAVIOUR_STATUS.FAILURE;
                }
            }

            // Set new direction
            Vector3 dir = openSpaces[Random.Range(0, openSpaces.Count)];
            lastDir = dir;
            GetOwner().SetDestination(GetOwner().transform.position + (new Vector3(Mathf.RoundToInt(dir.x), Mathf.RoundToInt(dir.y), Mathf.RoundToInt(dir.z)) * CS_GameManager.fGridCellSize));
        }

        return BEHAVIOUR_STATUS.SUCCESS;
    }
}
