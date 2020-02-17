using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI.Screens
{
    public class ScreenPlayerGameUI : Core.UI.Screen
    {
        public override string ScreenName { get { return "PlayerGameUI"; } }

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