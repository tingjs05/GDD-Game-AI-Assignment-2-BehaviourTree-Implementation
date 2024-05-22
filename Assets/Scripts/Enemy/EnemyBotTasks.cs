using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;

[RequireComponent(typeof(PandaBehaviour), typeof(EnemyController))] 
public class EnemyBotTasks : MonoBehaviour
{
    // componenets
    PandaBehaviour panda;
    EnemyController enemy;

    // Start is called before the first frame update
    void Start()
    {
        // get componenets
        panda = GetComponent<PandaBehaviour>();
        enemy = GetComponent<EnemyController>();
    }

    // priorities tree
    [Task]
    bool IsDead()
    {
        if (enemy.CurrentHealth <= 0f)
        {
            Destroy(gameObject);
            return true;
        }
        return false;
    }

    [Task]
    bool IsStunned()
    {
        return enemy.Stunned;
    }

    [Task]
    void TrapTriggered()
    {

    }

    // attack tree
    [Task]
    bool IsAlert()
    {
        return false;
    }

    [Task]
    void Prowl()
    {

    }

    [Task]
    void Hide()
    {

    }

    [Task]
    void Attack()
    {

    }

    [Task]
    void Flee()
    {

    }

    // lay trap tree
    [Task]
    void LayTrap()
    {

    }

    // push tree
    [Task]
    void Wait()
    {

    }

    [Task]
    void Push()
    {

    }

    // patrol tree
    [Task]
    void Patrol()
    {

    }

    [Task]
    void Idle()
    {
        return;
    }
}
