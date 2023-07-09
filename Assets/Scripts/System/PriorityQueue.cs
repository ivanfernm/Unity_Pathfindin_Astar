using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriorityQueue : MonoBehaviour
{
    public Dictionary<Node, float> allNodes = new Dictionary<Node, float>();

    public void Put(Node n, float cost)
    {
        if (!allNodes.ContainsKey(n)) allNodes.Add(n, cost);
        else allNodes[n] = cost;
    }

    public Node Get()
    {
        Node n = null;
        if (allNodes.Count == 0) return n;

        foreach (var item in allNodes)
        {
            if (n == null) n = item.Key;

            if (item.Value < allNodes[n]) n = item.Key;
        }

        allNodes.Remove(n);

        return n;
    }

    public float Count()
    {
        return allNodes.Count;
    }
}
