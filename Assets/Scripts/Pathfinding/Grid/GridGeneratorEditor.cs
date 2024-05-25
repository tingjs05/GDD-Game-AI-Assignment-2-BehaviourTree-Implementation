using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Astar.Grid
{
    [CustomEditor(typeof(GridGenerator))]
    public class GridGeneratorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            GridGenerator gridGenerator = (GridGenerator) target;

            // grid setup fields
            EditorGUILayout.LabelField("Grid Setup Fields", EditorStyles.boldLabel);
            gridGenerator.pointOfOrigin = EditorGUILayout.Vector3Field("Point of Origin", gridGenerator.pointOfOrigin);
            gridGenerator.gridSize = EditorGUILayout.Vector2Field("Grid Size", gridGenerator.gridSize);
            gridGenerator.gridFrequency = EditorGUILayout.FloatField("Grid Frequency", gridGenerator.gridFrequency);
            // toggle gizmos
            if (GUILayout.Button("Toggle Grid Setup Gizmos"))
            {
                gridGenerator.showGridSetupGizmos = !gridGenerator.showGridSetupGizmos;
            }
            // generate grid
            if (GUILayout.Button("Generate Grid"))
            {
                gridGenerator.GenerateGrid();
            }

            // pathfinding setup fields

            gridGenerator.showGizmos = EditorGUILayout.Toggle("Show Gizmos", gridGenerator.showGizmos);
        }
    }
}
