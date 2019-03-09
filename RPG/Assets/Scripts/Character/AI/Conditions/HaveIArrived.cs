using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HaveIArrived : Condition
{
    private const float arrivalRange = 1.1f;

    public HaveIArrived(CS_Enemy ownerBrain) : base(ownerBrain)
    {

    }

    public override BEHAVIOUR_STATUS Update()
    {
        if(GetOwner().GetDestination() == GetOwner().gameObject.transform.position)
        {
            return BEHAVIOUR_STATUS.SUCCESS;
        }

        if(GetOwner().IsMoving())
        {
            return BEHAVIOUR_STATUS.FAILURE;
        }
        else
        {
            return BEHAVIOUR_STATUS.SUCCESS;
        }
    }
}
