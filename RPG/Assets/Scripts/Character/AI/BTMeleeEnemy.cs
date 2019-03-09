using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTMeleeEnemy : BTBasicEnemy
{
    // Compounds

    // Conditions
    private CanIAttack canIAttack;


    // Actions
    private Attack attack;
    

    public BTMeleeEnemy(CS_Enemy ownerBrain) : base(ownerBrain)
    {
        // Initialise Nodes
        ///////////////////
        // Compounds

        // Conditions
        canIAttack = new CanIAttack(GetOwner());
        canIAttack.SetMelee(true);

        // Actions
        attack = new Attack(GetOwner());
        attack.SetMelee(true);


        // Link Nodes
        /////////////
        attackSequence.AddChild(canIAttack);
        attackSequence.AddChild(attack);



    }

    public override void Update()
    {
        base.Update();
    }

    public override void Interrupt()
    {

    }
}