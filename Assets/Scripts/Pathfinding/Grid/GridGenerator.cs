using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Astar.Grid
{
    public class GridGenerator : MonoBehaviour
    {
        // editor fields
        // grid setup fields
        public Vector3 pointOfOrigin;
        public Vector2 gridSize;
        public float gridFrequency = 1f;
        public bool showGizmos = true;

        // pathfinding setup fields
        public float gridObstacleDetectionRange = 1f;

        // hidden fields
        public bool showGridSetupGizmos = true;

        // list to store grid nodes
        public List<Vector3> nodePositions = new List<Vector3>();

        // Start is called before the first frame update
        void Start()
        {
            GenerateGrid();
        }

        public void GenerateGrid()
        {
            // reset list 
            nodePositions.Clear();
            // generate grid
            float x = 0f;
            float z = 0f;
            // loop through each position to gernate grid
            // while (x <= Mathf.Abs(gridSize.x))
            // {
            //     while (z <= Mathf.Abs(gridSize.y))
            //     {
            //         // add value to list
            //         nodePositions.Add(new Vector3(pointOfOrigin.x + x, pointOfOrigin.y, pointOfOrigin.z + z));
            //         // iterate position
            //         z += gridFrequency;
            //     }
            //     // iterate position
            //     x += gridFrequency;
            // }

            for (int i = 0; x < (1 / gridFrequency) * gridSize.x; i++)
            {
                for (int j = 0; j < (1 / gridFrequency) * gridSize.y; j++)
                {
                    // add value to list
                    nodePositions.Add(new Vector3(pointOfOrigin.x + x, pointOfOrigin.y, pointOfOrigin.z + z));
                    // iterate position
                    z += gridFrequency;
                    // reset x if it is the last iteration for this column
                    if (z == ((1 / gridFrequency) * gridSize.y) - 1)
                        x = 0f;
                }
                // iterate position
                x += gridFrequency;
            }
        }

        void OnDrawGizmos() 
        {
            // check if want to show gizmos
            if (!showGizmos) return;

            // check to show setup gizmos
            if (showGridSetupGizmos)
            {
                Vector3 tempPoint;
                // show point of origin
                Gizmos.color = Color.magenta;
                tempPoint = pointOfOrigin;
                Gizmos.DrawSphere(tempPoint, 0.5f);
                // show other points
                tempPoint.x += gridSize.x;
                tempPoint.z += gridSize.y;
                Gizmos.DrawSphere(tempPoint, 0.5f);
                // change color to white
                Gizmos.color = Color.white;
                tempPoint.x -= gridSize.x;
                Gizmos.DrawSphere(tempPoint, 0.5f);
                tempPoint.x += gridSize.x;
                tempPoint.z -= gridSize.y;
                Gizmos.DrawSphere(tempPoint, 0.5f);
                // dont show map if show grid setup
                return;
            }

            // do not run if there are no nodes
            if (nodePositions.Count <= 0) return;
            // show grid
            Gizmos.color = Color.green;
            foreach (Vector3 position in nodePositions)
            {
                Gizmos.DrawSphere(position, 0.1f);
            }
        }
    }
}
