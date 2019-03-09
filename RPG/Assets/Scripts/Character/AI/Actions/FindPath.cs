using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class FindPath : BTAction
{
    public FindPath(CS_Enemy owner) : base(owner)
    {
    }

    public override BEHAVIOUR_STATUS Update()
    {
        Vector3 result = CS_GameManager.FindPath(GetOwner().gameObject.transform.position, GetOwner().playerLastPos);

        if(result == new Vector3(-1, -1, -1) || result == GetOwner().transform.position)
        {
            return BEHAVIOUR_STATUS.FAILURE;
        }
        else
        {
            GetOwner().SetDestination(result);
            return BEHAVIOUR_STATUS.SUCCESS;
        }
    }
}
