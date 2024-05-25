using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Astar
{
    namespace Nodes
    {
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

            // set instance when in inspector, ensure only one instance running at once (singleton)
            void OnDrawGizmosSelected()
            {
                if (Instance == this)
                    return;
                else if (Instance == null)
                    Instance = this;
                else if (Instance != this)
                    Destroy(gameObject);
            }
        }
    }
}
