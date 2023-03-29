using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum eCharacterType
{
    Non,
    Npc,
    Player,
    Monster
}

public class NpcStateMachine : MonoBehaviour
{
    [SerializeField]
    private Npc _npc = null;

    [SerializeField]
    private List<OldState> _states = new List<OldState>();

    [SerializeField]
    private eCharacterType _targetType = eCharacterType.Non;

    [SerializeField]
    private eCharacterType _evasionType = eCharacterType.Non;

    private Action<List<Npc>, List<Npc>> _onCallbackCorditionComparison = null;

    private Action _onCallbackOnStateStay = null;

    private void Start()
    {
        Application.targetFrameRate = 60;

        List<OldStateTransition> statesTransitions = new List<OldStateTransition>(_states.Count);
        foreach (var state in _states)
        {
            OldStateTransition transition = new OldStateTransition(state);
            statesTransitions.Add(transition);
        }

        for (int i = 0; i < _states.Count; i++)
        {
            _states[i].Initialize(_npc, SetCordinaionComparisonAndStateStay, statesTransitions);
        }

        _npc.AroundSearch(_targetType.ToString(), _evasionType.ToString(), (targetPos, evasionPos) =>
        {
            _states[0].OnEnterState(targetPos, evasionPos);
        });
    }

    private void SetCordinaionComparisonAndStateStay(Action<List<Npc>, List<Npc>> onCallbackCorditionComparison, Action onCallbackOnStateStay)
    {
        _onCallbackCorditionComparison = onCallbackCorditionComparison;
        _onCallbackOnStateStay = onCallbackOnStateStay;
    }

    private void Update()
    {
        if(_onCallbackOnStateStay == null || _onCallbackCorditionComparison == null)
        {
            return;
        }

        _onCallbackOnStateStay();

        _npc.AroundSearch(_targetType.ToString(), _evasionType.ToString(), (targetPos, evasionPos) =>
        {
            _onCallbackCorditionComparison(targetPos, evasionPos);
        });
    }

    private void OnDrawGizmos()
    {
        if(_npc != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_npc.transform.position, _npc.searchRadius);
        }
    }
}
