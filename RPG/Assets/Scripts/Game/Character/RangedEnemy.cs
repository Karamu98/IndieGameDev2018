using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.AI
{
    public class RangedEnemy : Enemy
    {
        BTRangedEnemy m_btRanged;
        // Use this for initialization
        protected override void Start()
        {
            base.Start();
            m_btRanged = new BTRangedEnemy(this);
        }

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();
            if (m_isGameActive)
            {
                m_btRanged.Update();
            }
        }

        // Update is called once per frame
        protected override void Awake()
        {
            base.Awake();
        }
    }
}

