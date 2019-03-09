using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InPursuit : Condition
{

    public InPursuit(CS_Enemy ownerBrain) : base(ownerBrain)
    {

    }

    public override BEHAVIOUR_STATUS Update()
    {
        if (GetOwner().isInPursuit())
        {
            Debug.Log("In pursuit");
            return BEHAVIOUR_STATUS.SUCCESS;
        }
        else
        {
            return BEHAVIOUR_STATUS.FAILURE;
        }
    }
}
