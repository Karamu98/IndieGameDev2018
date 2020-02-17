using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

namespace Game.AI
{
    public class Attack : BTAction
    {
        bool m_isMelee;
        public Attack(Enemy owner) : base(owner)
        {
            m_isMelee = true;
        }

        public void SetMelee(bool a_IsMelee)
        {
            m_isMelee = a_IsMelee;
        }

        public override BEHAVIOUR_STATUS Update()
        {
            if (m_isMelee)
            {
                if (GetOwner().CanMelee())
                {
                    GetOwner().MeleeAttack();
                    Debug.Log("Attacking!");
                    return BEHAVIOUR_STATUS.SUCCESS;
                }
            }
            else
            {
                if (GetOwner().MagicAttack())
                {
                    return BEHAVIOUR_STATUS.SUCCESS;
                }
            }

            Debug.Log("Attack Failed!");
            return BEHAVIOUR_STATUS.FAILURE;
        }
    }
}

