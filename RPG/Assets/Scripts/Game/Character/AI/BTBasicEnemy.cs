using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.AI
{
    public class BTBasicEnemy : BehaviourTree
    {
        // Compounds
        protected Selector m_rootSelector;
        protected Selector m_combatSelector;
        protected Sequence m_chaseSequence;
        protected Sequence m_attackSequence;
        protected Sequence m_moveToAttackSequence;
        protected Sequence m_patrolSequence;

        // Conditions
        protected PlayerDetected m_canSeePlayer;
        protected InPursuit m_isPursuingPlayer;
        protected HaveIArrived m_haveIArrived;

        // Actions
        protected FindRandomPosition m_findRandomPosition;
        protected MoveToTarget m_moveToTarget;
        protected Wait m_patrolWait;
        protected Wait m_chargeWait;
        protected Wait m_pursueWait;
        protected FindAttackPosition m_findAttackPosition;
        protected FindPath m_findPath;



        public BTBasicEnemy(Enemy ownerBrain) : base(ownerBrain)
        {
            // Initialise Nodes
            ///////////////////
            // Compounds
            m_rootSelector = new Selector();
            m_combatSelector = new Selector();
            m_chaseSequence = new Sequence();
            m_patrolSequence = new Sequence();
            m_attackSequence = new Sequence();
            m_moveToAttackSequence = new Sequence();



            // Conditions
            m_canSeePlayer = new PlayerDetected(GetOwner());
            m_isPursuingPlayer = new InPursuit(GetOwner());
            m_haveIArrived = new HaveIArrived(GetOwner());

            // Actions
            m_findRandomPosition = new FindRandomPosition(GetOwner());
            m_moveToTarget = new MoveToTarget(GetOwner());
            m_patrolWait = new Wait(GetOwner());
            m_patrolWait.SetWaitTime(1);
            m_chargeWait = new Wait(GetOwner());
            m_chargeWait.SetWaitTime(0.2f);
            m_pursueWait = new Wait(GetOwner());
            m_pursueWait.SetWaitTime(0.5f);
            m_findAttackPosition = new FindAttackPosition(GetOwner());
            m_findPath = new FindPath(GetOwner());


            // Link Nodes
            /////////////
            // Top level root selector
            m_rootSelector.AddChild(m_combatSelector);
            m_rootSelector.AddChild(m_chaseSequence);
            m_rootSelector.AddChild(m_patrolWait);

            // Sequence to attack
            m_combatSelector.AddChild(m_attackSequence);
            m_combatSelector.AddChild(m_chargeWait);

            // Sequence to attack
            m_attackSequence.AddChild(m_canSeePlayer);

            // Sequence to move to attack
            m_chargeWait.SetChild(m_moveToAttackSequence);
            m_moveToAttackSequence.AddChild(m_haveIArrived);
            m_moveToAttackSequence.AddChild(m_canSeePlayer);
            m_moveToAttackSequence.AddChild(m_findAttackPosition);
            m_moveToAttackSequence.AddChild(m_moveToTarget);

            m_pursueWait.SetChild(m_chaseSequence);
            m_chaseSequence.AddChild(m_haveIArrived);
            m_chaseSequence.AddChild(m_isPursuingPlayer);
            m_chaseSequence.AddChild(m_findPath);
            m_chaseSequence.AddChild(m_moveToTarget);

            // Sequence to patrol
            m_patrolWait.SetChild(m_patrolSequence);
            m_patrolSequence.AddChild(m_haveIArrived);
            m_patrolSequence.AddChild(m_findRandomPosition);
            m_patrolSequence.AddChild(m_moveToTarget);
        }

        public override void Update()
        {
            m_rootSelector.Update();
        }

        public override void Interrupt()
        {

        }
    }
}