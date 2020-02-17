using UnityEngine;

namespace Game.AI
{
    public class Wait : Decorator
    {
        float m_waitTime = 5.0f;
        float m_waitTimer = 5.0f;
        BEHAVIOUR_STATUS m_behaviourPrevious = BEHAVIOUR_STATUS.NONE;

        public Wait(Enemy ownerBrain) : base(ownerBrain)
        {

        }

        public void SetWaitTime(float a_newWaitTime)
        {
            m_waitTime = a_newWaitTime;
        }

        public override BEHAVIOUR_STATUS Update()
        {
            if (m_behaviourPrevious == BEHAVIOUR_STATUS.RUNNING)
            {
                m_behaviourPrevious = GetChildNode().Update();
                return m_behaviourPrevious;
            }
            m_waitTimer = m_waitTimer - Time.deltaTime;

            if (m_waitTimer < 0.0f)
            {
                m_waitTimer = m_waitTime;
                m_behaviourPrevious = GetChildNode().Update();
                return m_behaviourPrevious;
            }
            else
            {
                return BEHAVIOUR_STATUS.RUNNING;
            }
        }
    }
}