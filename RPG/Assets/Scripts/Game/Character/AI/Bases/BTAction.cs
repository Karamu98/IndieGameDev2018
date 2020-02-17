using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.AI
{
    public class BTAction : Node
    {
        Enemy m_nodeOwnerAgent;

        public BTAction(Enemy ownerBrain)
        {
            m_nodeOwnerAgent = ownerBrain;
        }

        public override BEHAVIOUR_STATUS Update()
        {
            return BEHAVIOUR_STATUS.NONE;
        }

        public Enemy GetOwner()
        {
            return m_nodeOwnerAgent;
        }
    }
}