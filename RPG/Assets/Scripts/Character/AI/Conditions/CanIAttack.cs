using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanIAttack : Condition
{
    private bool bMelee = true;
    private float attackDistance = 1; // Cell
    RaycastHit outHit; // Cache

    public CanIAttack(CS_Enemy ownerBrain) : base(ownerBrain)
    {
        outHit = new RaycastHit();
    }

    public void SetMelee(bool a_isMelee)
    {
        bMelee = a_isMelee;
    }

    public void SetAttackDistance(int a_newCellDistance)
    {
        attackDistance = a_newCellDistance;
    }

    public override BEHAVIOUR_STATUS Update()
    {
        if(bMelee)
        {
            if(GetOwner().CanMelee())
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
