using UnityEngine;

namespace Game.AI
{
    public class BehaviourTree
    {
        Enemy m_nodeOwnerAgent;

        public BehaviourTree(Enemy ownerBrain)
        {
            m_nodeOwnerAgent = ownerBrain;
        }

        public virtual void Update()
        {

        }
        public virtual void Interrupt()
        {

        }

        protected Enemy GetOwner()
        {
            return m_nodeOwnerAgent;
        }
    }
}
