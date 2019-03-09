using UnityEngine;

public class BehaviourTree
{
    private CS_Enemy nodeOwnerAgent;

    public BehaviourTree(CS_Enemy ownerBrain)
    {
        nodeOwnerAgent = ownerBrain;
    }

    public virtual void Update()
    {

    }
    public virtual void Interrupt()
    {

    }

    protected CS_Enemy GetOwner()
    {
        return nodeOwnerAgent;
    }
}
