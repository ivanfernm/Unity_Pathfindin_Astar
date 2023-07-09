using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateChase : IState
{

    FSM _fsm;
    Enemys _sentinel;
    Vector3 _velocity;

    float counter; 
    float counterLimit = 0.55f;

    bool positionGiven, onPursuit;
    Vector3 tempPos;

    float counterPos, posLimit; 

    public StateChase(FSM fsm, Enemys s)
    {
        _fsm = fsm;
        _sentinel = s;
        positionGiven = false;
        onPursuit = false;
        posLimit = 0.5f;
    }

    public void OnEnter()
    {
        CheckTarget();
    }

    public void OnUpdate()
    {
        if (_sentinel.target == null)
            PatrolAgain();     

        if (_sentinel.isInRange && !(_sentinel.target == null))
            Found();

        if (onPursuit && !(_sentinel.target == null))
            Chase();
        
        if(!_sentinel.isInRange && !(_sentinel.target == null))
            ObjectiveLost();
    }

    public void OnExit() { }   

    #region OnEnter
    void CheckTarget()
    {       
        if (_sentinel.target == null)
        {
            Debug.Log("ErrorIn" + _sentinel.name + ": TargetDataNotFound");
            PatrolAgain();
        }
    }
    #endregion

    #region OnUpdate
    void Found()
    {
        if (!positionGiven)
        {
            _sentinel.UpdatePlayerPosition();
            _sentinel.AlertAll();
            positionGiven = true;
        }

        onPursuit = true;
    }

    void Chase()
    {
        tempPos = _sentinel.target.transform.position;
        Move(tempPos);
    }

    void Move(Vector3 temp)
    {
        Vector3 distance = temp - _sentinel.transform.position;
        distance.Normalize();             

        Vector3 steering = distance - _velocity;
        steering = Vector3.ClampMagnitude(steering, _sentinel.maxForce);

        _sentinel.Move(steering);
    }   

    void ObjectiveLost()
    {
        if (positionGiven)
            positionGiven = false;

        counterPos += Time.deltaTime;
        if(counterPos >= posLimit)
        {
            onPursuit = false;
            counterPos = 0;
        }

        counter += Time.deltaTime * 1;
        if (counter >= counterLimit)
        {
            _fsm.ChangeState("Alert");
        }
    }
    #endregion

    void PatrolAgain()
    {
        _fsm.ChangeState("Patrol");
    }
}
