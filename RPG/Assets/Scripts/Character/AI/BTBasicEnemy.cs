using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTBasicEnemy : BehaviourTree
{
    // Compounds
    protected Selector rootSelector;
    protected Selector combatSelector;
    protected Sequence chaseSequence;
    protected Sequence attackSequence;
    protected Sequence moveToAttackSequence;
    protected Sequence patrolSequence;

    // Conditions
    protected PlayerDetected canSeePlayer;
    protected InPursuit isPursuingPlayer;
    protected HaveIArrived haveIArrived;

    // Actions
    protected FindRandomPosition findRandomPosition;
    protected MoveToTarget moveToTarget;
    protected Wait patrolWait;
    protected Wait chargeWait;
    protected Wait pursueWait;
    protected FindAttackPosition findAttackPosition;
    protected FindPath findPath;



    public BTBasicEnemy(CS_Enemy ownerBrain) : base(ownerBrain)
    {
        // Initialise Nodes
        ///////////////////
        // Compounds
        rootSelector = new Selector();
        combatSelector = new Selector();
        chaseSequence = new Sequence();
        patrolSequence = new Sequence();
        attackSequence = new Sequence();
        moveToAttackSequence = new Sequence();
        


        // Conditions
        canSeePlayer = new PlayerDetected(GetOwner());
        isPursuingPlayer = new InPursuit(GetOwner());
        haveIArrived = new HaveIArrived(GetOwner());

        // Actions
        findRandomPosition = new FindRandomPosition(GetOwner());
        moveToTarget = new MoveToTarget(GetOwner());
        patrolWait = new Wait(GetOwner());
        patrolWait.SetWaitTime(1);
        chargeWait = new Wait(GetOwner());
        chargeWait.SetWaitTime(0.2f);
        pursueWait = new Wait(GetOwner());
        pursueWait.SetWaitTime(0.5f);
        findAttackPosition = new FindAttackPosition(GetOwner());
        findPath = new FindPath(GetOwner());


        // Link Nodes
        /////////////
        // Top level root selector
        rootSelector.AddChild(combatSelector);
        rootSelector.AddChild(chaseSequence);
        rootSelector.AddChild(patrolWait);

        // Sequence to attack
        combatSelector.AddChild(attackSequence);
        combatSelector.AddChild(chargeWait);

        // Sequence to attack
        attackSequence.AddChild(canSeePlayer);

        // Sequence to move to attack
        chargeWait.SetChild(moveToAttackSequence);
        moveToAttackSequence.AddChild(haveIArrived);
        moveToAttackSequence.AddChild(canSeePlayer);
        moveToAttackSequence.AddChild(findAttackPosition);
        moveToAttackSequence.AddChild(moveToTarget);

        pursueWait.SetChild(chaseSequence);
        chaseSequence.AddChild(haveIArrived);
        chaseSequence.AddChild(isPursuingPlayer);
        chaseSequence.AddChild(findPath);
        chaseSequence.AddChild(moveToTarget);

        // Sequence to patrol
        patrolWait.SetChild(patrolSequence);
        patrolSequence.AddChild(haveIArrived);
        patrolSequence.AddChild(findRandomPosition);
        patrolSequence.AddChild(moveToTarget);
    }

    public override void Update()
    {
        rootSelector.Update();
    }

    public override void Interrupt()
    {

    }
}