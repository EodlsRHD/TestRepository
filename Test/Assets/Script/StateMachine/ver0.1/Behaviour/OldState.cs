using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class OldStateTransition
{
    private OldState _destinationState = null;

    public OldStateTransition(OldState destinationState)
    {
        _destinationState = destinationState;
    }

    public bool StateTranstionConditionAndOnEnterState(List<Npc> targetPos, List<Npc> evasionPos, bool isActive, Action onCallbackOnExitState)
    {
        if(_destinationState.Condition(targetPos, evasionPos, isActive) == false)
        {
            return false;
        }

        onCallbackOnExitState();
        _destinationState.OnEnterState(targetPos, evasionPos);
        return true;
    }
}

public class OldState : MonoBehaviour
{
    protected bool _isActive = false;

    protected Npc _npc = null;

    protected List<OldStateTransition> _connectedStatesTransitions = new List<OldStateTransition>();

    protected Action<Action<List<Npc>, List<Npc>>, Action> _onCallbackCorditionComparisonAndStateStay = null;

    public void Initialize(Npc npc, Action<Action<List<Npc>, List<Npc>>, Action> onCallbackCorditionComparisonAndStateStay, List<OldStateTransition> connectedStatesTransitions)
    {
        _npc = npc;
        _onCallbackCorditionComparisonAndStateStay = onCallbackCorditionComparisonAndStateStay;
        _connectedStatesTransitions = connectedStatesTransitions;
    }

    public void Initialize(Npc npc, Action<Action<List<Npc>, List<Npc>>, Action> onCallbackCorditionComparisonAndStateStay)
    {
        _npc = npc;
        _onCallbackCorditionComparisonAndStateStay = onCallbackCorditionComparisonAndStateStay;
    }

    public virtual bool Condition(List<Npc> targetPos, List<Npc> evasionPos, bool isActive)
    {
        return false;
    }

    public virtual void OnEnterState(List<Npc> targetPos, List<Npc> evasionPos)
    {

    }

    protected virtual void OnStayState()
    {

    }

    protected virtual void OnExitState()
    {

    }

    public void ConditionComparison(List<Npc> targetPos, List<Npc> evasionPos)
    {
        foreach (var stateTransition in _connectedStatesTransitions)
        {
            if (stateTransition.StateTranstionConditionAndOnEnterState(targetPos, evasionPos, _isActive, OnExitState) == true)
            {
                break;
            }
        }
    }
}
