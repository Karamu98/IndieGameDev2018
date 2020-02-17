using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.AI
{
    public class Condition : Node
    {
        Enemy m_nodeOwner;

        public Condition(Enemy ownerBrain)
        {
            m_nodeOwner = ownerBrain;
        }

        public override BEHAVIOUR_STATUS Update()
        {
            return BEHAVIOUR_STATUS.NONE;
        }

        public Enemy GetOwner()
        {
            return m_nodeOwner;
        }

    }
}