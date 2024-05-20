using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] float movementSpeed = 2f;
    [SerializeField] Vector3 offset;
    [SerializeField] Transform target;

    // Update is called once per frame
    void Update()
    {
        if (target == null) return;
        transform.position = Vector3.Lerp(transform.position, target.position + offset, movementSpeed * Time.deltaTime);
    }

    void OnDrawGizmosSelected() 
    {
        if (target == null) return;
        transform.position = target.position + offset;
    }
}
