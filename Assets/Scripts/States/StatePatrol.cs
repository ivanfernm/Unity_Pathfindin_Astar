using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatePatrol : IState
{

    FSM _fsm;
    Enemys _sentinel;
    List<Node> _nodeList;

    AStar astar;
    List<Node> waypoints;

    LayerMask _mask;

    public StatePatrol(FSM fsm, Enemys s, LayerMask m)
    {
        _fsm = fsm;
        _sentinel = s;
        _mask = m;

        astar = _sentinel.GetComponent<AStar>();
    }

    public void OnEnter()
    {
        _sentinel.SentinelGetter(_sentinel.sentinelContainer);
        _sentinel.currentNode = 1;
    }

    public void OnUpdate()
    {
        CheckRange();
        CheckAlert();
        _sentinel.MoveInPath();        
    }

    public void OnExit() { }     

    #region OnUpdate     
    void CheckAlert()
    {        
        if(_sentinel.isAlerted == true && !_sentinel.isInRange)
        {
            //Debug.Log("He sido alertado");
            //_sentinel.playerNode = _sentinel.target.LastPosition(_sentinel.nodeMask);
            _fsm.ChangeState("Alert");
        }
    }

    void CheckRange()
    {
        if(_sentinel.isInRange && !(_sentinel.target == null))
        {
            //EventManager.TriggerEvent(EventManager.EventsType.Event_Sentinel_Alerted);
            _fsm.ChangeState("Chase");
        }
    }
    #endregion
}
