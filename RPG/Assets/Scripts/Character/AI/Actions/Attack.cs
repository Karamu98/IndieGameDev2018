using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class Attack : BTAction
{
    bool bIsMelee;
    public Attack(CS_Enemy owner) : base(owner)
    {
        bIsMelee = true;
    }

    public void SetMelee(bool a_IsMelee)
    {
        bIsMelee = a_IsMelee;
    }

    public override BEHAVIOUR_STATUS Update()
    {
        if(bIsMelee)
        {
            if(GetOwner().CanMelee())
            {
                GetOwner().MeleeAttack();
                Debug.Log("Attacking!");
                return BEHAVIOUR_STATUS.SUCCESS;
            } 
        }
        else
        {
            if(GetOwner().MagicAttack())
            {
                return BEHAVIOUR_STATUS.SUCCESS;
            }
        }

        Debug.Log("Attack Failed!");
        return BEHAVIOUR_STATUS.FAILURE;
    }
}
