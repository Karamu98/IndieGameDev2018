using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decorator : Node
{
    private Node child;
    private CS_Enemy nodeOwner;

    public Decorator(CS_Enemy ownerBrain)
    {
        nodeOwner = ownerBrain;
    }

    protected Node GetChildNode()
    {
        return child;
    }

    public void SetChild(Node a_newChild)
    {
        child = a_newChild;
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
