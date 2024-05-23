using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

namespace Agent
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class AgentController : MonoBehaviour, IDamagable
    {
        // inspector values
        [Header("Health")]
        [SerializeField] private float maxHealth = 3f;
        [SerializeField] private Slider healthBar;
        public float CurrentHealth { get; private set; }

        [field: Header("Movement")]
        [field: SerializeField] public float WalkSpeed { get; private set; } = 3f;
        [field: SerializeField] public float RunSpeed { get; private set; } = 6f;
        [field: SerializeField] public float SneakSpeed { get; private set; } = 2f;

        [field: Header("Ranges")]
        [field: SerializeField] public float PatrolRadius { get; private set; } = 3f;
        [field: SerializeField] public float AlertRadius { get; private set; } = 3.5f;
        [field: SerializeField] public float AttackRange { get; private set; } = 1f;
        [field: SerializeField] public float FleeDistance { get; private set; } = 5f;
        [field: SerializeField] public float PlayerInObstacleRange { get; private set; } = 3f;

        [field: Header("Durations")]
        [field: SerializeField] public float MaxFleeDuration { get; private set; } = 5f;
        [field: SerializeField] public float MinFaceEnemyDuration { get; private set; } = 0.5f;
        [field: SerializeField] public float MaxHideDuration { get; private set; } = 5f;
        [field: SerializeField] public float MaxWaitDuration { get; private set; } = 5f;
        [field: SerializeField] public float StunDuration { get; private set; } = 3f;
        [field: SerializeField] public float PushDuration { get; private set; } = 1.5f;
        [field: SerializeField] public Vector2 PushCheckCooldown { get; private set; } = new Vector2(3f, 5f);
        [field: SerializeField] public Vector2 PlaceTrapCooldown { get; private set; } = new Vector2(3f, 5f);

        [Header("Thresholds")]
        [SerializeField, Range(0f, 1f)] private float facingEnemyThreshold = 0.8f;
        [field: SerializeField] public float MinHideDistanceThreshold { get; private set; } = 3f;
        [field: SerializeField, Range(0f, 1f)] public float PushTransitionChance { get; private set; } = 0.3f;
        [field: SerializeField, Range(0f, 1f)] public float LayTrapChance { get; private set; } = 0.3f;

        [Header("Prefabs")]
        [SerializeField] public GameObject trapPrefab;

        // movement AI
        public NavMeshAgent Agent { get; private set; }

        // public booleans
        [HideInInspector]
        public bool Stunned, TrapTriggered, Hiding; 
        [HideInInspector]
        public bool CanHide, CanWait, CanStun;

        // coroutine manager
        [HideInInspector] public Coroutine coroutine;

        // Start is called before the first frame update
        void Start()
        {
            // get nav agent
            Agent = GetComponent<NavMeshAgent>();

            // set booleans
            Stunned = false;
            TrapTriggered = false;
            Hiding = false;

            CanHide = true;
            CanWait = true;
            CanStun = true;

            // set health
            CurrentHealth = maxHealth;
            if (healthBar != null)
            {
                healthBar.maxValue = maxHealth;
                healthBar.value = maxHealth;
                healthBar.gameObject.SetActive(false);
            }
        }

        // interface methods
        public void Damage(float damage)
        {
            // change health
            CurrentHealth -= damage;
            // update health bar
            if (healthBar != null)
            { 
                if (!healthBar.gameObject.activeInHierarchy) healthBar.gameObject.SetActive(true);
                healthBar.value = CurrentHealth;
            }
        }

        // coroutines
        public IEnumerator CountDuration(float duration, System.Action callback = null)
        {
            yield return new WaitForSeconds(duration);
            callback?.Invoke();
            coroutine = null;
        }

        // handle stun
        public void Stun()
        {
            if (!CanStun) return;
            if (coroutine != null && !Stunned) StopCoroutine(coroutine);
            Stunned = true;
            coroutine = StartCoroutine(CountDuration(StunDuration, AfterStun));
        }

        void AfterStun()
        {
            Stunned = false;
        }

        // trap mechanic
        // handle trap being triggered
        void TrapHasBeenTriggered(Trap trap)
        {
            // set trap triggered to true
            TrapTriggered = true;
            // set agent destination to trap destination
            Agent.SetDestination(trap.transform.position);
            // unsubscribe from event
            trap.TrapTriggered -= TrapHasBeenTriggered;
            // destroy trap
            Destroy(trap.gameObject);
        }

        // place down a trap at current location
        public void PlaceTrap()
        {
            if (trapPrefab == null || TrappablePositionManager.Instance == null) return;
            // instantiate trap prefab
            GameObject obj = Instantiate(
                    trapPrefab, 
                    new Vector3(transform.position.x, 0f, transform.position.z), 
                    Quaternion.identity, 
                    TrappablePositionManager.Instance.transform
                );
            // subscribe to event to listen for when trap is triggered
            obj.GetComponent<Trap>().TrapTriggered += TrapHasBeenTriggered;
        }

        // hide/push mechanic
        // get nearest hiding spot
        public bool GetNearestHidingSpot(out Vector3 hidingSpot)
        {
            // set hiding spots list
            List<Vector3> hidingSpotsOrdered= new List<Vector3>(HidingPositionManager.Instance.HidingSpots);
            // set hiding spot
            hidingSpot = Vector3.zero;
            // do not hide if there are no hiding spots
            if (hidingSpotsOrdered.Count <= 0) return false;
            // sort hiding spots by distance from self
            hidingSpotsOrdered = hidingSpotsOrdered
                .OrderBy(x =>  Vector3.Distance(transform.position, x))
                .Where(x => Vector3.Distance(transform.position, x) >= MinHideDistanceThreshold)
                .ToList();
            // return values
            hidingSpot = hidingSpotsOrdered[0];
            return true;
        }

        public void AfterHide()
        {
            CanHide = false;
            Hiding = false;
        }

        // check for player
        // check if player is nearby within a certain range around the enemy
        public bool PlayerNearby(float range, out Transform player)
        {
            // use sphere cast all, check all nearby objects
            Collider[] hits = Physics.OverlapSphere(transform.position, range);
            // check if anything is hit
            if (hits.Length > 0)
            {
                // loop through all hit objects and check if player is hit
                foreach (Collider hit in hits)
                {
                    if (hit.CompareTag("Player"))
                    {
                        player = hit.transform;
                        return true;
                    }
                }
            }
            player = null;
            return false;
        }

        // check if player is within range and can be seen
        public bool PlayerSeen(float range, out Transform player)
        {
            // use sphere cast all, check all nearby objects
            Collider[] hits = Physics.OverlapSphere(transform.position, range);
            // check if anything is hit
            if (hits.Length > 0)
            {
                // loop through all hit objects and check if player is hit
                foreach (Collider hit in hits)
                {
                    // check if there is a player, and player can be seen
                    if (hit.CompareTag("Player") && 
                        Physics.Raycast(transform.position, (hit.transform.position - transform.position).normalized, 
                            out RaycastHit _hit, Vector3.Distance(hit.transform.position, transform.position)))
                    {
                        player = _hit.transform;
                        return true;
                    }
                }
            }
            player = null;
            return false;
        }

        // check if the player is moving towards the enemy
        public bool PlayerIsMovingTowardsEnemy(Transform player)
        {
            // get direction player would have to move in to go towards enemy
            Vector3 dirFromPlayer = (transform.position - player.position).normalized;
            // get player controller, return false if not found
            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController == null) return false;
            // return false if the player is not moving
            if (playerController.MoveDir == Vector3.zero) return false;
            // use dot product to see how close the player is moving towards the enemy
            return Mathf.Abs(Vector3.Dot(playerController.MoveDir, dirFromPlayer) * dirFromPlayer.magnitude) >= facingEnemyThreshold;
        }

        // get a random point around a center point (usually self)
        public bool RandomPoint(Vector3 center, float range, out Vector3 result)
        {
            // get a random point in a sphere
            Vector3 randomPoint = center + Random.insideUnitSphere * range;
            // get the position on the random point on the navmesh
            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 1f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
            result = Vector3.zero;
            return false;
        }

        // gizmos
        void OnDrawGizmosSelected()
        {
            // show patrol radius
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, PatrolRadius);
            // show alert radius
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, AlertRadius);
            // show attack range
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, AttackRange);
            // show flee distance
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, FleeDistance);
        }
    }
}
