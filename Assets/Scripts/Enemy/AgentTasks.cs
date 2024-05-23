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

        // push check tree
        // check if currently pushing
        [Task]
        bool IsPushing()
        {
            return bot.Pushing;
        }

        [Task]
        void ContinuePushing()
        {
            // log action
            Debug.Log("Continuing Push");
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
                    // set the bot speed to run speed
                    bot.Agent.speed = bot.RunSpeed;
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
        }

        [Task]
        void Hide()
        {
            // log action
            Debug.Log("Hide");
            // handle hiding, ensure can only hide for set amount of time
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
            // check if can flee and player is within flee range
            if (bot.CanFlee && bot.PlayerNearby(bot.FleeDistance, out Transform player))
            {
                // run away from player by setting destination as vector away from player
                bot.Agent.SetDestination(transform.position +  
                    ((transform.position - player.position).normalized * (1f + bot.Agent.stoppingDistance)));
                
                // set the bot speed to run speed
                bot.Agent.speed = bot.RunSpeed;

                return true;
            }
            // reset flee counter once sucessfully fled
            bot.CanFlee = true;
            if (bot.coroutine != null)
            {
                bot.StopCoroutine(bot.coroutine);
                bot.coroutine = null;
            }
            return false;
        }

        [Task]
        void Flee()
        {
            // log action
            Debug.Log("Flee");
            // handle fleeing, ensure can only flee for set amount of time
            if (bot.CanFlee)
            {
                bot.CanFlee = false;
                bot.coroutine = StartCoroutine(bot.CountDuration(bot.MaxFleeDuration, bot.AfterFlee));
            }
        }

        // lay trap tree
        // lay trap
        [Task]
        bool IsAtCorridor()
        {
            // check if can lay trap
            if (!bot.CanLayTrap) return false;
            // randomly has a chance to transition to lay trapp state, if in a corridor
            if (TrappablePositionManager.Instance != null && 
                TrappablePositionManager.Instance.IsInCorridor(transform.position) &&
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
            // set can lay trap to false
            bot.CanLayTrap = false;
        }

        // push tree
        // wait
        [Task]
        bool IsAtWaitLocation()
        {
            // check if can push
            if (!bot.CanPush) return false;

            if (!bot.Waiting && !bot.Pushing)
            {
                if (bot.GetNearestPushSpot(out Vector3 pushingSpot))
                {
                    // set destination if not at pushing position
                    bot.Agent.SetDestination(pushingSpot);
                    // set the bot speed to walk speed
                    bot.Agent.speed = bot.WalkSpeed;
                }
            }
            else if (bot.Agent.remainingDistance <= bot.Agent.stoppingDistance)
            {
                // when at hiding pushing, start pushing
                bot.Waiting = true;
                return true;
            }

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
            // handle waiting, ensure can only wait for set amount of time
            if (bot.CanWait)
            {
                bot.CanWait = false;
                bot.coroutine = StartCoroutine(bot.CountDuration(bot.MaxHideDuration, bot.AfterHide));
            }
            return false;
        }

        [Task]
        void Push()
        {
            // log action
            Debug.Log("Push");
            // get reference to pushable object
            Collider[] hit = Physics.OverlapSphere(transform.position, bot.Agent.stoppingDistance, LayerMask.GetMask("Obstacles"));
            // try to get pushable object script
            PushableObject pushableObject = null;
            if (hit.Length > 0) pushableObject = hit[0].transform.parent.GetComponent<PushableObject>();
            // switch back to patrol state if failed to get reference to obstacle or pushable object
            // drop object, ensure drop was successful, if not return to patrol state as well
            if (hit.Length <= 0 || pushableObject == null || !pushableObject.DropObject(out Vector3 pushSpot))
            {
                // couldnt push object
                return;
            }
            // push object
            // cont allow push
            bot.CanPush = false;
            // dont allow stun when pushing
            bot.CanStun = false;
            // enter pushing
            bot.Pushing = true;
            // count push duration
            bot.coroutine = StartCoroutine(bot.CountDuration(bot.PushDuration, bot.AfterPush));
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
            if (!bot.RandomPoint(transform.position, bot.PatrolRadius, out Vector3 point)) return;
            // set target position to walk towards
            bot.Agent.SetDestination(point);
            // set the bot speed to walk speed
            bot.Agent.speed = bot.WalkSpeed;
        }
    }
}