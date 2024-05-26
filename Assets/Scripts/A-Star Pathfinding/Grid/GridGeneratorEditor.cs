using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Astar.Nodes;

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
                    // ensure node manager is not null
                    gridGenerator.InstantiateNodeManager();
                    // generate grid
                    gridGenerator.GenerateGrid();
                    // if want to show node connections, need to generate nodes before being able to show
                    if (!gridGenerator.showNodeConnections) return;
                    gridGenerator.GenerateNodes();
                }
            }
        }
    }
}
