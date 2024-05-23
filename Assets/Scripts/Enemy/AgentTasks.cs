using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;

[RequireComponent(typeof(PandaBehaviour), typeof(AgentController))] 
public class AgentTasks : MonoBehaviour
{
    // componenets
    PandaBehaviour panda;
    AgentController bot;

    // Start is called before the first frame update
    void Start()
    {
        // get componenets
        panda = GetComponent<PandaBehaviour>();
        bot = GetComponent<AgentController>();
    }

    // priorities tree
    // death
    [Task]
    bool IsDead()
    {
        // check if health has reached 0
        return bot.CurrentHealth <= 0;
    }

    [Task]
    void Die()
    {
        // log action
        Debug.Log("Died");
        // destroy enemy
        Destroy(gameObject);
    }

    // stun
    [Task]
    bool IsStunned()
    {
        return bot.Stunned;
    }

    [Task]
    void Stun()
    {
        // log action
        Debug.Log("Stunned");
        // do stunned stuff
        // aka play animations
    }

    // trap triggered
    [Task]
    bool IsTrapTriggered()
    {
        return bot.TrapTriggered;
    }

    [Task]
    bool IsAtTrapLocation()
    {
        return bot.Agent.remainingDistance <= bot.Agent.stoppingDistance;
    }

    [Task]
    void MoveToTrap()
    {
        // log action
        Debug.Log("Move to Trap");
        // set the bot speed to run
        // set trap triggered boolean to false when target destination is reached
        bot.Agent.speed = bot.RunSpeed;
        if (bot.Agent.remainingDistance <= bot.Agent.stoppingDistance) bot.TrapTriggered = false;
    }

    // attack tree
    // alert
    [Task]
    bool IsAlerted()
    {
        return false;
    }

    [Task]
    void Alert()
    {
        // log action
        Debug.Log("Alert");
    }

    // prowl
    [Task]
    bool IsPlayerSeen()
    {
        return false;
    }

    [Task]
    void Prowl()
    {
        // log action
        Debug.Log("Prowl");
    }

    // hide
    [Task]
    bool IsPlayerMovingTowardsSelf()
    {
        return false;
    }

    [Task]
    bool IsAtHidingLocation()
    {
        return false;
    }

    [Task]
    void MoveToHidingLocation()
    {
        // log action
        Debug.Log("Move to Hiding Location");
    }

    [Task]
    void Hide()
    {
        // log action
        Debug.Log("Hide");
    }

    // attack
    [Task]
    bool IsWithinAttackRange()
    {
        return false;
    }

    [Task]
    void Attack()
    {
        // log action
        Debug.Log("Attack");
    }

    // flee
    [Task]
    bool HasFledFromPlayer()
    {
        return false;
    }

    [Task]
    void Flee()
    {
        // log action
        Debug.Log("Flee");
    }

    // lay trap tree
    // lay trap
    [Task]
    bool IsAtCorridor()
    {
        return false;
    }

    [Task]
    void LayTrap()
    {
        // log action
        Debug.Log("Lay Trap");
    }

    // push tree
    // wait
    [Task]
    bool IsAtWaitLocation()
    {
        return false;
    }

    [Task]
    void MoveToWaitLocation()
    {
        // log action
        Debug.Log("Move to Wait Location");
    }

    // push
    [Task]
    bool IsWithinPushRange()
    {
        return false;
    }

    [Task]
    void Push()
    {
        // log action
        Debug.Log("Push");
    }

    // patrol tree
    // patrol
    [Task]
    bool HasReachedTargetLocation()
    {
        return false;
    }

    [Task]
    void Patrol()
    {
        // log action
        Debug.Log("Patrol");
    }
}
