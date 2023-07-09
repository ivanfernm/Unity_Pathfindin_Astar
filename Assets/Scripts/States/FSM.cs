using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FSM : MonoBehaviour
{
    Dictionary<string, IState> _stateDictionary = new Dictionary<string, IState>();

    IState _currentState = new EmptyState();

    public void Update()
    {
        _currentState.OnUpdate();
    }

    public void ChangeState(string id)
    {
        _currentState.OnExit();

        _currentState = _stateDictionary[id];

        _currentState.OnEnter();

        //Debug.Log("Cambie de estado a " + id);
    }

    public void AddState(string id, IState state)
    {
        _stateDictionary.Add(id, state);
    }

    public void RemoveState(string id)
    {
        _stateDictionary.Remove(id);
    }

    public bool CheckStateID(string id)
    {
        return _currentState == _stateDictionary[id];
    }
}

public class EmptyState : IState
{
    public void OnEnter() { }
    public void OnUpdate() { }
    public void OnExit() { }
}
