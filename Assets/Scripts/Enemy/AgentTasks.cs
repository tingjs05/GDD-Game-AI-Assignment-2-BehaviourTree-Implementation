using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;

namespace Agent
{
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
            // set the bot speed to run speed
            // set trap triggered boolean to false when target destination is reached
            bot.Agent.speed = bot.RunSpeed;
            if (bot.Agent.remainingDistance <= bot.Agent.stoppingDistance) bot.TrapTriggered = false;
        }

        // attack tree
        // alert
        [Task]
        bool IsAlerted()
        {
            // check if player is within alert range
            if (bot.PlayerNearby(bot.AlertRadius, out Transform player))
            {
                // set the bot speed to sneak speed
                bot.Agent.speed = bot.SneakSpeed;
                // set destination
                bot.Agent.SetDestination(player.position);
                // reset hiding if needed
                if (bot.Hiding)
                {
                    bot.Hiding = false;
                    bot.CanHide = true;
                    if (bot.coroutine != null) 
                    {
                        bot.StopCoroutine(bot.coroutine);
                        bot.coroutine = null;
                    }
                }
                return true;
            }
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
            // check if player is seen
            if (bot.PlayerSeen(bot.AlertRadius, out Transform player))
            {
                // set the bot speed to run speed
                bot.Agent.speed = bot.RunSpeed;
                // set destination
                bot.Agent.SetDestination(player.position);
                return true;
            }
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
            // check if can hide, and player is moving towards self
            if (bot.CanHide && bot.PlayerNearby(bot.AlertRadius, out Transform player))
            {
                return bot.PlayerIsMovingTowardsEnemy(player);
            }
            return false;
        }

        [Task]
        bool IsAtHidingLocation()
        {
            if (!bot.Hiding)
            {
                if (bot.GetNearestHidingSpot(out Vector3 hidingSpot))
                {
                    // set destination if not at hiding position
                    bot.Agent.SetDestination(hidingSpot);
                }
            }
            else if (bot.Agent.remainingDistance <= bot.Agent.stoppingDistance)
            {
                // when at hiding position, start hiding
                bot.Hiding = true;
                return true;
            }

            return false;
        }

        [Task]
        void MoveToHidingLocation()
        {
            // log action
            Debug.Log("Move to Hiding Location");
            // set the bot speed to run speed
            bot.Agent.speed = bot.RunSpeed;
        }

        [Task]
        void Hide()
        {
            // log action
            Debug.Log("Hide");
            // handle hiding
            if (bot.CanHide)
            {
                bot.CanHide = false;
                bot.coroutine = StartCoroutine(bot.CountDuration(bot.MaxHideDuration, bot.AfterHide));
            }
        }

        // attack
        [Task]
        bool IsWithinAttackRange()
        {
            return bot.PlayerNearby(bot.AttackRange, out Transform player);
        }

        [Task]
        void Attack()
        {
            // log action
            Debug.Log("Attack");
            // attack player
            if (bot.PlayerNearby(bot.AttackRange, out Transform player))
            {
                // damage player
                player.GetComponent<IDamagable>()?.Damage(1f);
            }
        }

        // flee
        [Task]
        bool IsHiding()
        {
            return bot.Hiding;
        }

        [Task]
        bool HasNotFledFromPlayer()
        {
            // check if player is within flee range
            if (bot.PlayerNearby(bot.FleeDistance, out Transform player))
            {

            }
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
            // randomly has a chance to transition to lay trapp state, if in a corridor
            if (TrappablePositionManager.Instance != null && 
                TrappablePositionManager.Instance.IsInCorridor(bot.transform.position) &&
                Random.Range(0f, 1f) < bot.LayTrapChance)
                    return true;
            return false;
        }

        [Task]
        void LayTrap()
        {
            // log action
            Debug.Log("Lay Trap");
             // do not let agent move when in this state
            bot.Agent.speed = 0f;
            // place down trap
            bot.PlaceTrap();
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
            return bot.Agent.remainingDistance <= bot.Agent.stoppingDistance;
        }

        [Task]
        void Patrol()
        {
            // log action
            Debug.Log("Patrol");
            // get a random point to walk to, ensure it is possible to get a position
            if (!bot.RandomPoint(bot.transform.position, bot.PatrolRadius, out Vector3 point)) return;
            // set target position to walk towards
            bot.Agent.SetDestination(point);
        }
    }
}