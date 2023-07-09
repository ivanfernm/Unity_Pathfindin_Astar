using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar : MonoBehaviour
{
    Node startingNode, destinationNode;

    public List<Node> ReturnPath(Node start, Node finish)
    {
        startingNode = start;
        destinationNode = finish;

        PriorityQueue frontier = new PriorityQueue();
        frontier.Put(startingNode, 0);

        Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();
        cameFrom.Add(startingNode, null);

        Dictionary<Node, int> costSoFar = new Dictionary<Node, int>();
        costSoFar.Add(startingNode, 0);

        while (frontier.Count() != 0)
        {
            var current = frontier.Get();

            //EarlyExit
            if (current == destinationNode)
            {
                List<Node> path = new List<Node>();
                while (current != startingNode)
                {
                    path.Add(current);
                    current = cameFrom[current];
                }
                path.Add(startingNode);
                path.Add(destinationNode);

                return path;
            }

            //Pathfinding itself
            foreach (var next in current.GetNeighboursList())
            {               
               int newCost = costSoFar[current] + next.cost;
               if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
               {
                  costSoFar[next] = newCost;
                  float priority = newCost + DistanceCalculation(destinationNode.transform.position, next.transform.position);
                  frontier.Put(next, priority);
                  cameFrom[next] = current;
               }
                
            }
        }
        Debug.Log("Retornamos null");
        return null;
    }

    float DistanceCalculation(Vector3 a, Vector3 b)
    {
        return Mathf.Abs((a - b).magnitude);
    }
}
