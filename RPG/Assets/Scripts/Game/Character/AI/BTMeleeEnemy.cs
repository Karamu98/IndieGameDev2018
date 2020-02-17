using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.AI
{
    public class BTMeleeEnemy : BTBasicEnemy
    {
        // Compounds

        // Conditions
        private CanIAttack m_canIAttack;


        // Actions
        private Attack m_attack;


        public BTMeleeEnemy(Enemy ownerBrain) : base(ownerBrain)
        {
            // Initialise Nodes
            ///////////////////
            // Compounds

            // Conditions
            m_canIAttack = new CanIAttack(GetOwner());
            m_canIAttack.SetMelee(true);

            // Actions
            m_attack = new Attack(GetOwner());
            m_attack.SetMelee(true);


            // Link Nodes
            /////////////
            m_attackSequence.AddChild(m_canIAttack);
            m_attackSequence.AddChild(m_attack);
        }

        public override void Update()
        {
            base.Update();
        }

        public override void Interrupt()
        {

        }
    }
}
