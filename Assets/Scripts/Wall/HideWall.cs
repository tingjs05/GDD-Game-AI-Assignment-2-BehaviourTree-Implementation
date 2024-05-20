using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideWall : MonoBehaviour
{
    [SerializeField] Material original;
    [SerializeField] Material transparent;
    [SerializeField] Vector2 size;
    [SerializeField] float maxZDetectionDistance;
    [SerializeField] Transform player;
    [SerializeField] bool showGizmos;
    [SerializeField] Renderer[] objects;

    // Start is called before the first frame update
    void Start()
    {
        foreach (Renderer obj in objects)
        {
            if (obj == null) return;
            obj.material = original;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null) return;

        foreach (Renderer obj in objects)
        {
            if (obj == null) return;
            obj.material = (player.position.z >= (obj.transform.position.z + size.y) && 
                player.position.x >= (obj.transform.position.x - size.x) && 
                player.position.x <= (obj.transform.position.x + size.x) && 
                (player.position.z - obj.transform.position.z) <= maxZDetectionDistance)? 
                transparent : original;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (!showGizmos) return;

        Gizmos.color = Color.blue;

        foreach (Renderer obj in objects)
        {
            if (obj == null) return;
            Gizmos.DrawSphere(obj.transform.position + new Vector3(0, 0, size.y), 0.5f);
        }

        Gizmos.color = Color.red;

        foreach (Renderer obj in objects)
        {
            if (obj == null) return;
            Gizmos.DrawSphere(obj.transform.position + new Vector3(size.x, 0, 0), 0.5f);
        }
    }
}
