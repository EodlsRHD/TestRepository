using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Character State Data Container", menuName = "Character State Refactoring/Data Container", order = int.MaxValue)]
public class CharacterStateDataContainer : ScriptableObject
{
    [SerializeField]
    private int _idleStateID = 0;

    [SerializeField]
    private string _searchTag = string.Empty;

    [SerializeField]
    private List<Editor_State> _characterStates = new List<Editor_State>();

    [SerializeField]
    private List<Editor_SendStateData> _editorConnectionStateDatas = new List<Editor_SendStateData>();

    public int idleStateID
    {
        get { return _idleStateID; }
    }

    public string searchTag
    {
        get { return _searchTag; }
    }

    public void SetData(CharacterStateEditorData editorDatabase)
    {
        _searchTag = editorDatabase.searchTag;

        foreach (var editorData in editorDatabase.editorStates)
        {
            if(editorData._isIDLE == true)
            {
                _idleStateID = editorData.id;
            }

            _characterStates.Add(editorData);
        }

        foreach(var editorDataKEY in editorDatabase.editorStateDic.Keys)
        {
            Editor_SendStateData stateData = new Editor_SendStateData();
            stateData.startID = editorDataKEY;

            foreach(var editorData in editorDatabase.editorStateDic[editorDataKEY])
            {
                stateData.destinationStateIDs.Add(editorData.destinationID);
            }

            _editorConnectionStateDatas.Add(stateData);
        }
    }

    public List<State> GetStates()
    {
        List<State> result = new List<State>();

        foreach(var characterState in _characterStates)
        {
            State newState = new State();

            CharacterState state = StateMachine.instance.GetState(characterState.stateType);

            newState.SetUp(characterState.id, state);

            result.Add(newState);
        }

        return result;
    }

    public Dictionary<int, List<StateTransition>> GetConnectionState(List<State> states)
    {
        Dictionary<int, List<StateTransition>> result = new Dictionary<int, List<StateTransition>>();

        foreach(var data in _editorConnectionStateDatas)
        {
            List<StateTransition> transitions = new List<StateTransition>();

            foreach (var destinationID in data.destinationStateIDs)
            {
                StateTransition transition = new StateTransition();
                transition.destinationState = FindState(destinationID, states);

                transitions.Add(transition);
            }

            result.Add(data.startID, transitions);
        }

        return result;
    }

    public void Clear()
    {
        _characterStates.Clear();
        _editorConnectionStateDatas.Clear();
    }

    private State FindState(int id, List<State> states)
    {
        State result = new State();

        foreach(var state in states)
        {
            if(state.id != id)
            {
                continue;
            }

            result = state;
            break;
        }

        return result;
    }
}
