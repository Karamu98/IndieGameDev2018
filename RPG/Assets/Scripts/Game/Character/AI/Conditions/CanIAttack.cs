using UnityEngine;

namespace Game.AI
{
    public class CanIAttack : Condition
    {
        private bool m_isMelee = true;
        private float m_attackDistance = 1; // Cell
        RaycastHit m_outHit; // Cache

        public CanIAttack(Enemy ownerBrain) : base(ownerBrain)
        {
            m_outHit = new RaycastHit();
        }

        public void SetMelee(bool a_isMelee)
        {
            m_isMelee = a_isMelee;
        }

        public void SetAttackDistance(int a_newCellDistance)
        {
            m_attackDistance = a_newCellDistance;
        }

        public override BEHAVIOUR_STATUS Update()
        {
            if (m_isMelee)
            {
                if (GetOwner().CanMelee())
                {
                    return BEHAVIOUR_STATUS.SUCCESS;
                }
                else
                {
                    return BEHAVIOUR_STATUS.FAILURE;
                }
            }
            else
            {
                if (GetOwner().CanMagic())
                {
                    return BEHAVIOUR_STATUS.SUCCESS;
                }
                else
                {
                    return BEHAVIOUR_STATUS.FAILURE;
                }
            }
        }
    }
}

