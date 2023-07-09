using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateAlert : IState
{
    FSM _fsm;
    Enemys _sentinel;

    Vector3 lastPosition;

    //PathFinding
    AStar astar;

    bool reversed = false;

    float lookCounter, lookLimit;

    public StateAlert(FSM fsm, Enemys s)
    {
        _fsm = fsm;
        _sentinel = s;
        astar = _sentinel.GetComponent<AStar>();
    }

    public void OnEnter()
    {
        lookLimit = 3f;
        UpdatePath();       
    }

    public void OnUpdate()
    {
     
        if (_sentinel.playerNode != _sentinel.grid.playerNode)
            UpdatePath();

        if(!(_sentinel.isClose && _sentinel.currentNode + 1 >= _sentinel.currentNodeList.Count))
        _sentinel.MoveInPath();

        if (_sentinel.isInRange)
            _fsm.ChangeState("Chase");

        if (_sentinel.target == null)
            ChangeCourse();
        if (_sentinel.isClose && _sentinel.currentNode + 1 >= _sentinel.currentNodeList.Count && !_sentinel.isInRange)       
            LookAround();        
    }

    public void OnExit()
    {
        _sentinel.isAlerted = false;
        _sentinel.NodeGetter(_sentinel.myNodeContainer);
    }


    void UpdatePath()
    {
        reversed = false;
        _sentinel.currentNode = 1;

        _sentinel.playerNode = _sentinel.grid.playerNode;
        var temp = astar.ReturnPath(_sentinel.nextNode, _sentinel.grid.playerNode);
        
        temp.Reverse();
        _sentinel.currentNodeList = temp;

        lookCounter = 0;
    }

    void ChangeCourse()
    {
       if (reversed)
       {
             _fsm.ChangeState("Patrol");
       }
       else
       {
            reversed = true;

            _sentinel.currentNode = 0; 
            var temp = astar.ReturnPath(_sentinel.nextNode, _sentinel.ogPos);
            temp.Reverse();

            _sentinel.currentNodeList = temp;
       }        
    }

    void LookAround()
    {
        if (!reversed)
        {
           lookCounter += 1 * Time.deltaTime;
            if (lookCounter <= lookLimit / 2)
            {
                _sentinel.Rotation(12);
            }
            else if (lookCounter > lookLimit / 2 && lookCounter < lookLimit)
            {
                _sentinel.Rotation(-12);
            }
            else if (lookCounter >= lookLimit)
                ChangeCourse();
        }
        else 
          ChangeCourse();

    }
}
