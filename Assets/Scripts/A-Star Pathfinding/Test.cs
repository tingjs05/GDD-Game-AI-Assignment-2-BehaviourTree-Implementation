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
            pathfinder.FindPath(startPos, endPos);
        }
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(startPos, 0.2f);
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(endPos, 0.2f);


        if (pathfinder == null) return;

        Gizmos.color = Color.blue;
        foreach (Node node in pathfinder.open)
        {
            Gizmos.DrawSphere(node.position, 0.15f);
        }

        Gizmos.color = Color.cyan;
        foreach (Node node in pathfinder.closed)
        {
            Gizmos.DrawSphere(node.position, 0.15f);
        }

        if (path == null) return;

        Gizmos.color = Color.yellow;
        foreach (Node node in path)
        {
            Gizmos.DrawSphere(node.position, 0.15f);
        }
    }
}
