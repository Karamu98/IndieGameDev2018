using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Condition : Node
{
    private CS_Enemy nodeOwner;

    public Condition(CS_Enemy ownerBrain)
    {
        nodeOwner = ownerBrain;
    }

    public override BEHAVIOUR_STATUS Update()
    {
        return BEHAVIOUR_STATUS.NONE;
    }
    
    public CS_Enemy GetOwner()
    {
        return nodeOwner;
    }

}
