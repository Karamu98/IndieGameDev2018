using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Game.UI.Screens
{
    public class ScreenPlayerGameUI : Core.UI.Screen
    {
#if PC_TEST
        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.A))
            {
                LeftPressed();
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                RightPressed();
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                ForwardPressed();
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                BackwardsPressed();
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                TurnLeftPressed();
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                TurnRightPressed();
            }

            if(Input.GetMouseButtonDown(0))
            {
                RightHandPressed();
            }

            if (Input.GetMouseButtonDown(1))
            {
                LeftHandPressed();
            }
        }
#endif
        public override string ScreenName { get { return "PlayerGameUI"; } }

        public void LeftHandPressed()
        {
            GameManager.s_Player.MeleeAttack();
        }
        public void RightHandPressed()
        {
            GameManager.s_Player.MagicAttack();
        }

        public void ForwardPressed()
        {
            MovePressed(Direction.FORWARD);
        }
        public void BackwardsPressed()
        {
            MovePressed(Direction.BACKWARDS);
        }
        public void LeftPressed()
        {
            MovePressed(Direction.LEFT);
        }
        public void RightPressed()
        {
            MovePressed(Direction.RIGHT);
        }

        public void TurnLeftPressed()
        {
            TurnPressed(Direction.LEFT);
        }
        public void TurnRightPressed()
        {
            TurnPressed(Direction.RIGHT);
        }

        void MovePressed(Direction dirToGo)
        {
            GameManager.s_Player.CharacterLocomotion.Move(dirToGo);
        }
        void TurnPressed(Direction dirToTurn)
        {
            GameManager.s_Player.CharacterLocomotion.Turn(dirToTurn);
        }
    }
}