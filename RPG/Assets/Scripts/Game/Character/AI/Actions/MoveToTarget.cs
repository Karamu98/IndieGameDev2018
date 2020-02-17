using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.AI
{
    public class MoveToTarget : BTAction
    {
        public MoveToTarget(Enemy owner) : base(owner)
        {

        }

        public override BEHAVIOUR_STATUS Update()
        {
            if (GetOwner().CharacterLocomotion.IsMoving)
            {
                GetOwner().CharacterLocomotion.Process();
                return BEHAVIOUR_STATUS.RUNNING;
            }
            else
            {
                return BEHAVIOUR_STATUS.SUCCESS;
            }
        }
    }
}