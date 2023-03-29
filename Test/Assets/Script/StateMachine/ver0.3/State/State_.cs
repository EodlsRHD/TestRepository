using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class State_ : ScriptableObject
{
    public virtual bool Condition(ref Character character, ref List<Character> objectDetected)
    {
        return false;
    }

    public virtual void OnEnterState(ref Character character, ref List<Character> objectDetected, Action<Action<Character, Action>> onCallbackSetUpdate)
    {

    }

    public virtual void OnStayState(Character character, Action onCallbackConditionComparison)
    {

    }

    public virtual void OnExitState(Character character)
    {

    }
}
