using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Astar.Pathfinding;
using Astar.Nodes;

public class Test : MonoBehaviour
{
    public Vector3 startPos, endPos;
    Pathfinding pathfinder;
    List<Node> path;

    bool run = false;

    // Start is called before the first frame update
    void Start()
    {
        // creat a new instance of path finder
        pathfinder = new Pathfinding();
    }

    void Update()
    {
        if (run) return;
        if (!(NodeManager.Instance.nodes.Count <= 0))
        {
            run = true;
            path = pathfinder.FindPath(startPos, endPos);
        }
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(startPos, 0.3f);
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(endPos, 0.3f);

        // if pathfinder cannot be found, dont draw gizmos for path finding
        if (pathfinder == null) return;
        
        // draw horizon nodes
        foreach (Node node in pathfinder.open)
        {
            Gizmos.color = Color.blue;
            // if node is part of path, draw it as yellow
            if (path != null && path.Contains(node))
            {
                Gizmos.color = Color.yellow;
                // draw connection to rest of the path if there is a previous node set
                if (node.previousNode != null) Debug.DrawRay(node.position, node.previousNode.position - node.position, Color.yellow);
            }
            Gizmos.DrawSphere(node.position, 0.2f);
        }

        // draw visited nodes
        foreach (Node node in pathfinder.closed)
        {
            Gizmos.color = Color.cyan;
            // if node is part of path, draw it as yellow
            if (path != null && path.Contains(node))
            {
                Gizmos.color = Color.yellow;
                // draw connection to rest of the path if there is a previous node set
                if (node.previousNode != null) Debug.DrawRay(node.position, node.previousNode.position - node.position, Color.yellow);
            }
            Gizmos.DrawSphere(node.position, 0.2f);
        }
    }
}
