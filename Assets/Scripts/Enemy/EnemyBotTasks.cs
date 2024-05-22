using System.Collections;
using System.Collections.Generic;
using Panda;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

[RequireComponent(typeof(NavMeshAgent), typeof(PandaBehaviour))] 
public class EnemyBotTasks : MonoBehaviour
{
    // inspector values
    [Header("Health")]
    [SerializeField] float maxHealth = 3f;
    [SerializeField] Slider healthBar;
    float currentHealth;

    [field: Header("Movement")]
    [field: SerializeField] float WalkSpeed = 3f;
    [field: SerializeField] float RunSpeed = 6f;
    [field: SerializeField] float SneakSpeed = 2f;

    [field: Header("Ranges")]
    [field: SerializeField] float PatrolRadius = 3f;
    [field: SerializeField] float AlertRadius = 3.5f;
    [field: SerializeField] float AttackRange = 1f;
    [field: SerializeField] float FleeDistance = 5f;
    [field: SerializeField] float PlayerInObstacleRange = 3f;

    [field: Header("Durations")]
    [field: SerializeField] float AttackDuration = 0.25f;
    [field: SerializeField] float MaxFleeDuration = 5f;
    [field: SerializeField] float MinFaceEnemyDuration = 0.5f;
    [field: SerializeField] float MaxHideDuration = 5f;
    [field: SerializeField] float MaxWaitDuration = 5f;
    [field: SerializeField] float StunDuration = 3f;
    [field: SerializeField] float PushDuration = 1.5f;
    [field: SerializeField] float LayTrapDuration = 1f;
    [field: SerializeField] Vector2 PushCheckCooldown = new Vector2(3f, 5f);
    [field: SerializeField] Vector2 PlaceTrapCooldown = new Vector2(3f, 5f);

    [Header("Thresholds")]
    [SerializeField, Range(0f, 1f)] float facingEnemyThreshold = 0.8f;
    [field: SerializeField] float MinHideDistanceThreshold = 3f;
    [field: SerializeField, Range(0f, 1f)] float PushTransitionChance = 0.3f;
    [field: SerializeField, Range(0f, 1f)] float LayTrapChance = 0.3f;

    [Header("Prefabs")]
    [SerializeField] GameObject trapPrefab;

    // componenets
    NavMeshAgent agent;
    PandaBehaviour panda;

    // Start is called before the first frame update
    void Start()
    {
        // get componenets
        agent = GetComponent<NavMeshAgent>();
        panda = GetComponent<PandaBehaviour>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
