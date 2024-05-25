using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Astar
{
    namespace Grid
    {
        public class Node
        {
            // struct to store connections between nodes
            public struct Connection
            {
                public Vector3 direction;
                public float distance;
            }

            // public variables
            public Vector3 position;
            public List<Connection> connections = new List<Connection>();
        }
    }
}
