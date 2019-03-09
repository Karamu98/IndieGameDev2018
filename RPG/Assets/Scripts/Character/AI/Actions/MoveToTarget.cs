using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToTarget : BTAction
{
    public MoveToTarget(CS_Enemy owner) : base(owner)
    {

    }

    public override BEHAVIOUR_STATUS Update()
    {
        
        if (GetOwner().IsMoving())
        {
            GetOwner().UpdateRotation();
            GetOwner().UpdatePosition();
            return BEHAVIOUR_STATUS.RUNNING;
        }

        if(GetOwner().transform.position == GetOwner().GetDestination())
        {
            return BEHAVIOUR_STATUS.SUCCESS;
        }

        return BEHAVIOUR_STATUS.FAILURE;
    }
}
