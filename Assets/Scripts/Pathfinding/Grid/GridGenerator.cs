using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Astar.Grid
{
    [System.Serializable]
    public class GridGenerator : MonoBehaviour
    {
        // editor fields
        [Header("Grid Setup Fields")]
        public Vector3 pointOfOrigin = new Vector3(-8f, 0f, -5f);
        public Vector2 gridSize = new Vector3(16f, 10f);
        public float gridFrequency = 1f;

        [Header("Pathfinding Setup Fields")]
        public float gridObstacleDetectionRange = 1f;
        public LayerMask obstacleLayerMask;

        [Header("Gizmos")]
        public bool showGridSetupGizmos = false;
        public bool showObstacleDerectionRange = false;
        public bool showObstructedNodes = true;
        public bool showGizmos = true;

        // list to store grid nodes
        [HideInInspector] public List<Vector3> nodePositions = new List<Vector3>();
        [HideInInspector] public List<Vector3> obstructedNodePositions = new List<Vector3>();

        // Start is called before the first frame update
        void Start()
        {
            GenerateGrid();
        }

        public void GenerateGrid()
        {
            // reset lists
            nodePositions.Clear();
            obstructedNodePositions.Clear();
            // generate grid
            float x = 0f;
            float z = 0f;
            Vector3 currentPosition;
            // loop through each position to gernate grid
            for (int i = 0; i < ((1 / gridFrequency) * gridSize.x) + 1; i++)
            {
                for (int j = 0; j < ((1 / gridFrequency) * gridSize.y) + 1; j++)
                {
                    // set node position
                    currentPosition = new Vector3(pointOfOrigin.x + x, pointOfOrigin.y, pointOfOrigin.z + z);
                    // add value to list depending on if there are obstacles nearby
                    if (ObstacleNearby(currentPosition))
                        obstructedNodePositions.Add(currentPosition);
                    else
                        nodePositions.Add(currentPosition);
                    // iterate position
                    z += gridFrequency;
                    // if it is the last column, reset z to 0 to prepare to iterate through next column
                    z = (j == (1 / gridFrequency) * gridSize.y)? 0f : z;
                }
                // iterate position
                x += gridFrequency;
            }
        }

        bool ObstacleNearby(Vector3 position)
        {
            return Physics.OverlapSphere(position, gridObstacleDetectionRange, obstacleLayerMask).Length > 0;
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

            // if there are no nodes, generate grid
            if (nodePositions.Count <= 0) GenerateGrid();
            // show grid
            foreach (Vector3 position in nodePositions)
            {
                // draw grid node
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(position, 0.1f);
                // draw obstacle detection range
                if (showObstacleDerectionRange)
                {
                    Gizmos.color = Color.white;
                    Gizmos.DrawWireSphere(position, gridObstacleDetectionRange);
                }
            }

            // return if show obstructed nodes is false
            if (!showObstructedNodes) return;
            // set gizmos color
            Gizmos.color = Color.red;
            // show obstructed grid
            foreach (Vector3 position in obstructedNodePositions)
            {
                // draw grid node
                Gizmos.DrawSphere(position, 0.1f);
                // draw obstacle detection range
                if (showObstacleDerectionRange) Gizmos.DrawWireSphere(position, gridObstacleDetectionRange);
            }
        }
    }
}
