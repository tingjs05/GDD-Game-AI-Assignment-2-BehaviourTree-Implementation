using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Astar.Nodes;

namespace Astar
{
    namespace Pathfinding
    {
        public class Pathfinding
        {
            // lists to store nodes that have been visited
            public List<Node> open { get; private set; } = new List<Node>();
            public List<Node> closed { get; private set; } = new List<Node>();

            // list to store path
            List<Node> path = new List<Node>();

            // store start and end node after converting position => node
            Node startNode, endNode;
            // boolean to control whether or not a path is found
            bool pathFound;

            public List<Node> FindPath(Vector3 startPosition, Vector3 endPosition)
            {
                // ensure node manager is not null
                if (NodeManager.Instance == null)
                {
                    Debug.LogError("Pathfinding.cs: NodeManager instance is null! Unable to find path. ");
                    return null;
                }
                // ensure nodes are generated
                if (NodeManager.Instance.nodes.Count <= 0)
                {
                    Debug.LogWarning("Pathfinding.cs: FindPath() was called before nodes are generated! Process has been terminated. ");
                    return null;
                }

                // reset boolean
                pathFound = false;
                // reset open and closed lists
                ResetLists();
                // reset path list
                path.Clear();
                // get start and end nodes
                (startNode, endNode) = NodeManager.Instance.GetNearestNode(startPosition, endPosition);
                // reset the previous node of the start node
                startNode.previousNode = null;
                // add start node to open list
                open.Add(startNode);

                // find path
                while (!pathFound)
                {
                    // ensure open has items inside
                    if (open.Count <= 0)
                    {
                        Debug.LogError("Pathfinding.cs: open list is not set. ");
                        break;
                    }
                    // sort open list based on distance to end point
                    open = SortList(open);
                    // open the closest node to the end point
                    OpenNode(open[0]);
                }

                // calculate path
                path.Add(endNode);
                // reset path found boolean
                pathFound = false;
                // calculate path
                while (!pathFound)
                {
                    CalculatePath(path[0]);
                }
                // return path
                return path;
            }

            // method to reset lists
            public void ResetLists()
            {
                open.Clear();
                closed.Clear();
            }

            // code to "open" a node, and check it out
            void OpenNode(Node node)
            {
                // if connection is the end point, set the previous node as current node
                // and mark path found as true
                if (node.Equals(endNode))
                {
                    // complete path find
                    pathFound = true;
                    // break out of loop when found end node, no need to continue searching
                    return;
                }

                // remove from open list since its already opened
                open.Remove(node);

                // add all connected nodes to open list
                foreach (Node connection in node.connections)
                {
                    // do not check connection if connection node is already opened before
                    if (closed.Contains(connection)) continue;

                    // find if the node from the connection is already known
                    if (open.Contains(connection))
                    {
                        // if the current node is cheaper than the connection's previous node
                        // change the connection node's previous node connection to current node
                        if (IsCloserToStartNode(node, connection.previousNode)) connection.previousNode = node;
                        // do not add connection to open if it is already known
                        continue;
                    }
                    // set connection to current node
                    connection.previousNode = node;
                    // if node is not seen before, add to open list
                    open.Add(connection);
                }
                // move node to closed list after visiting it
                closed.Add(node);
            }

            void CalculatePath(Node node)
            {
                // ensure a previous node is set, assuming it is not the starting node
                if (node.previousNode == null && !node.Equals(startNode))
                {
                    Debug.LogError("Pathfinding.cs: path calculation failed due to null node. ");
                    return;
                }
                // insert previous node into path
                path.Insert(0, node.previousNode);
                // end path calculation if current node is start node
                if (node.Equals(startNode)) pathFound = true;
            }
            
            // method to sort list based on cost, where cost = distance travelled + remaining distance
            // using bubble sort
            List<Node> SortList(List<Node> list)
            {
                // temporary list to apply sorting to
                List<Node> tempList = new List<Node>(list);
                // temporary variable to store node when swapping items in list
                Node tempNode;
                // sort the list according to cost (distance) from position
                for (int i = 0; i < tempList.Count - 1; i++)
                {
                    for (int j = 0; j < tempList.Count - i - 1; j++)
                    {
                        // if next item is cheaper, swap nodes
                        if (!(GetCost(tempList[j + 1]) < GetCost(tempList[j]))) continue;
                        // swap items
                        tempNode = tempList[i];
                        tempList[i] = tempList[j];
                        tempList[j] = tempNode;
                    }
                }
                // return sorted temp list
                return tempList;
            }

            // methods to find cost of node
            int GetCost(Node node)
            {
                return FindManhattanDistance(startNode.position, node.position) + 
                    FindManhattanDistance(node.position, endNode.position);
            }

            int FindManhattanDistance(Vector3 start, Vector3 end)
            {
                // return Vector3.Distance(start, end);
                return Mathf.Abs(ConvertToInt(end.x) - ConvertToInt(start.x)) + 
                    Mathf.Abs(ConvertToInt(end.z) - ConvertToInt(start.z));
            }

            int ConvertToInt(float num)
            {
                return (int) Mathf.Round(num * 10);
            }

            // method to compare distance to start node
            bool IsCloserToStartNode(Node currentNode, Node nodeToCompare)
            {
                // use manhattan distance first
                float d1 = FindManhattanDistance(startNode.position, currentNode.position);
                float d2 = FindManhattanDistance(startNode.position, nodeToCompare.position);
                // if both values are equal, use more precise calculation
                if (d1 == d2)
                {
                    d1 = Vector3.Distance(startNode.position, currentNode.position);
                    d2 = Vector3.Distance(startNode.position, nodeToCompare.position);
                }
                // return boolean depending on if distance to current node is smaller than distance to node to compare
                return d1 < d2;
            }
        }
    }
}
