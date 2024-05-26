using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Astar
{
    namespace Nodes
    {
        public class Node
        {
            // public variables
            public Vector3 position { get; private set; }
            public List<Node> connections { get; private set; } = new List<Node>();
            public Node previousNode;
            public int G, H;

            // constructor
            public Node(Vector3 _position)
            {
                position = _position;
            }

            // public method to generate connections between nodes
            public virtual void GenerateConnections(float frequency)
            {
                // reset connections
                connections.Clear();
                // do not run if node manager instance is not found
                if (NodeManager.Instance == null) return;
                // set the max distance for a connection between nodes
                float maxDistance = (float) System.Math.Round(frequency * Mathf.Sqrt(2), 2);
                // loop through each node to find which nodes can form connection
                foreach (Node node in NodeManager.Instance.nodes)
                {
                    // if the node is itself, dont add as a connection
                    if (node.Equals(this)) continue;
                    // ensure node is only within certain distance before making a connection
                    if (Vector3.Distance(position, node.position) > maxDistance) continue;
                    // add the connection to connections list
                    connections.Add(node);
                }
            }
        }
    }
}
