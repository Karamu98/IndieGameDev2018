using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.AI
{
    public class Selector : Composite
    {
        public Selector()
        {
            Init();
        }

        public override BEHAVIOUR_STATUS Update()
        {
            BEHAVIOUR_STATUS returnStatus = BEHAVIOUR_STATUS.FAILURE;
            Node currentNode = GetChildBehaviours()[m_currentIndex];

            if (currentNode != null)
            {
                BEHAVIOUR_STATUS behaviourStatus = currentNode.Update();

                if (behaviourStatus == BEHAVIOUR_STATUS.FAILURE)
                {
                    if (m_currentIndex == GetChildBehaviours().Count - 1)
                    {
                        returnStatus = BEHAVIOUR_STATUS.FAILURE;
                    }
                    else
                    {
                        m_currentIndex += 1;
                        returnStatus = BEHAVIOUR_STATUS.RUNNING;
                    }
                }
                else
                {
                    returnStatus = behaviourStatus;
                }
            }


            if (returnStatus == BEHAVIOUR_STATUS.SUCCESS || returnStatus == BEHAVIOUR_STATUS.FAILURE)
            {
                Reset();
            }

            return returnStatus;
        }
    }
}