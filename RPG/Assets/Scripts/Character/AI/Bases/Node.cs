using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BEHAVIOUR_STATUS
{
    SUCCESS,
    FAILURE,
    RUNNING,
    NONE
}


public class Node
{
    private BEHAVIOUR_STATUS behaviourStatus;

    public Node()
    {
        behaviourStatus = BEHAVIOUR_STATUS.NONE;
    }

    public virtual BEHAVIOUR_STATUS Update()
    {
        return BEHAVIOUR_STATUS.NONE;
    }
}
