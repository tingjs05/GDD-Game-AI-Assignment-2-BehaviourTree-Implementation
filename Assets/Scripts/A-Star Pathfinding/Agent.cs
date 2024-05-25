using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Astar
{
    namespace Pathfinding
    {
        [RequireComponent(typeof(Rigidbody))]
        public class Agent : MonoBehaviour
        {
            public float speed = 1f;
            public float stoppingDistance = 0.5f;
            public float remainingDistance { get; private set; }
            
            // components
            Rigidbody rb;
            // pathfinding component
            Pathfinding pathfinder;

            // Start is called before the first frame update
            void Start()
            {
                // get components
                rb = GetComponent<Rigidbody>();

                // creat a new instance of path finder
                pathfinder = new Pathfinding();
            }

            // Update is called once per frame
            void Update()
            {
                
            }

            void SetDestination(Vector3 destination)
            {
                pathfinder.FindPath(transform.position, destination);
            }
        }
    }
}
