using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_MeleeEnemy : CS_Enemy
{
    private BTMeleeEnemy btMeleeEnemy;

	// Use this for initialization
	protected override void Start ()
    {
        base.Start();
        btMeleeEnemy = new BTMeleeEnemy(this);
	}

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (bGameActive)
        {
            btMeleeEnemy.Update();
        }
    }

    // Update is called once per frame
    protected override void Awake()
    {
        base.Awake();
    }
}
