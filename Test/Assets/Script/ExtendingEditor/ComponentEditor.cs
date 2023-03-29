using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TestComponent))]
public class ComponentEditor : Editor
{
    private TestComponent _test;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(30f);
        GUILayout.Label("Test Center");

        _test = (TestComponent)target;


        _test.startPoint = EditorGUILayout.Vector3Field("StartPoint", _test.startPoint);
        _test.endPoint = EditorGUILayout.Vector3Field("EndPoint", _test.endPoint);
        _test.transform.position = EditorGUILayout.Vector3Field("Obj Position", _test.transform.position);
        _test.moveSpeed = EditorGUILayout.Slider(_test.moveSpeed, 0f, 10f);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Test Start"))
        {
            _test.TestStart = true;
        }
        if (GUILayout.Button("Restart"))
        {
            _test.TestStart = false;
            _test.transform.position = _test.startPoint;
            _test._pointList.Clear();
        }
        GUILayout.EndHorizontal();
    }
}
