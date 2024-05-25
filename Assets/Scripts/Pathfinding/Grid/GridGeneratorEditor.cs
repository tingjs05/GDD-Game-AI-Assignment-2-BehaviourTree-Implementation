using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Astar
{
    namespace Grid
    {
        [CustomEditor(typeof(GridGenerator))]
        public class GridGeneratorEditor : Editor
        {
            public override void OnInspectorGUI()
            {
                GridGenerator gridGenerator = (GridGenerator) target;

                // draw default editor
                DrawDefaultInspector();
                // add a small space between custom GUI and default GUI
                EditorGUILayout.Space();

                // generate grid
                if (GUILayout.Button("Generate Grid"))
                {
                    gridGenerator.GenerateGrid();
                }
            }
        }
    }
}
