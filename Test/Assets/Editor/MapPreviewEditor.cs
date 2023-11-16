using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MapGanerate
{
    [CustomEditor(typeof(MapPerview))]
    public class MapPreviewEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            MapPerview perivew = (MapPerview)target;

            if (DrawDefaultInspector() == true)
            {
                if (perivew.autoUpdate == true)
                {
                    perivew.Initialize();
                    perivew.DrawMapInEditor();

                    return;
                }
            }

            if (GUILayout.Button("Generate"))
            {
                perivew.Initialize();
                perivew.DrawMapInEditor();
            }
        }
    }
}
