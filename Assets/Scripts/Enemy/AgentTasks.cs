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

        // public boolean to manage task completion
        [HideInInspector] public bool taskCompleted;

        // Start is called before the first frame update
        void Start()
        {
            // get componenets
            panda = GetComponent<PandaBehaviour>();
            bot = GetComponent<AgentController>();

            // set task completed boolean
            taskCompleted = false;
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
            // destroy enemy
            Destroy(gameObject);
            // complete task
            ThisTask.Succeed();
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
            // if task is mark as completed, task is successful
            if (taskCompleted) 
            {
                ThisTask.Succeed();
                return;
            }
            // reset task completed boolean
            taskCompleted = false;

            // dont run if coroutine counter is running
            if (bot.coroutine != null) return;
            // dont allow double stun
            bot.CanStun = false;
            // wait for stun duration, after that, set stun to false and complete task
            bot.coroutine = bot.StartCoroutine(bot.CountDuration(bot.StunDuration, () => {
                    bot.Stunned = false;
                    bot.CanStun = true;
                    taskCompleted = true;
                }));
        }

        // trap triggered
        [Task]
        bool IsTrapTriggered()
        {
            if (bot.TrapTriggered)
            {
                // set the bot speed to run speed to run towards trap
                bot.Agent.speed = bot.RunSpeed;
                return true;
            }
            return false;
        }

        [Task]
        void MoveToTrap()
        {
            // set trap triggered boolean to false when target destination is reached
            if (bot.Agent.remainingDistance <= bot.Agent.stoppingDistance) 
            {
                bot.TrapTriggered = false;
                // task is successful
                ThisTask.Succeed();
            }
        }

        // attack tree
        // alert
        [Task]
        bool IsAlerted()
        {
            // check if player is within alert range
            if (bot.PlayerNearby(bot.AlertRadius, out Transform player))
            {
                // set the bot speed to sneak speed to prepare for alert
                bot.Agent.speed = bot.SneakSpeed;
                // set destination
                bot.Agent.SetDestination(player.position);
                return true;
            }
            return false;
        }

        [Task]
        void Alert()
        {
            // move on to prowl if player is seen
            if (bot.PlayerSeen(bot.AlertRadius, out Transform player))
            {
                // set the bot speed to run speed to prepare for prowl
                bot.Agent.speed = bot.RunSpeed;
                ThisTask.Succeed();
            }
            // fail sequence if player is not within alert range
            else if (Vector3.Distance(transform.position, player.position) > bot.AlertRadius)
            {
                ThisTask.Fail();
            }
        }

        // prowl
        [Task]
        void Prowl()
        {
            // if task is mark as completed, task is successful
            if (taskCompleted) 
            {
                ThisTask.Succeed();
                return;
            }
            // reset task completed boolean
            taskCompleted = false;

            // check if player is still seen
            if (!bot.PlayerSeen(bot.AlertRadius, out Transform player)) 
            {
                // if player is no longer seen, task failed
                ThisTask.Fail();
                return;
            }

            // if player is within attack range, prowl is successful
            if (bot.PlayerNearby(bot.AttackRange, out Transform _player))
            {
                // reset coroutine before setting task as successful
                bot.ResetCoroutine();
                ThisTask.Succeed();
                return;
            }
            
            // if player is not walking to self anymore, reset coroutine
            if (!bot.PlayerIsMovingTowardsEnemy(player))
            {
                // reset coroutine, if player is no longer moving towards self
                bot.ResetCoroutine();
            }

            // handle player still being seen
            // complete task (stop prowling) if player is moving towards self for set time
            // wait for coroutine if there is already a coroutine running
            if (bot.coroutine != null) return;
            // start new coroutine if there are no coroutines
            bot.coroutine = bot.StartCoroutine(bot.CountDuration(bot.MinFaceEnemyDuration, () => {
                    // go into hiding
                    if (bot.GetNearestHidingSpot(out Vector3 hidingSpot))
                    {
                        // set destination if not at hiding position
                        bot.Agent.SetDestination(hidingSpot);
                        // set the bot speed to run speed to run to hiding spot
                        bot.Agent.speed = bot.RunSpeed;
                    }
                    // task is successful (exit task)
                    taskCompleted = true;
                }));
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
            // if task is mark as completed, task is successful
            if (taskCompleted) 
            {
                ThisTask.Succeed();
                return;
            }
            // reset task completed boolean
            taskCompleted = false;

            // dont run if coroutine counter is running
            if (bot.coroutine != null) return;
            // attack player
            if (bot.PlayerNearby(bot.AttackRange, out Transform player))
            {
                // damage player
                player.GetComponent<IDamagable>()?.Damage(1f);
            }
            // stay in attack for attack duration
            bot.coroutine = bot.StartCoroutine(bot.CountDuration(bot.AttackDuration, () => {
                    // set the bot speed to run speed to prepare to flee after attack
                    bot.Agent.speed = bot.RunSpeed;
                    taskCompleted = true;
                }));
        }

        // hide
        [Task]
        void MoveToHidingLocation()
        {
            // when reached hiding location, task is successful
            if (bot.Agent.remainingDistance <= bot.Agent.stoppingDistance)
                ThisTask.Succeed();
        }

        [Task]
        void Hide()
        {
            // if task is mark as completed, task is successful
            if (taskCompleted) 
            {
                ThisTask.Succeed();
                return;
            }
            // reset task completed boolean
            taskCompleted = false;

            // ensure coroutine counter to count max hide duration has started
            if (bot.coroutine != null)
            {
                // if can see player, means they found us, so hiding has failed
                if (bot.PlayerSeen(bot.AlertRadius, out Transform player))
                {
                    // reset coroutine
                    bot.ResetCoroutine();
                    // fail task
                    ThisTask.Fail();
                }
                return;
            }
            // start coroutine to count max hide duration
            // handle hiding, ensure can only hide for set amount of time
            bot.coroutine = bot.StartCoroutine(bot.CountDuration(bot.MaxHideDuration, () => {
                    // after hiding for max hide duration, flee
                    // set the bot speed to run speed to prepare to flee
                    bot.Agent.speed = bot.RunSpeed;
                    // so mark task as successful
                    taskCompleted = true;
                }));
        }

        // flee
        [Task]
        bool HasFled()
        {
            return !bot.PlayerNearby(bot.FleeDistance, out Transform player);
        }

        [Task]
        void Flee()
        {
            // if task is mark as completed, task is successful
            if (taskCompleted) 
            {
                ThisTask.Succeed();
                return;
            }
            // reset task completed boolean
            taskCompleted = false;

            // start coroutine to ensure dont flee for too long
            if (bot.coroutine != null)
            {
                bot.coroutine = bot.StartCoroutine(bot.CountDuration(bot.MaxFleeDuration, () => {
                        // successfully fled after max flee duration
                        taskCompleted = true;
                    }));
            }
            // flee from player if player still within flee distance
            if (bot.PlayerNearby(bot.FleeDistance, out Transform player))
            {
                // run away from player by setting destination as vector away from player
                bot.Agent.SetDestination(transform.position +  
                    ((transform.position - player.position).normalized * (1f + bot.Agent.stoppingDistance)));
                return;
            }
            // handle successfully fleeing from player
            // stop coroutine counter
            bot.ResetCoroutine();
            // once successfully fled, task is successful
            ThisTask.Succeed();
        }

        // lay trap tree
        // lay trap
        [Task]
        bool CanLayTrap()
        {
            return bot.CanLayTrap;
        }

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
            // if task is mark as completed, task is successful
            if (taskCompleted) 
            {
                ThisTask.Succeed();
                return;
            }
            // reset task completed boolean
            taskCompleted = false;

            // dont run if coroutine counter is running
            if (bot.coroutine != null) return;
            // do not let agent move when in this state
            bot.Agent.speed = 0f;
            // place down trap
            bot.PlaceTrap();
            // set can lay trap to false
            bot.CanLayTrap = false;
            // stay in lay trap for lay trap duration
            bot.coroutine = bot.StartCoroutine(bot.CountDuration(bot.LayTrapDuration, () => {
                    taskCompleted = true;
                }));
        }

        // push tree
        [Task]
        bool CanPush()
        {
            return bot.CanPush;
        }

        // wait
        [Task]
        void FindNearestWaitLocation()
        {
            // set destination to nearest push spot
            if (bot.GetNearestPushSpot(out Vector3 pushingSpot))
            {
                // set destination if not at pushing position
                bot.Agent.SetDestination(pushingSpot);
                // set the bot speed to walk speed
                bot.Agent.speed = bot.WalkSpeed;
                // set task to successful
                ThisTask.Succeed();
            }
        }

        [Task]
        void MoveToWaitLocation()
        {
            // when reached waiting location, task is successful
            if (bot.Agent.remainingDistance <= bot.Agent.stoppingDistance)
                ThisTask.Succeed();
        }

        // push
        [Task]
        void Wait()
        {
            // if task is mark as completed, task is successful
            if (taskCompleted) 
            {
                ThisTask.Fail();
                return;
            }
            // reset task completed boolean
            taskCompleted = false;

            // start a coroutine to only wait for set amount if time before giving up
            if (bot.coroutine == null)
            {
                // after max wait duration, give up
                bot.coroutine = bot.StartCoroutine(bot.CountDuration(bot.MaxWaitDuration, () => {
                        // set can push to false, take it as a push attempt
                        bot.CanPush = false;
                        taskCompleted = true;
                    }));
            }

            // check for task successful result
            // get reference to pushable object
            Collider[] hit = Physics.OverlapSphere(transform.position, bot.Agent.stoppingDistance, LayerMask.GetMask("Obstacles"));
            // check if anything is detected
            if (hit.Length > 0)
            {
                Vector3 frontOfObstacle = hit[0].transform.forward;
                // check for players within range, if there are, transition to push state
                hit = Physics.OverlapSphere(transform.position + frontOfObstacle, bot.PlayerInObstacleRange);
                // check if the player is hit, if so, switch to push state
                foreach (Collider obj in hit)
                {
                    if (obj.CompareTag("Player"))
                    {
                        // once player is within range, task is successful
                        bot.ResetCoroutine();
                        ThisTask.Succeed();
                        return;
                    }
                }
            }
        }

        [Task]
        void Push()
        {
            // if task is mark as completed, task is successful
            if (taskCompleted) 
            {
                ThisTask.Succeed();
                return;
            }
            // reset task completed boolean
            taskCompleted = false;

            // dont run if coroutine counter is running
            if (bot.coroutine != null) return;
            // dont allow stun when pushing
            bot.CanStun = false;
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
                ThisTask.Fail();
                return;
            }
            // after pushing object
            // move agent to push spot
            bot.Agent.Warp(pushSpot);
            // dont allow push
            bot.CanPush = false;
            // count push duration
            bot.coroutine = StartCoroutine(bot.CountDuration(bot.PushDuration, () => {
                // allow stun once task is successful
                bot.CanStun = true;
                // once push is over, task is successful
                taskCompleted = true;
            }));
        }

        // patrol tree
        // patrol
        [Task]
        void Patrol()
        {
            // set a new destination if reached target location
            if (bot.Agent.remainingDistance <= bot.Agent.stoppingDistance)
            {
                // get a random point to walk to, ensure it is possible to get a position
                if (!bot.RandomPoint(transform.position, bot.PatrolRadius, out Vector3 point)) return;
                // set target position to walk towards
                bot.Agent.SetDestination(point);
                // set the bot speed to walk speed
                bot.Agent.speed = bot.WalkSpeed;
            }
            // complete task
            ThisTask.Succeed();
        }
    }
}