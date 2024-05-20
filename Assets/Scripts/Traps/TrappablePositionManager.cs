using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrappablePositionManager : MonoBehaviour
{
    public static TrappablePositionManager Instance { get; private set; }

    [SerializeField] Vector3[] corridorArray;
    [SerializeField] float corridorSize;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
    }

    // returns a boolean if the position is within the corridor area
    public bool IsInCorridor(Vector3 position)
    {
        // loop through all the corridors
        foreach (Vector3 corridor in corridorArray)
        {
            // check if distance is less than corridor size to check it it is within corridor area
            if (Vector3.Distance(corridor, position) <= corridorSize)
            {
                return true;
            }
        }
        return false;
    }


    void OnDrawGizmosSelected() 
    {
        // null check
        if (corridorArray == null || corridorArray.Length <= 0) return;
        // show corridor areas
        Gizmos.color = Color.grey;
        foreach (Vector3 corridor in corridorArray)
        {
            Gizmos.DrawWireSphere(corridor, corridorSize);
        }
    }
}
