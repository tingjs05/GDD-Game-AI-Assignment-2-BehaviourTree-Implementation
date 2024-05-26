using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Astar.Nodes;
using UnityEditor.MemoryProfiler;

namespace Astar
{
    namespace Grid
    {
        [RequireComponent(typeof(NodeManager))]
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
            public bool showObstacleDetectionRange = false;
            public bool showNodeConnections = false;
            public bool showObstructedNodes = true;
            public bool showGizmos = true;

            // list to store grid nodes
            List<Vector3> nodePositions = new List<Vector3>();
            List<Vector3> obstructedNodePositions = new List<Vector3>();

            // Start is called before the first frame update
            void Start()
            {
                GenerateGrid();
                GenerateNodes();
            }

            // method to generate the grid
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
                        // add value to list depending on if there are obstacles near the node
                        if (Physics.OverlapSphere(currentPosition, gridObstacleDetectionRange, obstacleLayerMask).Length > 0)
                            obstructedNodePositions.Add(currentPosition);
                        else
                            nodePositions.Add(currentPosition);
                        // iterate position
                        z += gridFrequency;
                        // if it is the last column, reset z to 0 to prepare to iterate through next column
                        z = (j >= (1 / gridFrequency) * gridSize.y)? 0f : z;
                    }
                    // iterate position
                    x += gridFrequency;
                }
            }

            // method to generate node classes at each node position, and getting the connections between each node
            public void GenerateNodes()
            {
                // error prevention
                if (NodeManager.Instance == null)
                {
                    Debug.LogError("GridGenerator.cs: NodeManager instance is null! Unable to generate nodes. ");
                    return;
                }
                // ensure that there are node positions already set
                if (nodePositions == null || nodePositions.Count <= 0) return;

                // reset list before adding nodes
                NodeManager.Instance.nodes.Clear();

                // create a node for each node position
                foreach (Vector3 nodePosition in nodePositions)
                {
                    NodeManager.Instance.nodes.Add(new Node(nodePosition));
                }

                // loop through the nodes list and generate connections between the nodes
                foreach (Node node in NodeManager.Instance.nodes)
                {
                    node.GenerateConnections(gridFrequency);
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

                // show node connections if needed
                if (showNodeConnections && NodeManager.Instance != null)
                {
                    foreach (Node node in NodeManager.Instance.nodes)
                    {
                        foreach (Node connection in node.connections)
                        {
                            Debug.DrawRay(node.position, connection.position - node.position, Color.black);
                        }
                    }
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
                    if (!showObstacleDetectionRange) continue;
                    Gizmos.color = Color.white;
                    Gizmos.DrawWireSphere(position, gridObstacleDetectionRange);
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
                    if (!showObstacleDetectionRange) continue;
                    Gizmos.DrawWireSphere(position, gridObstacleDetectionRange);
                }
            }
        }
    }
}
