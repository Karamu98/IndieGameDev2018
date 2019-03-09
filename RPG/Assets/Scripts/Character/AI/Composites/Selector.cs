using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selector : Composite
{
    public Selector()
    {
        Init();
    }

    public override BEHAVIOUR_STATUS Update()
    {
        BEHAVIOUR_STATUS returnStatus = BEHAVIOUR_STATUS.FAILURE;
        Node currentNode = GetChildBehaviours()[currentIndex];

        if(currentNode != null)
        {
            BEHAVIOUR_STATUS behaviourStatus = currentNode.Update();

            if (behaviourStatus == BEHAVIOUR_STATUS.FAILURE)
            {
                if (currentIndex == GetChildBehaviours().Count - 1)
                {
                    returnStatus = BEHAVIOUR_STATUS.FAILURE;
                }
                else
                {
                    currentIndex += 1;
                    returnStatus = BEHAVIOUR_STATUS.RUNNING;
                }
            }
            else
            {
                returnStatus = behaviourStatus;
            }
        }


        if(returnStatus == BEHAVIOUR_STATUS.SUCCESS || returnStatus == BEHAVIOUR_STATUS.FAILURE)
        {
            Reset();
        }

        return returnStatus;
    }
}
