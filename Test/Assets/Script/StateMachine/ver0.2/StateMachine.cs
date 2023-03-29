using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StateMachine : MonoBehaviour
{
    private static StateMachine _instance;
    public static StateMachine instance
    {
        get 
        { 
            if(_instance == null)
            {
                _instance = new StateMachine();
            }

            return _instance;
        }
    }

    [SerializeField]
    private List<CharacterState> _states = new List<CharacterState>();

    public void Awake()
    {
        _instance = this;

        CharacterState[] states = GetComponentsInChildren<CharacterState>();

        foreach(var state in states)
        {
            _states.Add(state);
        }
    }

    public CharacterState GetState(string type)
    {
        CharacterState newState = new CharacterState();

        foreach (var state in _states)
        {
            if(state.GetType().ToString().Contains(type))
            {
                newState = state;
            }
        }

        return newState;
    }
}