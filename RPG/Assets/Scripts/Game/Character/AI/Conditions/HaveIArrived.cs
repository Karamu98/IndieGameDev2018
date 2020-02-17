using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.AI
{
    public class HaveIArrived : Condition
    {
        public HaveIArrived(Enemy ownerBrain) : base(ownerBrain)
        {

        }

        public override BEHAVIOUR_STATUS Update()
        {
            if (GetOwner().CharacterLocomotion.IsMoving)
            {
                return BEHAVIOUR_STATUS.FAILURE;
            }
            else
            {
                return BEHAVIOUR_STATUS.SUCCESS;
            }
        }
    }
}

