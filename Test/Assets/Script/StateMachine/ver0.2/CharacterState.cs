using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CharacterStateSO : ScriptableObject
{
    public string _stateType = string.Empty;
}

public class CharacterState : MonoBehaviour
{
    protected Character _character = null;

    protected bool _isActive = false;

    protected Action<Action> _onCallbackSetUpdate = null;

    public bool isActive
    {
        get { return _isActive; }
    }

    public void Initialize(Action<Action> onCallbackSetUpdate)
    {
        _onCallbackSetUpdate = onCallbackSetUpdate;
    }

    public virtual bool Condition(ref List<Character> objectDetected, ref bool isActive)
    {
        return false;
    }

    public virtual void OnStateEnter(ref Character character, ref List<Character> objectDetected)
    {
        _character = character;
    }

    public virtual void OnStateStay()
    {

    }

    public virtual void OnStateExit(Character character)
    {

    }
}
