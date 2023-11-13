using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using MapGanerate;

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapGenerator mapGenerator = (MapGenerator)target;

        if (DrawDefaultInspector() == true)
        {
            if(mapGenerator.autoUpdate == true)
            {
                mapGenerator.Initialize();
                mapGenerator.DrawMapInEditor();

                return;
            }
        }

        if(GUILayout.Button("Generate"))
        {
            mapGenerator.Initialize();
            mapGenerator.DrawMapInEditor();
        }
    }
}
