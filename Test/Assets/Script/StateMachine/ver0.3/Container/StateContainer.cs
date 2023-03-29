using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class StateData
{
    public int _id;

    public State_ _state;

    public float _x;

    public float _y;

    public float _width;

    public float _height;

    public bool _isIDLE = false;

    public eStateRectangleColor faceColorType = eStateRectangleColor.white;

    public void SetStateData(int id, State_ state, Rect rect)
    {
        _id = id;
        _state = state;
        _x = rect.position.x;
        _y = rect.position.y;
        _width = rect.width;
        _height = rect.height;
    }

    public void SetPosition(Vector2 position)
    {
        _x = position.x;
        _y = position.y;
    }

    public Rect GetRect()
    {
        return new Rect(_x, _y, _width, _height);
    }
}

[System.Serializable]
public class StateDestination
{
    public int _startID;

    public int _destinationID;

    public void SetTransition(int startID, int destinationID)
    {
        _startID = startID;
        _destinationID = destinationID;
    }
}

[CreateAssetMenu(fileName = "newStateContainer", menuName = "Character State/StateContainer", order = 1)]
public class StateContainer : ScriptableObject
{
    [SerializeField]
    private int _id;

    [SerializeField]
    private int _idleStateID;

    [SerializeField]
    private string _searchTag;

    [SerializeField]
    private List<StateData> _states = new List<StateData>();

    [SerializeField]
    private List<StateDestination> _connectionIDs = new List<StateDestination>();

    public int id
    {
        get { return _id; }
        set { _id = value; }
    }

    public int idleStateID
    {
        get { return _idleStateID; }
        set { _idleStateID = value; }
    }

    public string searchTag
    {
        get { return _searchTag; }
        set { _searchTag = value; }
    }

    public List<StateData> states
    {
        get { return _states; }
    }

    public List<StateDestination> connectionIDs
    {
        get { return _connectionIDs; }
    }

    public void AddStateData(State_ newState, Rect rect)
    {
        StateData state = new StateData();
        state.SetStateData(_id, newState, rect);

        _states.Add(state);
        _id++;
    }

    public void RemoveStateData(int removeStateID)
    {
        for (int i = _connectionIDs.Count - 1; i >= 0; i--)
        {
            if (_connectionIDs[i]._startID == removeStateID)
            {
                _connectionIDs.Remove(connectionIDs[i]);
                continue;
            }

            if (_connectionIDs[i]._destinationID == removeStateID)
            {
                _connectionIDs.Remove(connectionIDs[i]);
            }
        }

        foreach (var state in _states)
        {
            if (state._id != removeStateID)
            {
                continue;
            }

            if (state._isIDLE == true)
            {
                _idleStateID = -1;
            }

            _states.Remove(state);
            break;
        }

        if (_states.Count == 0)
        {
            _id = 0;
        }
    }

    public void ClearAllData()
    {
        _id = 0;
        _idleStateID = -1;
        _searchTag = string.Empty;

        if (_states != null)
        {
            _states.Clear();
        }

        if (_connectionIDs != null)
        {
            _connectionIDs.Clear();
        }
    }

    public void AddTransitionData(int startID, int destinationID)
    {
        foreach(var IDs in _connectionIDs)
        {
            if(IDs._startID == startID && IDs._destinationID == destinationID)
            {
                return;
            }
        }

        StateDestination transition = new StateDestination();
        transition.SetTransition(startID, destinationID);

        _connectionIDs.Add(transition);
    }

    public void RemoveTransitionData(object obj)
    {
        StateDestination stateDestination = (StateDestination)obj;

        foreach (var connection in _connectionIDs)
        {
            if(stateDestination._startID == connection._startID && stateDestination._destinationID == connection._destinationID)
            {
                _connectionIDs.Remove(connection);
                break;
            }
        }
    }

    public Dictionary<int, List<StateTransitionBase>> SetTransition(Action<Action<Character, Action>> onCallbackSetUpdataMethod)
    {
        Dictionary<int, List<StateTransitionBase>> result = new Dictionary<int, List<StateTransitionBase>>();

        foreach (var state in _connectionIDs)
        {
            if (result.ContainsKey(state._startID) == false)
            {
                result.Add(state._startID, new List<StateTransitionBase>());

                StateData start = FindStateData(state._startID);
                StateTransitionBase startTransition = new StateTransitionBase(start, onCallbackSetUpdataMethod);

                result[state._startID].Add(startTransition);
            }

            StateData destination = FindStateData(state._destinationID);
            StateTransitionBase destinationTransition = new StateTransitionBase(destination, onCallbackSetUpdataMethod);

            result[state._startID].Add(destinationTransition);
        }

        return result;
    }

    private StateData FindStateData(int id)
    {
        foreach(var state in _states)
        {
            if(state._id != id)
            {
                continue;
            }

            return state;
        }

        return null;
    }
}
