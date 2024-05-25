using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Astar
{
    namespace Nodes
    {
        [ExecuteInEditMode]
        public class NodeManager : MonoBehaviour
        {
            // static singleton instances
            public static NodeManager Instance;
            // list to store all the nodes
            public List<Node> nodes = new List<Node>();

            // singleton
            void Awake()
            {
                if (Instance == null)
                    Instance = this;
                else if (Instance != this)
                    Destroy(gameObject);
            }

            void Update()
            {
                // do not run when game is playing
                if (Application.isPlaying) return;

                // set singleton
                if (Instance == null)
                    Instance = this;
                else if (Instance != this)
                    Destroy(gameObject);
            }

            // public methods
            public Node GetNearestNode(Vector3 position)
            {
                // do not run if there are no items in the list
                if (nodes.Count <= 0) return null;
                // store nearest node, default set to first item of list
                Node currentNearestNode = nodes[0];
                // loop through list to find nearest node
                for (int i = 0; i < nodes.Count; i++)
                {
                    if (Vector3.Distance(position, nodes[i].position) <= Vector3.Distance(position, currentNearestNode.position))
                            currentNearestNode = nodes[i];
                }
                // return nearest node
                return currentNearestNode;
            }

            public (Node, Node) GetNearestNode(Vector3 position1, Vector3 position2)
            {
                // do not run if there are no items in the list
                if (nodes.Count <= 0) return (null, null);
                // store nearest node, default set to first item of list
                Node currentNearestNode1 = nodes[0];
                Node currentNearestNode2 = nodes[0];
                // loop through list to find nearest node
                for (int i = 0; i < nodes.Count; i++)
                {
                    if (Vector3.Distance(position1, nodes[i].position) < Vector3.Distance(position1, currentNearestNode1.position))
                            currentNearestNode1 = nodes[i];
                    if (Vector3.Distance(position2, nodes[i].position) < Vector3.Distance(position2, currentNearestNode2.position))
                            currentNearestNode2 = nodes[i];
                }
                // return nearest node
                return (currentNearestNode1, currentNearestNode2);
            }
        }
    }
}
