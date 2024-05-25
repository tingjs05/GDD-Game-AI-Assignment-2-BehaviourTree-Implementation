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
            // struct to store the connections between nodes
            public struct Connection
            {
                public float distance;
                public Node node;

                public Connection(float _distance, Node _node)
                {
                    distance = _distance;
                    node = _node;
                }
            }

            // public variables
            public Vector3 position;
            public List<Connection> connections = new List<Connection>();
            public Node previousNode;

            // constructor
            public Node(Vector3 _position)
            {
                position = _position;
            }

            // public method to generate connections between nodes
            public void GenerateConnections(float frequency, bool includeDiagonal = true)
            {
                // do not run if node manager instance is not found
                if (NodeManager.Instance == null) return;
                // set the max distance for a connection between nodes
                float maxDistance = includeDiagonal? frequency * Mathf.Sqrt(2) : frequency;
                float distance;

                foreach (Node node in NodeManager.Instance.nodes)
                {
                    // get the distance between nodes
                    distance = Vector3.Distance(position, node.position);
                    // ensure node is only within certain distance before making a connection
                    if (distance > maxDistance) continue;
                    // add the connection to connections list
                    connections.Add(new Connection(
                            distance, 
                            node
                        ));
                }
            }
        }
    }
}
