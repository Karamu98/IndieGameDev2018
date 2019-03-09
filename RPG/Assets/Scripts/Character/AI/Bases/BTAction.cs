using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTAction : Node
{
    private CS_Enemy nodeOwnerAgent;

    public BTAction(CS_Enemy ownerBrain)
    {
        nodeOwnerAgent = ownerBrain;
    }

    public override BEHAVIOUR_STATUS Update()
    {
        return BEHAVIOUR_STATUS.NONE;
    }

    public CS_Enemy GetOwner()
    {
        return nodeOwnerAgent;
    }
}
