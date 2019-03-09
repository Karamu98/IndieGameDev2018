using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sequence : Composite
{
    public Sequence()
    {
        Init();
    }

    public override BEHAVIOUR_STATUS Update()
    {
        BEHAVIOUR_STATUS returnStatus = BEHAVIOUR_STATUS.FAILURE;
        Node currentBehaviour = GetChildBehaviours()[currentIndex];

        if(currentBehaviour != null)
        {
            BEHAVIOUR_STATUS behaviourStatus = currentBehaviour.Update();

            if(behaviourStatus == BEHAVIOUR_STATUS.SUCCESS)
            {
                if (currentIndex == GetChildBehaviours().Count - 1)
                {
                    returnStatus = BEHAVIOUR_STATUS.SUCCESS;
                }
                else
                {
                    currentIndex++;
                    returnStatus = BEHAVIOUR_STATUS.RUNNING;
                }
            }
            else
            {
                returnStatus = behaviourStatus;
            }

            if(returnStatus == BEHAVIOUR_STATUS.SUCCESS || returnStatus == BEHAVIOUR_STATUS.FAILURE)
            {
                Reset();
            }
        }

        return returnStatus;
    }
}
