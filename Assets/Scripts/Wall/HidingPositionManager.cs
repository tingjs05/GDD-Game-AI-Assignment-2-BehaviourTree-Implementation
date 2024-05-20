using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HidingPositionManager : MonoBehaviour
{
    [SerializeField] float cornerOffset;
    [SerializeField] Vector2 size;
    [SerializeField] LayerMask obstacleMask;
    [SerializeField] bool showGizmos;
    [SerializeField] Transform[] walls;

    public static HidingPositionManager Instance { get; private set; }
    public List<Vector3> HidingSpots { get; private set; } = new List<Vector3>();
    public List<Vector3> PushingSpots { get; private set; } = new List<Vector3>();

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        obstacleMask = LayerMask.GetMask("Obstacles");
        GetHidingSpots();
    }

    void OnDrawGizmosSelected()
    {
        if (!showGizmos) return;

        GetHidingSpots();

        // show position of hiding spots
        Gizmos.color = Color.green;
        foreach (Vector3 spot in HidingSpots)
        {
            Gizmos.DrawSphere(spot, 0.5f);
        }

        // show position of all pushing spots
        Gizmos.color = Color.yellow;
        foreach (Vector3 spot in PushingSpots)
        {
            Gizmos.DrawSphere(spot, 0.5f);
        }
    }

    void GetHidingSpots()
    {
        // reset hiding spots list
        HidingSpots.Clear();
        // instantiate temp spot positions for calculations
        Vector3 tempSpot1, tempSpot2;

        foreach (Transform wall in walls)
        {
            // corner 1
            CalculateHidingSpotFromCorner(wall.position, true, true, out tempSpot1, out tempSpot2);
            AddSpot(tempSpot1, 0.5f);
            AddSpot(tempSpot2, 0.5f);
            // corner 2
            CalculateHidingSpotFromCorner(wall.position, true, false, out tempSpot1, out tempSpot2);
            AddSpot(tempSpot1, 0.5f);
            AddSpot(tempSpot2, 0.5f);
            // corner 3
            CalculateHidingSpotFromCorner(wall.position, false, true, out tempSpot1, out tempSpot2);
            AddSpot(tempSpot1, 0.5f);
            AddSpot(tempSpot2, 0.5f);
            // corner 4
            CalculateHidingSpotFromCorner(wall.position, false, false, out tempSpot1, out tempSpot2);
            AddSpot(tempSpot1, 0.5f);
            AddSpot(tempSpot2, 0.5f);
        }
    }

    void CalculateHidingSpotFromCorner(Vector3 wallPosition, bool addX, bool addZ, out Vector3 tempSpot1, out Vector3 tempSpot2)
    {
        wallPosition.x = addX? wallPosition.x + (size.x - cornerOffset) : wallPosition.x - (size.x - cornerOffset);
        wallPosition.z = addZ? wallPosition.z + (size.y - cornerOffset) : wallPosition.z - (size.y - cornerOffset);
        tempSpot1 = wallPosition;
        tempSpot2 = wallPosition;
        tempSpot1.x = addX? tempSpot1.x + (cornerOffset * 2) : tempSpot1.x - (cornerOffset * 2);
        tempSpot2.z = addZ? tempSpot1.z + (cornerOffset * 2) : tempSpot1.z - (cornerOffset * 2);
    }

    void AddSpot(Vector3 spot, float spotSize)
    {
        // add position to pushing spot if a pushable object is found within range
        if (Physics.OverlapSphere(spot, spotSize, obstacleMask).Length > 0)
        {
            PushingSpots.Add(spot);
            return;
        }
        // else add it to hiding spots
        HidingSpots.Add(spot);
    }
}
