using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class ArenaEnd : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == GameManager.s_Player.gameObject)
            {
                GameManager.LevelComplete();
            }
        }
    }
}