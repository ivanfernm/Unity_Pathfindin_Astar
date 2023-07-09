using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Base")]
    [SerializeField] int life = 1;
    [SerializeField] float speed = 0.2f;

    private void Update()
    {
        if (life > 0)
        {
            Controller();
        }
        else 
        {
            Destroy();
        }
    }

    void Destroy()
    {
        EventManager.TriggerEvent(EventManager.EventsType.Event_Game_Loss);
        Destroy(this.gameObject);
    }

    private void Controller()
    {
        if (Input.GetKey(KeyCode.A)) {transform.position += new Vector3(speed, 0, 0);}
        if (Input.GetKey(KeyCode.D)) {transform.position += new Vector3(-speed, 0, 0);}

        if (Input.GetKey(KeyCode.S)) {transform.position += new Vector3(0, 0, speed);}
        if (Input.GetKey(KeyCode.W)) {transform.position += new Vector3(0, 0, -speed);}
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Enemys>())
        {
            life -= 1;
        }

        if (collision.gameObject.GetComponent<Exit>())
            EventManager.TriggerEvent(EventManager.EventsType.Event_Game_Won);
    }

    public Node LastPosition(LayerMask nodeMask, LayerMask wallMask)
    {       
        Collider[] sphereNode = Physics.OverlapSphere(transform.position, 15, nodeMask);
        foreach (var item in sphereNode)
        {
            Vector3 dirToTarget = item.transform.position - transform.position;

            if (!Physics.Raycast(transform.position, dirToTarget, dirToTarget.magnitude, wallMask))
            {            
              Node temp = item.gameObject.GetComponent<Node>();
              if (!(temp == null))
              {
                 return temp;
              }
            }
        }

        return null;
    }
}
