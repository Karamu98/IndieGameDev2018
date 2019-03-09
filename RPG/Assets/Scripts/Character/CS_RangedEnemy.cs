using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_RangedEnemy : CS_Enemy
{
    BTRangedEnemy btRanged;
    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        btRanged = new BTRangedEnemy(this);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if(bGameActive)
        {
            btRanged.Update();
        }
    }

    // Update is called once per frame
    protected override void Awake()
    {
        base.Awake();
    }
}
