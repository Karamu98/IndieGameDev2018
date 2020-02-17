using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.AI
{
    public class Composite : Node
    {
        List<Node> m_children;
        protected int m_currentIndex = 0;

        public List<Node> GetChildBehaviours()
        {
            return m_children;
        }

        public void AddChild(Node newChild)
        {
            m_children.Add(newChild);
        }

        protected void Reset()
        {
            m_currentIndex = 0;
        }

        protected void Init()
        {
            m_currentIndex = 0;
            m_children = new List<Node>();
        }
    }

}