using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ExtendingEditor : EditorWindow
{
    private TestComponent _test;

    [MenuItem("WindowTest/ExtendingEditor")]
    public static void Init()
    {
        ExtendingEditor window = (ExtendingEditor)EditorWindow.GetWindow(typeof(ExtendingEditor), false, "Window Test");
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.Label("Color Setting", EditorStyles.boldLabel);

        _test = (TestComponent)EditorGUILayout.ObjectField("TestComponent Object", _test, typeof(TestComponent), true);

        if(_test != null)
        {
            _test.mat.color = EditorGUILayout.ColorField("Color", _test.mat.color);
            _test.endPoint = EditorGUILayout.Vector3Field("End Point", _test.endPoint);
        }
    }
}
