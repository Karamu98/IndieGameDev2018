using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.AI
{
    public class Decorator : Node
    {
        Node m_child;
        Enemy m_nodeOwner;

        public Decorator(Enemy ownerBrain)
        {
            m_nodeOwner = ownerBrain;
        }

        protected Node GetChildNode()
        {
            return m_child;
        }

        public void SetChild(Node a_newChild)
        {
            m_child = a_newChild;
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

