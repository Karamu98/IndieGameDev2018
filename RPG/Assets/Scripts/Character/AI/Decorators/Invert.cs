using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Invert : Decorator
{
    public Invert(CS_Enemy ownerBrain) : base(ownerBrain)
    {

    }

    public override BEHAVIOUR_STATUS Update()
    {
        BEHAVIOUR_STATUS returnStatus = BEHAVIOUR_STATUS.NONE;
        BEHAVIOUR_STATUS previous = GetChildNode().Update();

        if (previous == BEHAVIOUR_STATUS.FAILURE)
        {
            returnStatus = BEHAVIOUR_STATUS.SUCCESS;
        }
        else if(previous == BEHAVIOUR_STATUS.SUCCESS)
        {
            returnStatus = BEHAVIOUR_STATUS.FAILURE;
        }

        return returnStatus;
    }
}
