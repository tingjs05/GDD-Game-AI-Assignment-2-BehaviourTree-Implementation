using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

[RequireComponent(typeof(NavMeshAgent), typeof(EnemyBotTasks))]
public class EnemyController : MonoBehaviour, IDamagable
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
    [field: SerializeField] public float AttackDuration { get; private set; } = 0.25f;
    [field: SerializeField] public float MaxFleeDuration { get; private set; } = 5f;
    [field: SerializeField] public float MinFaceEnemyDuration { get; private set; } = 0.5f;
    [field: SerializeField] public float MaxHideDuration { get; private set; } = 5f;
    [field: SerializeField] public float MaxWaitDuration { get; private set; } = 5f;
    [field: SerializeField] public float StunDuration { get; private set; } = 3f;
    [field: SerializeField] public float PushDuration { get; private set; } = 1.5f;
    [field: SerializeField] public float LayTrapDuration { get; private set; } = 1f;
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
    public bool Stunned { get; private set; }

    // private variables
    Coroutine coroutine;

    // Start is called before the first frame update
    void Start()
    {
        // get nav agent
        Agent = GetComponent<NavMeshAgent>();

        // set booleans
        Stunned = false;

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
    IEnumerator CountDuration(float duration, bool boolToSet)
    {
        boolToSet = true;
        yield return new WaitForSeconds(duration);
        boolToSet = false;
        coroutine = null;
    }

    // public methods
    public void Stun()
    {
        if (coroutine != null) StopCoroutine(coroutine);
        coroutine = StartCoroutine(CountDuration(StunDuration, Stunned));
    }
}
