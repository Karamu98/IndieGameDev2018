using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.AI
{
    public class PlayerDetected : Condition
    {
        public PlayerDetected(Enemy ownerBrain) : base(ownerBrain)
        {

        }

        public override BEHAVIOUR_STATUS Update()
        {
            if (GetOwner().CanSeePlayer())
            {
                if (!GetOwner().isInPursuit())
                {
                    GetOwner().SetPursue(true);
                }
                return BEHAVIOUR_STATUS.SUCCESS;
            }
            else
            {
                return BEHAVIOUR_STATUS.FAILURE;
            }
        }
    }

}
