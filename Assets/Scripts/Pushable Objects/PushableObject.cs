using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PushableObject : MonoBehaviour
{
    [SerializeField] float dropAngle = 90f;
    [SerializeField] float dropSpeed = 1f;
    [SerializeField] PushableObjectHitbox hitbox;
    private bool hasBeenDropped = false;

    // Start is called before the first frame update
    void Start()
    {
        hasBeenDropped = false;
    }

    public bool DropObject(out Vector3 pushSpot)
    {
        // reset push spot output to zero
        pushSpot = Vector3.zero;
        if (hasBeenDropped) return false;
        // set can hit boolean
        hasBeenDropped = true;
        if (hitbox != null) hitbox.canHit = hasBeenDropped;

        // return position of pushing spot of pusher to set their position to
        // cannot be completed if hiding position manager is null
        if (HidingPositionManager.Instance == null) return false;
        // get push spot by finding nearest push spot
        // this is achieved through sorting it by distance, and taking the first element
        pushSpot = HidingPositionManager.Instance.PushingSpots
            .OrderBy(x => Vector3.Distance(transform.position, x))
            .ToArray()[0];
        
        // remove push spot, and add to hiding spot
        HidingPositionManager.Instance.PushingSpots.Remove(pushSpot);
        HidingPositionManager.Instance.HidingSpots.Add(pushSpot);

        // start coroutine to slowly drop pillar
        StartCoroutine(Drop());
        // return true if operation is complete
        return true;
    }

    IEnumerator Drop()
    {
        // slowly interpolate between current rotation and target angle
        while (transform.rotation.eulerAngles.x < (dropAngle * 0.9f))
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, 
                Quaternion.Euler(dropAngle, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z), 
                dropSpeed * Time.deltaTime);
            yield return null;
        }
        // destroy game object when sucessfully dropped
        Destroy(gameObject);
    }
}
