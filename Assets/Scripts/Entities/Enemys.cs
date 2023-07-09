using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemys : MonoBehaviour
{
    //Node --> Waypoint

    [Header("Base")]
    private FSM _fsm;
    public bool isAlerted, isDead;
    public float speed = 12f, maxForce = 1.2f;
    
    [Header("FOV")]
    public Player target;
    public LayerMask playerMask, ObstaclelMask, nodeMask;
    public bool isInRange;

    [Header("Waypoints")]
    public GameObject myNodeContainer, sentinelContainer;
    
    public List<Node> currentNodeList;
    public List<Enemys> currentSentinelList;

    public Node nextNode; 
    public Node playerNode; 
    public Node ogPos;

    public GridInfo grid; 

    public bool isClose; 

    private void Awake()
    {
        #region Getters
        SentinelGetter(sentinelContainer);
        if(currentSentinelList == null || currentSentinelList.Count <= 0)
        {
            Debug.Log("ErrorIn" + this.name + ": SentinelsNotFound");
        }

        NodeGetter(myNodeContainer);
        if (currentNodeList == null || currentNodeList.Count <= 0)
        {
            Debug.Log("ErrorIn" + this.name + ":  WaypointsNotFound");
        }
        #endregion

        target = FindObjectOfType<Player>();
        EventManager.SubscribeToEvent(EventManager.EventsType.Event_Sentinel_Alerted, SetAlerted);
        EventManager.SubscribeToEvent(EventManager.EventsType.Event_Game_Won, Dead);

        #region FSM SetUp
        _fsm = GetComponent<FSM>();

        _fsm.AddState("Patrol", new StatePatrol(_fsm, this, playerMask));
        _fsm.AddState("Chase", new StateChase(_fsm, this));
        _fsm.AddState("Alert", new StateAlert(_fsm, this));

        _fsm.ChangeState("Patrol");
        #endregion
    }

    #region Starting
    public void NodeGetter(GameObject container)
    {
        List<Node> temp = new List<Node>();

        Node[] allChildren = container.GetComponentsInChildren<Node>();
        foreach (Node child in allChildren)
        {            
            temp.Add(child);
        }

        currentNodeList = temp;
    }

    public void SentinelGetter(GameObject container)
    {
        List<Enemys> temp = new List<Enemys>();

        Enemys[] allChildren = container.GetComponentsInChildren<Enemys>();
        foreach (Enemys child in allChildren)
        {
            if(!(child.gameObject == this.gameObject))
            temp.Add(child);
        }

        currentSentinelList = temp;
    }

    #endregion

    private void Update()
    {       
        if(isDead)
        {
            Destroy();
        }
        else
        {
            TargetVision();          
        }
    }

    #region Movement
    public void Move(Vector3 direction)
    {
        direction.y = 0;

        transform.position += Time.deltaTime * direction * speed;
        transform.forward = Vector3.Lerp(transform.forward, direction, 9 * Time.deltaTime);
    }

    public int currentNode = 0;
    public void MoveInPath()
    {

        Vector3 nodePos = currentNodeList[currentNode].transform.position;

        nodePos.y = transform.position.y; 
        
        Vector3 dis = nodePos - transform.position; 

        if (dis.magnitude < 0.15f) 
        {
            isClose = true;
            
            if (currentNode + 1 < currentNodeList.Count) 
            {               
                currentNode++; 

            }
            else 
            {
               currentNode = 0; 
            }
            #region LookAt
            //Ver porque es el error, mientras tener esto
            if (name == "SentinelYellow")
            {
                Vector3 temp = currentNodeList[currentNode].transform.position;
                temp.y = 0;
                transform.LookAt(temp);
            }
            #endregion
        }
        else if (dis.magnitude > 0.16f)
            isClose = false;

        nextNode = currentNodeList[currentNode];
        Move(dis.normalized);
    }
    #endregion

    #region FoV
    //Por ahora se deja acá, se verá si se mueve
    float viewRadius = 8.2f, viewAngle = 75;
    List<GameObject> visibleTargets = new List<GameObject>();

    void TargetVision()
    {
        //Es mas barato limpiar una lista para un solo target que chequear si ya esta
        //Ya que esta acción se hace en cada frame
        visibleTargets.Clear();

        //Esta es la esfera que va a buscar al player
        Collider[] objectsInVision = Physics.OverlapSphere(transform.position, viewRadius, playerMask);
        foreach (var item in objectsInVision)
        {
            GameObject t = item.gameObject;

            //Una vez que la esfera detecta a un player
            //Se calcula una distancia al player
            Vector3 dirToTarget = t.transform.position - transform.position;
            //y se calcula si el player esta dentro de nuestro angulo
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                if (!Physics.Raycast(transform.position, dirToTarget, dirToTarget.magnitude, ObstaclelMask))
                {
                    TargetInRange(objectsInVision[0].GetComponent<Player>());
                }
                else
                {
                    isInRange = false;
                }
            }
            else
            {
                isInRange = false;
            }
        }       
    }

    void TargetInRange(Player p)
    {
        if(!isAlerted) EventManager.TriggerEvent(EventManager.EventsType.Event_Sentinel_Alerted);
        target = p;
        isInRange = true;
    }
    #endregion

    #region Events
    void SetAlerted(params object[] param)
    {
        //isAlerted = true;
    }

    void Dead(params object[] param)
    {
        isDead = true;
    }
    #endregion    


    public void UpdatePlayerPosition()
    {
       grid.playerNode = target.LastPosition(nodeMask, ObstaclelMask);        
    }

    public void AlertAll()
    {
        foreach (var sentinel in currentSentinelList)
        {
            sentinel.target = target;
            sentinel.isAlerted = true;
        }
    }    

    private void Destroy()
    {
        Destroy(this.gameObject);
    }


    public void Rotation(float val)
    {
        Vector3 Rotation = transform.rotation.eulerAngles;
        Rotation += new Vector3(0, val * speed, 0) * Time.deltaTime;
        transform.rotation = Quaternion.Euler(Rotation);
    }
}
