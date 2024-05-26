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
            Node startNode, endNode, previousNode;
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
                // reset previous node
                previousNode = null;
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
                // while (!pathFound)
                // {
                //     CalculatePath(path[0]);
                // }

                for (int i = 0; i < 50; i++)
                {
                    if (pathFound) break;
                    CalculatePath(path[0]);
                }

                if (!pathFound) Debug.Log("path not found!");
                Debug.Log(path.Count);
                
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
                // remove from open list since its already opened
                open.Remove(node);
                // add all connected nodes to open list
                foreach (Node connection in node.connections)
                {
                    // if connection is the end point, set the previous node as current node, 
                    // and mark path found as true
                    if (connection.Equals(endNode))
                    {
                        // set previous node
                        connection.previousNode = node;
                        // complete path find
                        pathFound = true;
                        // break out of loop when found end node, no need to continue searching
                        break;
                    }
                    // find if the node from the connection is already known
                    if (open.Contains(connection))
                    {
                        // if the current node is cheaper than the connection's previous node
                        // change the previous node connection to current node
                        if (connection.previousNode == null || GetCost(node.position) < GetCost(connection.previousNode.position))
                            connection.previousNode = node;
                        // do not add connection to open if it is already known
                        continue;
                    }
                    // if node is not seen before, add to open list
                    open.Add(connection);
                }
                // cache previous node if it is not null
                if (previousNode != null) node.previousNode = previousNode;
                previousNode = node;
                // move node to closed list after visiting it
                closed.Add(node);
            }

            void CalculatePath(Node node)
            {
                // ensure a previous node is set
                if (node.previousNode == null)
                {
                    Debug.LogError("Pathfinding.cs: path calculation failed due to null node. ");
                    return;
                }
                path.Insert(0, node.previousNode);
                // end path calculation if current node is start node
                if (node.Equals(startNode)) pathFound = true;
            }
            
            // method to sort list based on cost, where cost = distance travelled + remaining distance
            List<Node> SortList(List<Node> list)
            {
                // temporary list to apply sorting to
                List<Node> tempList = new List<Node>(list);
                // store current nearest node
                Node nearestNode = tempList[0];
                // sort the list according to cost (distance) from position
                for (int i = 0; i < tempList.Count - 1; i++)
                {
                    for (int j = 0; j < tempList.Count - i; j++)
                    {
                        if (GetCost(tempList[j + i].position) < GetCost(nearestNode.position))
                            nearestNode = tempList[j + i];
                    }
                    // move nearest node to back of working list
                    tempList.Remove(nearestNode);
                    tempList.Insert(i, nearestNode);
                }
                // return sorted temp list
                return tempList;
            }

            // methods to find cost of node
            int GetCost(Vector3 nodePos)
            {
                return FindManhattanDistance(startNode.position, nodePos) + 
                    FindManhattanDistance(endNode.position, nodePos);
            }

            int FindManhattanDistance(Vector3 start, Vector3 end)
            {
                return Mathf.Abs(ConvertToInt(end.x) - ConvertToInt(start.x)) + 
                    Mathf.Abs(ConvertToInt(end.z) - ConvertToInt(start.z));
            }

            int ConvertToInt(float num)
            {
                return (int) Mathf.Round(num * 10);
            }
        }
    }
}
