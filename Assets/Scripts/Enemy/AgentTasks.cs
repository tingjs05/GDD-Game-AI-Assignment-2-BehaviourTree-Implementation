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
        return false;
    }

    [Task]
    void Die()
    {

    }

    // stun
    [Task]
    bool IsStunned()
    {
        return false;
    }

    [Task]
    void Stun()
    {

    }

    // trap triggered
    [Task]
    bool IsTrapTriggered()
    {
        return false;
    }

    [Task]
    bool IsAtTrapLocation()
    {
        return false;
    }

    [Task]
    void MoveToTrap()
    {
        
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
        
    }

    [Task]
    void Hide()
    {
        
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
        
    }
}
