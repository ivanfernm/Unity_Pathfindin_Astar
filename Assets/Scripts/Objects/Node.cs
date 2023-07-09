using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    [Header("Base")]
    public int cost = 1;
    [SerializeField] private float radius = 10;
    [SerializeField] private LayerMask nodeMask, wallMask;

    public List<Node> neighbours = new List<Node>();

    private void Awake()
    {
        GetNeighboursList();
    }

    public List<Node> GetNeighboursList()
    {
        SeeNeighbours();    

        return neighbours;
    }

    void SeeNeighbours()
    {       
        Collider[] objectsInVision = Physics.OverlapSphere(transform.position, radius, nodeMask);
        foreach (var item in objectsInVision)
        {
            Vector3 dirToTarget = item.gameObject.transform.position - transform.position;
            
            //Tiramos un rayo para ver si no hay un pared por medio, asi evitamos "vecinos" inaccesibles
            if (!Physics.Raycast(transform.position, dirToTarget, dirToTarget.magnitude, wallMask))
            {
                CheckNode(item.gameObject.GetComponent<Node>());            
            }         
        }

    }
    void CheckNode(Node n)
    {
        Node temp = n;
        if (!(n == null) && !(n == this))
        {
            neighbours.Add(temp);
        }
    }
}
