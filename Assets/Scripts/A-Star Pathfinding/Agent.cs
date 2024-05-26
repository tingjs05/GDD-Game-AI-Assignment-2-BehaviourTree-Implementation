using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Astar.Nodes;

namespace Astar
{
    namespace Pathfinding
    {
        [RequireComponent(typeof(Rigidbody))]
        public class Agent : MonoBehaviour
        {
            public float speed = 1f;
            public float stoppingDistance = 1f;
            public bool showGizmos = true;
            public float remainingDistance { get; private set; } = 0f;

            // variables to control path following
            List<Node> path;
            Vector3 destination;
            int currentWayPoint;
            
            // components
            Rigidbody rb;
            // pathfinding component
            Pathfinding pathfinder;

            // Start is called before the first frame update
            void Start()
            {
                // get components
                rb = GetComponent<Rigidbody>();
                // creat a new instance of path finder module
                pathfinder = new Pathfinding();
            }

            // Update is called once per frame
            void Update()
            {
                // follow path if path is not null and contains nodes
                if (path != null && path.Count > 0) FollowPath();
            }

            void FollowPath()
            {
                // update remaining distance
                remainingDistance = Vector3.Distance(transform.position, destination);
                // add force to move agent in the direction of waypoint
                rb.velocity = (path[currentWayPoint].position - transform.position).normalized * speed;
                rb.velocity += Physics.gravity;
                // check if reached waypoint
                if (Vector3.Distance(transform.position, path[currentWayPoint].position) < stoppingDistance)
                {
                    // iterate current waypoint
                    currentWayPoint += 1;
                    // handle final waypoint
                    if (currentWayPoint < path.Count) return;
                    // reset all path following variables
                    path = null;
                    currentWayPoint = -1;
                    remainingDistance = 0f;
                }
            }

            // public methods
            public void SetDestination(Vector3 _destination)
            {
                // set destination
                destination = _destination;
                // reset current waypoint to first item in path list
                currentWayPoint = 0;
                // get path to destination
                path = pathfinder.FindPath(transform.position, _destination);
                // set remaining distance
                remainingDistance = Vector3.Distance(transform.position, destination);
            }

            public void Warp(Vector3 position)
            {
                // get to nearest node
                Vector3 newPosition = NodeManager.Instance.GetNearestNode(position).position;
                // set only the x and z positions
                transform.position = new Vector3(newPosition.x, transform.position.y, newPosition.z);
            }

            void OnDrawGizmos()
            {
                // do not draw gizmos of show gizmos is false
                if (!showGizmos) return;

                // if pathfinder cannot be found, dont draw gizmos for path finding
                if (pathfinder == null) return;
                
                // draw horizon nodes
                foreach (Node node in pathfinder.open)
                {
                    // if node is part of path, draw it as yellow
                    Gizmos.color = path != null && path.Contains(node)? Color.yellow : Color.blue;
                    // show connection to previous node
                    if (node.previousNode != null) Debug.DrawRay(node.position, node.previousNode.position - node.position, Gizmos.color);
                    Gizmos.DrawSphere(node.position, 0.2f);
                }

                // draw visited nodes
                foreach (Node node in pathfinder.closed)
                {
                    // if node is part of path, draw it as yellow
                    Gizmos.color = path != null && path.Contains(node)? Color.yellow : Color.cyan;
                    // show connection to previous node
                    if (node.previousNode != null) Debug.DrawRay(node.position, node.previousNode.position - node.position, Gizmos.color);
                    Gizmos.DrawSphere(node.position, 0.2f);
                }
            }
        }
    }
}
