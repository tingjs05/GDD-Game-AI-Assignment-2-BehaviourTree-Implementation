using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Astar
{
    namespace Nodes
    {
        public class NodeManager : MonoBehaviour
        {
            public static NodeManager Instance;
            public List<Node> nodes = new List<Node>();

            void Awake()
            {
                if (Instance == null)
                    Instance = this;
                else if (Instance != this)
                    Destroy(gameObject);
            }

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
