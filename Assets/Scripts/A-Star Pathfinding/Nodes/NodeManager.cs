using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Astar
{
    namespace Nodes
    {
        public class NodeManager
        {
            // static singleton instances
            public static NodeManager Instance { get; private set; }
            // list to store all the nodes
            public List<Node> nodes = new List<Node>();

            // constructor to create a singleton
            public NodeManager()
            {
                // set instance to 'this' if instance is null
                // instances that are not set to the public property would be cleaned up by GC
                Instance ??= this;
            }

            // public methods to get nearest nodes to position
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
