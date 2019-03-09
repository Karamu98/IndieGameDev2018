using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wait : Decorator
{
    private float WaitTime = 5.0f;
    private float waitTimer = 5.0f;
    BEHAVIOUR_STATUS previous = BEHAVIOUR_STATUS.NONE;

    public Wait(CS_Enemy ownerBrain) : base(ownerBrain)
    {

    }

    public void SetWaitTime(float a_newWaitTime)
    {
        WaitTime = a_newWaitTime;
    }

    public override BEHAVIOUR_STATUS Update()
    {
        if(previous == BEHAVIOUR_STATUS.RUNNING)
        {
            previous = GetChildNode().Update();
            return previous;
        }
        waitTimer = waitTimer - Time.deltaTime;        

        if (waitTimer < 0.0f)
        {
            waitTimer = WaitTime;
            previous = GetChildNode().Update();
            return previous;
        }
        else
        {
            return BEHAVIOUR_STATUS.RUNNING;
        }
    }
}
