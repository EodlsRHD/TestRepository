using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class State
{
    [SerializeField]
    private int _id = 0;

    [SerializeField]
    private CharacterState _state = null;

    private Character _character = null;

    private List<StateTransition> _connectionStateTransition = new List<StateTransition>();

    public int id
    {
        get { return _id; }
    }
    public CharacterState state
    {
        get { return _state; }
    }

    public void SetUp(int id, CharacterState state)
    {
        _id = id;
        _state = state;
    }

    public void Initialize(ref Character character, Action<int, Action<List<StateTransition>>> onCallbackFindStateTrantition, Action<Action> onCallbackSetUpdate, Action<Action<List<Character>>> onCallbackSetContition)
    {
        _character = character;
        _state.Initialize(onCallbackSetUpdate);
        onCallbackSetContition(ConditionComparison);

        onCallbackFindStateTrantition(_id, (result) =>
        {
            _connectionStateTransition = result;
        });
    }

    public void ConditionComparison(List<Character> objectDetected)
    {
        foreach (var stateTransition in _connectionStateTransition)
        {
            if (stateTransition.StateTranstionConditionAndOnEnterState(ref _character, ref objectDetected, _state.OnStateExit, state.isActive))
            {
                break;
            }
        }
    }
}

[System.Serializable]
public class StateTransition
{
    private State _destinationState;

    public State destinationState
    {
        set { _destinationState = value; }
    }

    public bool StateTranstionConditionAndOnEnterState(ref Character character, ref List<Character> objectDetected, Action<Character> onCallbackOnExitState, bool isActive)
    {
        if (objectDetected.Count > 0)
        {
            character.NearestObject(ref objectDetected);
        }

        if (_destinationState.state.Condition(ref objectDetected, ref isActive) == false)
        {
            return false;
        }

        onCallbackOnExitState(character);
        _destinationState.state.OnStateEnter(ref character, ref objectDetected);
        return true;
    }
}


public class CharacterStateController : MonoBehaviour
{
    [SerializeField]
    protected Character _character = null;

    [SerializeField]
    protected CharacterStateDataContainer _dataContainer = null;

    [SerializeField]
    protected string _searchTag = string.Empty;

    [SerializeField]
    protected int _idleStateID = 0;

    [SerializeField]
    protected List<State> _states = new List<State>();

    protected Dictionary<int, List<StateTransition>> _connectionStateDic = new Dictionary<int, List<StateTransition>>();

    protected Action _onCallbackOnStateStay = null;

    protected Action<List<Character>> _onCallbackConditionComparison = null;

    protected bool _useAroundSearch = true;

    public CharacterStateDataContainer dataContainer
    {
        get { return _dataContainer; }
    }

    public int idleStateID
    {
        set { _idleStateID = value; }
    }

    private void Start()
    {
        _character.Initialize();
        Initialize(_character);
    }

    public void Initialize(Character character)
    {
        _character = character;

        _searchTag = _dataContainer.searchTag;
        _idleStateID = _dataContainer.idleStateID;
        _states = _dataContainer.GetStates();
        _connectionStateDic = _dataContainer.GetConnectionState(_states);

        foreach(var state in _states)
        {
            state.Initialize(ref _character, GetStateTransition, SetUpdateMethod, SetConditionMethod);
        }

        _character.AroundSearch(_useAroundSearch, _searchTag, (objectDetected) =>
        {
            foreach (var state in _states)
            {
                if(_idleStateID != state.id)
                {
                    continue;
                }

                state.state.OnStateEnter(ref character, ref objectDetected);
                break;
            }
        });
    }

    public void SetSearchTag(string tag)
    {
        _searchTag = tag;
    }

    protected void SetUpdateMethod(Action onCallbackOnStateStay)
    {
        _onCallbackOnStateStay = onCallbackOnStateStay;
    }

    protected void SetConditionMethod(Action<List<Character>> onCallbackConditionComparison)
    {
        _onCallbackConditionComparison = onCallbackConditionComparison;
    }

    public void GetStateTransition(int id, Action<List<StateTransition>> onCallbackResult)
    {
        onCallbackResult(_connectionStateDic[id]);
    }

    public void Update()
    {
        if (_onCallbackOnStateStay == null)
        {
            return;
        }

        _onCallbackOnStateStay();

        _character.AroundSearch(_useAroundSearch, _searchTag, (objectDetected) =>
        {
            _onCallbackConditionComparison(objectDetected);
        });
    }
}