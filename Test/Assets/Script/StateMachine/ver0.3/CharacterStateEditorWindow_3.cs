using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public enum eStateRectangleColor
{
    white,
    orange
}

public class CharacterStateEditorWindow_3 : EditorWindow
{
    private enum eContextMode
    {
        Non = -1,
        MakeTransition,
        SetIdle,
        OpenCode,
        Delete
    }

    private StateContainer _container;

    private eContextMode _contextMode;

    private bool _useGenegicMenu = false;

    private bool _firstLoad = false;

    private Vector2 _stateLabelSize;

    private Rect _backgroundRect;

    private Material _arrowMaterial = null;

    private GUIContent _title;

    private GUIStyle _editorStateNameStyle;

    private StateData _selectState = null;

    private StateData _selectGenegicMenuState = null;

    private void OnEnable()
    {
        _firstLoad = true;
    }

    private void OnDisable()
    {
        _contextMode = eContextMode.Non;
        _firstLoad = false;
        _selectState = null;
        _selectGenegicMenuState = null;
    }

    [UnityEditor.Callbacks.OnOpenAsset(1)]
    public static bool OpenGameStateFlow(int instanceID, int line)
    {
        try
        {
            StateContainer container = (StateContainer)EditorUtility.InstanceIDToObject(instanceID);

            var window = GetWindow<CharacterStateEditorWindow_3>();
            window.SetContariner(container);

            return true;
        }
        catch
        {
            return false;
        }
    }

    private void SetContariner(StateContainer container)
    {
        EditorUtility.SetDirty(container);

        _container = container;
        _firstLoad = true;
    }

    private void OnGUI()
    {
        Setting();

        if(_container == null)
        {
            return;
        }

        MouseDown();
        MouseDrag();
        MouseUp();

        DrawStateRectangle();

        AddState();
        GenericMenu();
    }

    // Drawing

    private void Setting()
    {
        if (_firstLoad == true)
        {
            _title = new GUIContent("State Container");
            titleContent = _title;

            _stateLabelSize = new Vector2(160f, 45f);

            _contextMode = eContextMode.Non;

            _editorStateNameStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter
            };

            _firstLoad = false;
        }

        GUILayout.BeginHorizontal();
        {
            if (_container != null)
            {
                EditorGUILayout.LabelField(_container.name, GUILayout.Width(150));

                EditorGUILayout.LabelField("|", _editorStateNameStyle, GUILayout.Width(10));

                _container.searchTag = EditorGUILayout.TagField("Search Tag", _container.searchTag, GUILayout.Width(315));

                EditorGUILayout.LabelField("|", _editorStateNameStyle, GUILayout.Width(10));
            }
        }
        GUILayout.EndHorizontal();

        if (_arrowMaterial == null)
        {
            _arrowMaterial = new Material(Shader.Find("Custom/Draw"));
        }

        _backgroundRect = GUILayoutUtility.GetAspectRect(1, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

        GUI.color = Color.gray;
        EditorGUI.DrawTextureTransparent(_backgroundRect, Texture2D.blackTexture, ScaleMode.StretchToFill);
        GUI.color = Color.white;
    }

    public void DrawStateRectangle()
    {
        foreach (var connection in _container.connectionIDs) // line
        {
            Rect StartRect = FindState(connection._startID).GetRect();
            Rect destinationRect = FindState(connection._destinationID).GetRect();
            ChackAngle(StartRect.center, destinationRect.center);
        }

        foreach (var state in _container.states) // state rectangle
        {
            DrawRectangle(state.GetRect(), state.faceColorType);

            GUI.color = Color.black;
            EditorGUI.LabelField(state.GetRect(), state._state.name, _editorStateNameStyle);
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
                            State_ state = (State_)draggedObj;

                            foreach(var one in _container.states)
                            {
                                if(state.name.Contains(one._state.name))
                                {
                                    Event.current.Use();
                                    return;
                                }
                            }

                            Rect rect = new Rect(ConversionVector(Event.current.mousePosition, true), _stateLabelSize);

                            _container.AddStateData(state, rect);
                        }
                    }

                    Event.current.Use();
                }
                break;
        }
    }

    // Input

    private void MouseDown()
    {
        if (Event.current.type == EventType.MouseDown && Event.current.isMouse)
        {
            if (_backgroundRect.Contains(Event.current.mousePosition) == false)
            {
                _selectState = null;
                _selectGenegicMenuState = null;
                _useGenegicMenu = false;
                _contextMode = eContextMode.Non;
                return;
            }

            if (_container.states.Count == 0)
            {
                return;
            }

            if (Event.current.button == 0)
            {
                _selectState = FindState(Event.current.mousePosition);

                if (_selectState == null)
                {
                    _contextMode = eContextMode.Non;
                }

                if (_contextMode != eContextMode.MakeTransition)
                {
                    _selectGenegicMenuState = null;
                }

                switch (_contextMode)
                {
                    case eContextMode.Non:
                        {
                            
                        }
                        break;

                    case eContextMode.MakeTransition:
                        {
                            _container.AddTransitionData(_selectGenegicMenuState._id, _selectState._id);

                            if (_selectState._isIDLE == true)
                            {
                                _selectState.faceColorType = eStateRectangleColor.orange;
                            }
                            else
                            {
                                _selectState.faceColorType = eStateRectangleColor.white;
                            }

                            _selectGenegicMenuState = null;
                            _selectState = null;
                        }
                        break;
                }

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
            if (_selectGenegicMenuState != null)
            {
                Rect start = _selectGenegicMenuState.GetRect();
                Rect end = new Rect(Event.current.mousePosition, new Vector2(1f, 1f));
                ChackAngle(start.center, end.center);

                DrawRectAngleOutline(ReturnStateRectangleVertice(ConversionVector(_selectGenegicMenuState.GetRect().position, false), _stateLabelSize), new Color(1f, 0.4f, 0.2f));

                Repaint();
            }
        }

        if (_selectState != null)
        {
            _selectState.SetPosition(ConversionVector(Event.current.mousePosition, true));

            DrawRectAngleOutline(ReturnStateRectangleVertice(ConversionVector(_selectState.GetRect().position, false), _stateLabelSize), new Color(0.2f, 0.8f, 1f));

            Repaint();
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
                    _selectState.faceColorType = eStateRectangleColor.orange;
                }
                else if (_selectState._isIDLE == false)
                {
                    _selectState.faceColorType = eStateRectangleColor.white;
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

        if (_backgroundRect.Contains(Event.current.mousePosition) == false)
        {
            _useGenegicMenu = false;
            return;
        }

        _selectGenegicMenuState = FindState(Event.current.mousePosition);

        if (_selectGenegicMenuState == null)
        {
            return;
        }

        InStateRectangleGenericMenu();

        Event.current.Use();
    }

    private void InStateRectangleGenericMenu()
    {
        GenericMenu menu = new GenericMenu();

        menu.AddItem(new GUIContent("Make Transition"), false, ContextMode, eContextMode.MakeTransition);

        // https://answers.unity.com/questions/1784325/node-editor-select-handlesbezier.html use this
        foreach (var transition in _container.connectionIDs)
        {
            if(_selectGenegicMenuState._id == transition._startID)
            {
                string name = FindState(transition._destinationID)._state.name;
                menu.AddItem(new GUIContent("Delete Transition/to " + name), false, _container.RemoveTransitionData, transition);
            }
        }

        menu.AddItem(new GUIContent("Set Idle"), false, ContextMode, eContextMode.SetIdle);
        //menu.AddItem(new GUIContent("Open Code"), false, ContextMode, eContextMode.OpenCode);
        menu.AddItem(new GUIContent("Delete"), false, ContextMode, eContextMode.Delete);

        menu.ShowAsContext();
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

                    foreach (var state in _container.states)
                    {
                        state._isIDLE = false;
                        state.faceColorType = eStateRectangleColor.white;
                    }

                    _selectGenegicMenuState._isIDLE = true;
                    _selectGenegicMenuState.faceColorType = eStateRectangleColor.orange;
                    _container.idleStateID = _selectGenegicMenuState._id;

                    _contextMode = eContextMode.Non;
                }
                break;

            case eContextMode.OpenCode:
                {
                    Debug.Log("Not yet implemented...");
                }
                break;

            case eContextMode.Delete:
                {
                    _contextMode = eContextMode.Delete;

                    _container.RemoveStateData(_selectGenegicMenuState._id);
                    _selectGenegicMenuState = null;
                    _selectState = null;

                    _contextMode = eContextMode.Non;
                }
                break;
        }
    }

    // Tools

    private void DrawRectangle(Rect rect, eStateRectangleColor faceColorType)
    {
        Color faceColor = Color.white;

        switch (faceColorType)
        {
            case eStateRectangleColor.white:

                break;

            case eStateRectangleColor.orange:
                faceColor = new Color(1f, 0.6f, 0f);
                break;
        }

        Handles.DrawSolidRectangleWithOutline(rect, faceColor, Color.gray);
    }

    private void DrawRectAngleOutline(Vector3[] vertices, Color color)
    {
        Handles.BeginGUI();

        Handles.color = color;

        Handles.DrawAAPolyLine(13f, vertices);

        Handles.color = Color.white;

        Handles.EndGUI();
    }

    private void ChackAngle(Vector2 startPos, Vector2 endPos)
    {
        Vector2[] result = new Vector2[2];

        Vector2 O = startPos;
        Vector2 X = endPos;
        Vector2 P = new Vector2(endPos.x, startPos.y);

        float R = 5f;
        float OX = Vector2.Distance(O, X);
        float OP = Vector2.Distance(O, P);

        float Rad90 = 90 * Mathf.Deg2Rad;
        float cosAngle = 0f;
        Vector2 dir = (X - O);

        if (dir.x > 0 && dir.y <= 0) // 1
        {
            cosAngle = (-Rad90) - (Mathf.Acos(OP / OX));
        }
        else if (dir.x <= 0 && dir.y <= 0) // 2
        {
            cosAngle = (-Rad90) - (Mathf.Acos(-OP / OX));
        }
        else if (dir.x <= 0 && dir.y > 0) // 3
        {
            cosAngle = (-Rad90) + (Mathf.Acos(-OP / OX));
        }
        else if (dir.x > 0 && dir.y > 0) // 4
        {
            cosAngle = (-Rad90) + (Mathf.Acos(OP / OX));
        }

        result[0] = startPos - new Vector2(R * Mathf.Cos(cosAngle), R * Mathf.Sin(cosAngle));
        result[1] = endPos - new Vector2(R * Mathf.Cos(cosAngle), R * Mathf.Sin(cosAngle));

        DrawTransition(result);
    }

    private void DrawTransition(Vector2[] linePosition)
    {
        Handles.BeginGUI();
        Handles.DrawBezier(linePosition[0], linePosition[1], linePosition[0], linePosition[1], Color.white, null, 4f);
        Handles.EndGUI();

        Vector2 dir = (linePosition[1] - linePosition[0]).normalized;
        float dis = Vector2.Distance(linePosition[0], linePosition[1]) * 0.5f;
        Vector2 center = linePosition[0] + (dir * dis);

        Vector2 a = center + (dir * 4f);
        Vector2 b = (center + (dir * -3f)) + (verticeRotate(dir, 120 * Mathf.Deg2Rad) * 6f);
        Vector2 c = (center + (dir * -3f)) + (verticeRotate(dir, -120 * Mathf.Deg2Rad) * 6f);

        a.y = Screen.height - a.y;
        b.y = Screen.height - b.y;
        c.y = Screen.height - c.y;

        float height = Screen.height - position.height;
        a.y -= height;
        b.y -= height;
        c.y -= height;

        a = InverseLerp(Vector2.zero, Screen.safeArea.size, a);
        b = InverseLerp(Vector2.zero, Screen.safeArea.size, b);
        c = InverseLerp(Vector2.zero, Screen.safeArea.size, c);

        GL.PushMatrix();
        _arrowMaterial.SetPass(0);
        GL.LoadOrtho();
        GL.Begin(GL.TRIANGLES);
        GL.Color(Color.white);
        GL.Vertex(a);
        GL.Vertex(b);
        GL.Vertex(c);
        GL.End();
        GL.PopMatrix();
    }

    private Vector2 ConversionVector(Vector2 originalPosition, bool originalPositionIsMouse)
    {
        Vector2 result = originalPosition;

        if (originalPositionIsMouse == true)
        {
            result = new Vector2(result.x - (_stateLabelSize.x * 0.5f), result.y - (_stateLabelSize.y * 0.5f));
        }
        else if (originalPositionIsMouse == false)
        {
            result = new Vector2(result.x + (_stateLabelSize.x * 0.5f), result.y + (_stateLabelSize.y * 0.5f));
        }

        return result;
    }

    private Vector2 verticeRotate(Vector2 position, float angle)
    {
        return new Vector2(position.x * Mathf.Cos(angle) - position.y * Mathf.Sin(angle), position.x * Mathf.Sin(angle) + position.y * Mathf.Cos(angle));
    }

    public Vector2 InverseLerp(Vector2 start, Vector2 end, Vector2 value)
    {
        return (value - start) / (end - start);
    }

    private Vector3[] ReturnStateRectangleVertice(Vector2 rectanglePosition, Vector2 labelSize)
    {
        Vector3[] result = new Vector3[6];
        result[0] = new Vector2(rectanglePosition.x + labelSize.x * 0.5f, rectanglePosition.y + labelSize.y * 0.5f);
        result[1] = new Vector2(rectanglePosition.x - labelSize.x * 0.5f, rectanglePosition.y + labelSize.y * 0.5f);
        result[2] = new Vector2(rectanglePosition.x - labelSize.x * 0.5f, rectanglePosition.y - labelSize.y * 0.5f);
        result[3] = new Vector2(rectanglePosition.x + labelSize.x * 0.5f, rectanglePosition.y - labelSize.y * 0.5f);
        result[4] = new Vector2(rectanglePosition.x + labelSize.x * 0.5f, rectanglePosition.y + labelSize.y * 0.5f);
        result[5] = new Vector2(rectanglePosition.x - labelSize.x * 0.5f, rectanglePosition.y + labelSize.y * 0.5f);

        return result;
    }

    private StateData FindState(int id)
    {
        foreach (var state in _container.states)
        {
            if (id == state._id)
            {
                return state;
            }
        }

        return null;
    }

    private StateData FindState(Vector2 pos)
    {
        foreach (var state in _container.states)
        {
            if (state.GetRect().Contains(pos) == false)
            {
                continue;
            }

            return state;
        }

        return null;
    }
}