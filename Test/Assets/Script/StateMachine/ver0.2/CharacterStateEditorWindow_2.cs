using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Newtonsoft.Json;
using System;

[System.Serializable]
public class Editor_State
{
    public int id;

    public float _x;

    public float _y;

    public float _width;

    public float _height;

    public bool _isIDLE = false;

    public eColor faceColorType = eColor.white;

    public string stateType;

    public void SetRect(Rect rect)
    {
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
public class Editor_StateTransition
{
    public int startID;

    public int destinationID;
}

[System.Serializable]
public class Editor_SendStateData
{
    public int startID;

    public List<int> destinationStateIDs = new List<int>();
}

[System.Serializable]
public class CharacterStateEditorData
{
    public int id;

    public string searchTag;

    public List<Editor_State> editorStates;

    public Dictionary<int, List<Editor_StateTransition>> editorStateDic;

    public CharacterStateEditorData()
    {
        id = 0;
        searchTag = string.Empty;
        editorStates = new List<Editor_State>();
        editorStateDic = new Dictionary<int, List<Editor_StateTransition>>();
    }

    public void SetSearchTag(string tag)
    {
        searchTag = tag;
    }
}

public enum eColor
{
    white,
    gray,
    orange
}

public class CharacterStateEditorWindow : EditorWindow
{
    private enum eContextMode
    {
        Non = -1,
        MakeTransition,
        SetIdle,
        Delete
    }

    [MenuItem("WindowTest/Character State Refactoring")]
    public static void Initialize()
    {
        CharacterStateEditorWindow window = (CharacterStateEditorWindow)GetWindow(typeof(CharacterStateEditorWindow), false, "Character State Controller Refactoring");
        window.Show();

        window.minSize = new Vector2(300f, 200f);
    }

    private CharacterStateController _characterStateController = null;

    private CharacterStateEditorData _database = null;

    private eContextMode _contextMode = eContextMode.Non;

    private string _dataKEY = string.Empty;

    private string _searchTag = string.Empty;

    private bool _useGenegicMenu = false;

    private bool _loadDone = false;

    private Vector2 _mousePosition = Vector2.zero;

    private Vector2 _stateLabelSize = new Vector2(160f, 45f);

    private Vector2[] _arrowVerts = new Vector2[3];

    private Vector2 _toolBarSize = Vector2.zero;

    private Rect _backgroundRect;

    private GUIStyle _editorStateNameStyle;

    private Texture2D _arrowTexture = null;

    private Editor_State _selectState = null;

    private Editor_State _selectGenegicMenuState = null;

    private void OnEnable()
    {
        _database = new CharacterStateEditorData();
    }

    private void OnDisable()
    {
        _loadDone = false;
    }

    private void OnGUI()
    {
        _mousePosition = Event.current.mousePosition;

        Setting();

        EditorGUILayout.Space(5f);

        if (_characterStateController == null)
        {
            return;
        }

        if (_loadDone == false)
        {
            return;
        }

        Background();
        DrawStateRectangle();

        AddState();

        MouseDown();
        MouseDrag();
        MouseUp();

        GenericMenu();

        Repaint();
    }

    private void Setting()
    {
        EditorGUILayout.BeginHorizontal();
        {
            _characterStateController = (CharacterStateController)EditorGUILayout.ObjectField("Character State Controller", _characterStateController, typeof(CharacterStateController), true, GUILayout.Width(315), GUILayout.Height(20));

            if (_characterStateController != null)
            {
                _dataKEY = _characterStateController.gameObject.GetInstanceID().ToString();
            }

            SettingButton();
        }
        EditorGUILayout.EndHorizontal();

        _arrowTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Prefab/StateMachine/Editor_Arrow");

        _editorStateNameStyle = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter
        };
    }

    private void SettingButton()
    {
        if (GUILayout.Button("Save", GUILayout.Width(60), GUILayout.Height(20)))
        {
            if (_characterStateController == null)
            {
                return;
            }

            if (_loadDone == false)
            {
                return;
            }

            _database.searchTag = _searchTag;

            _characterStateController.dataContainer.SetData(_database);

            string data = JsonConvert.SerializeObject(_database);
            Debug.Log("Save   : " + data);
            EditorPrefs.SetString(_dataKEY, data);
        }

        if (GUILayout.Button("Load", GUILayout.Width(60), GUILayout.Height(20)))
        {
            if (_characterStateController == null)
            {
                return;
            }

            if (_loadDone == true)
            {
                return;
            }

            var data = EditorPrefs.GetString(_dataKEY);
            Debug.Log("Load   : " + data);
            CharacterStateEditorData database = JsonConvert.DeserializeObject<CharacterStateEditorData>(data);

            if (database == null)
            {
                database = new CharacterStateEditorData();
            }

            _database = database;
            _searchTag = _database.searchTag;

            _loadDone = true;
        }

        if (GUILayout.Button("Clear", GUILayout.Width(60), GUILayout.Height(20)))
        {
            if (_characterStateController == null)
            {
                return;
            }

            if (_database == null)
            {
                return;
            }

            _database.id = 0;

            _database.editorStates.Clear();
            _database.editorStateDic.Clear();

            _characterStateController.dataContainer.Clear();
        }

        EditorGUILayout.Space(10f);

        _searchTag = EditorGUILayout.TagField("Search Tag", _searchTag, GUILayout.Width(315), GUILayout.Height(20));
    }

    private void Background()
    {
        _backgroundRect = GUILayoutUtility.GetAspectRect(1, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

        GUI.color = Color.gray;
        EditorGUI.DrawTextureTransparent(_backgroundRect, Texture2D.blackTexture, ScaleMode.StretchToFill);
        GUI.color = Color.white;
    }

    public void DrawStateRectangle()
    {
        foreach (var state in _database.editorStates)
        {
            if (_database.editorStateDic.ContainsKey(state.id) == false)
            {
                _database.editorStateDic.Add(state.id, new List<Editor_StateTransition>());
            }

            if (_database.editorStateDic[state.id].Count > 0)
            {
                int count = Mathf.CeilToInt(_database.editorStateDic[state.id].Count * 0.5f);

                for (int i = 0; i < _database.editorStateDic[state.id].Count; i++)
                {
                    Editor_State destination = FindState(_database.editorStateDic[state.id][i].destinationID);
                    Rect rect = state.GetRect();
                    rect.position = new Vector2(rect.position.x + ((count - i) * 0.5f), rect.position.y);

                    MakeLine(rect, destination.GetRect());
                }
            }
        }

        foreach (var state in _database.editorStates)
        {
            MakeRectangleWithOutline(state.GetRect(), state.faceColorType);

            GUI.color = Color.black;
            EditorGUI.LabelField(state.GetRect(), state.stateType, _editorStateNameStyle);
            GUI.color = Color.white;
        }
    }

    private void AddState()
    {
        switch (Event.current.type)
        {
            case EventType.DragUpdated:
            case EventType.DragPerform:
                {
                    if (_backgroundRect.Contains(Event.current.mousePosition) == false)
                    {
                        break;
                    }

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (Event.current.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();

                        foreach (var draggedObj in DragAndDrop.objectReferences)
                        {
                            CharacterStateSO state = (CharacterStateSO)draggedObj;

                            Rect rect = new Rect(ConversionVector(_mousePosition, true), _stateLabelSize);

                            Editor_State stateEditor = new Editor_State();
                            stateEditor.id = _database.id;
                            stateEditor.SetRect(rect);
                            stateEditor.faceColorType = eColor.white;
                            stateEditor.stateType = state._stateType;

                            _database.id++;

                            _database.editorStates.Add(stateEditor);
                        }
                    }

                    Event.current.Use();
                }
                break;
        }
    }

    private void MouseDown()
    {
        if(Event.current.type == EventType.MouseDown && Event.current.isMouse)
        {
            if (_backgroundRect.Contains(Event.current.mousePosition) == false)
            {
                _contextMode = eContextMode.Non;
                return;
            }

            if (_database.editorStates.Count == 0)
            {
                return;
            }

            if (Event.current.button == 0)
            {
                _selectState = FindClickState(_mousePosition);
                _selectState.faceColorType = eColor.gray;

                if (_contextMode != eContextMode.MakeTransition)
                {
                    _selectGenegicMenuState = null;
                    return;
                }

                foreach (var state in _database.editorStateDic[_selectGenegicMenuState.id])
                {
                    if (state.destinationID == _selectGenegicMenuState.id)
                    {
                        _contextMode = eContextMode.Non;
                        return;
                    }
                }

                Editor_StateTransition transition = new Editor_StateTransition();
                transition.startID = _selectGenegicMenuState.id;
                transition.destinationID = _selectState.id;

                //_selectGenegicMenuState.state.connectionStateID = _selectState.id;
                _database.editorStateDic[_selectGenegicMenuState.id].Add(transition);

                _contextMode = eContextMode.Non;
            }

            if (Event.current.button == 1)
            {
                _contextMode = eContextMode.Non;
                _useGenegicMenu = true;
                return;
            }

            Event.current.Use();
        }
    }

    private void MouseDrag()
    {
        if (_contextMode == eContextMode.MakeTransition)
        {
            Rect rect = new Rect(_mousePosition, new Vector2(1f, 1f));
            MakeLine(_selectGenegicMenuState.GetRect(), rect);
        }

        if (_selectState != null)
        {
            _selectState.SetPosition(ConversionVector(_mousePosition, true));
        }
    }

    private void MouseUp()
    {
        if (_selectState == null)
        {
            return;
        }

        if (Event.current.type == EventType.MouseUp && Event.current.isMouse)
        {
            if (_backgroundRect.Contains(Event.current.mousePosition) == false)
            {
                return;
            }

            if (Event.current.button == 0)
            {
                if (_selectState._isIDLE == true)
                {
                    _selectState.faceColorType = eColor.orange;
                }
                else if (_selectState._isIDLE == false)
                {
                    _selectState.faceColorType = eColor.white;
                }

                _selectState = null;
            }

            Event.current.Use();
        }
    }

    private void GenericMenu()
    {
        if (_useGenegicMenu == false)
        {
            return;
        }

        if (_backgroundRect.Contains(_mousePosition) == false)
        {
            _useGenegicMenu = false;
            return;
        }

        _selectGenegicMenuState = FindClickState(_mousePosition);

        GenericMenu menu = new GenericMenu();

        menu.AddItem(new GUIContent("Make Transition"), false, ContextMode, eContextMode.MakeTransition);
        menu.AddItem(new GUIContent("Set Idle"), false, ContextMode, eContextMode.SetIdle);
        menu.AddItem(new GUIContent("Delete"), false, ContextMode, eContextMode.Delete);

        menu.ShowAsContext();

        Event.current.Use();
    }

    private void ContextMode(object obj)
    {
        switch ((eContextMode)obj)
        {
            case eContextMode.MakeTransition:
                {
                    _contextMode = eContextMode.MakeTransition;
                }
                break;

            case eContextMode.SetIdle:
                {
                    _contextMode = eContextMode.SetIdle;

                    foreach(var state in _database.editorStates)
                    {
                        state._isIDLE = false;
                        state.faceColorType = eColor.white;
                    }

                    _selectGenegicMenuState._isIDLE = true;
                    _selectGenegicMenuState.faceColorType = eColor.orange;
                    _characterStateController.idleStateID = _selectGenegicMenuState.id;

                    _contextMode = eContextMode.Non;
                }
                break;

            case eContextMode.Delete:
                {
                    _contextMode = eContextMode.Delete;

                    _database.editorStateDic.Remove(_selectGenegicMenuState.id);
                    _database.editorStates.Remove(_selectGenegicMenuState);
                    _selectState = null;

                    _contextMode = eContextMode.Non;
                }
                break;
        }
    }

    private Vector2 ConversionVector(Vector2 originalPosition, bool originalPositionIsMouse)
    {
        Vector2 result = Vector2.zero;

        if(originalPositionIsMouse == true)
        {
            result = new Vector2(originalPosition.x - (_stateLabelSize.x * 0.5f), originalPosition.y - (_stateLabelSize.y * 0.5f));
        }
        else if (originalPositionIsMouse == false)
        {
            result = new Vector2(originalPosition.x + (_stateLabelSize.x * 0.5f), originalPosition.y + (_stateLabelSize.y * 0.5f));
        }

        return result;
    }

    private Editor_State FindState(int id)
    {
        Editor_State result = new Editor_State();

        foreach (var state in _database.editorStates)
        {
            if(state.id != id)
            {
                continue;
            }

            result = state;
        }

        return result;
    }

    private Editor_State FindClickState(Vector2 pos)
    {
        Editor_State result = new Editor_State();

        foreach (var stateEditor in _database.editorStates)
        {
            if (stateEditor.GetRect().Contains(pos) == false)
            {
                continue;
            }

            result = stateEditor;
            break;
        }

        return result;
    }

    private void MakeLine(Rect start, Rect end)
    {
        //Vector2 dir = (end.center - start.center).normalized;
        //float dis = Vector2.Distance(end.center, start.center) * 0.5f;
        //Vector2 pos = start.center + (dir * dis);

        Handles.BeginGUI();

        Handles.DrawBezier(start.center, end.center, new Vector2(start.center.x, start.center.y), new Vector2(end.center.x, end.center.y), Color.white, null, 5f);

        Handles.EndGUI();

        //SetArrowVerts(pos, Quaternion.LookRotation(dir), ref _arrowVerts, (result) =>
        //{
        //    GL.PushMatrix();
        //    GL.LoadOrtho();
        //    GL.Begin(GL.TRIANGLES);
        //    GL.Vertex3(result[0].x, result[0].y, 0);
        //    GL.Vertex3(result[1].x, result[1].y, 0);
        //    GL.Vertex3(result[2].x, result[2].y, 0);
        //    GL.End();
        //    GL.PopMatrix();
        //});
    }

    private void SetArrowVerts(Vector2 arrowPosition, Quaternion angle, ref Vector2[] vertice, Action<Vector2[]> onCallbackResult)
    {
        vertice[0] = new Vector2(arrowPosition.x, arrowPosition.y + 2f); // Top
        vertice[1] = new Vector2(arrowPosition.x - 1, arrowPosition.y); // Left
        vertice[2] = new Vector2(arrowPosition.x + 1, arrowPosition.y); // Right

        onCallbackResult(vertice);
    }

    private Vector2 verticeRotate(Vector2 position, float angle)
    {
        return new Vector2(position.x * Mathf.Cos(angle) - position.y * Mathf.Sin(angle), position.x * Mathf.Sin(angle) + position.y * Mathf.Cos(angle));
    }

    private void MakeRectangleWithOutline(Rect rect, eColor faceColorType)
    {
        Color faceColor = Color.white;

        switch(faceColorType)
        {
            case eColor.white:

                break;

            case eColor.gray:
                faceColor = Color.gray;
                break;

            case eColor.orange:
                faceColor = new Color(1f, 0.6f, 0f);
                break;
        }

        Handles.DrawSolidRectangleWithOutline(rect, faceColor, Color.gray);
    }
}
