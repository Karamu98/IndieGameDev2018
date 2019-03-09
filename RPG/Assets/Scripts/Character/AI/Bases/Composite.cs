using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Composite : Node
{
    private List<Node> children;
    protected int currentIndex = 0;

    public List<Node> GetChildBehaviours()
    {
        return children;
    }

    public void AddChild(Node newChild)
    {
        children.Add(newChild);
    }

    protected void Reset()
    {
        currentIndex = 0;
    }

    protected void Init()
    {
        currentIndex = 0;
        children = new List<Node>();
    }
}
