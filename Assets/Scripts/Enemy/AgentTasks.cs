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
            // set text
            bot.SetText("Dead");
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
            // set text
            bot.SetText("Stunned");

            // if task is mark as completed, task is successful
            if (taskCompleted) 
            {
                // mark task as completed
                ThisTask.Succeed();
                // reset task completed
                taskCompleted = false;
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
            return bot.TrapTriggered;
        }

        [Task]
        void MoveToTrap()
        {
            // set text
            bot.SetText("Move to Trap");
            // set the bot speed to run speed
            bot.Agent.speed = bot.RunSpeed;
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
                // set destination
                bot.Agent.SetDestination(player.position);
                return true;
            }
            return false;
        }

        [Task]
        void Alert()
        {
            // set text
            bot.SetText("Alert");
            // set the bot speed to sneak speed
            bot.Agent.speed = bot.SneakSpeed;
            // move on to prowl if player is seen
            if (bot.PlayerSeen(bot.AlertRadius, out Transform player))
            {
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
            // set text
            bot.SetText("Prowl");
            // set the bot speed to run speed
            bot.Agent.speed = bot.RunSpeed;
            // if task is mark as completed, task is successful
            if (taskCompleted) 
            {
                // mark task as completed
                ThisTask.Succeed();
                // reset task completed
                taskCompleted = false;
                return;
            }
            // reset task completed boolean
            taskCompleted = false;

            // check if player is still seen
            if (!bot.PlayerSeen(bot.AlertRadius, out Transform player)) 
            {
                // reset coroutine before setting task as failed
                bot.ResetCoroutine();
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
            // reset coroutine, if player is no longer moving towards self
            if (!bot.PlayerIsMovingTowardsEnemy(player)) bot.ResetCoroutine();

            // handle player still being seen
            // complete task (stop prowling) if player is moving towards self for set time
            // wait for coroutine if there is already a coroutine running
            if (bot.coroutine != null) return;
            // start new coroutine if there are no coroutines
            bot.coroutine = bot.StartCoroutine(bot.CountDuration(bot.MinFaceEnemyDuration, () => {
                    // go into hiding
                    // set destination if not at hiding position
                    if (bot.GetNearestHidingSpot(out Vector3 hidingSpot)) bot.Agent.SetDestination(hidingSpot);
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
            // set text
            bot.SetText("Attack");

            // if task is mark as completed, task is successful
            if (taskCompleted) 
            {
                // mark task as completed
                ThisTask.Succeed();
                // reset task completed
                taskCompleted = false;
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
                    taskCompleted = true;
                }));
        }

        // hide
        [Task]
        void MoveToHidingLocation()
        {
            // set text
            bot.SetText("Move to Hiding Location");
            // set the bot speed to run speed
            bot.Agent.speed = bot.RunSpeed;
            // when reached hiding location, task is successful
            if (bot.Agent.remainingDistance <= bot.Agent.stoppingDistance)
                ThisTask.Succeed();
        }

        [Task]
        void Hide()
        {
            // set text
            bot.SetText("Hide");

            // if task is mark as completed, task is successful
            if (taskCompleted) 
            {
                // mark task as completed
                ThisTask.Succeed();
                // reset task completed
                taskCompleted = false;
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
                    // so mark task as successful
                    taskCompleted = true;
                }));
        }

        // flee
        [Task]
        void Flee()
        {
            // set text
            bot.SetText("Flee");
            // set the bot speed to run speed
            bot.Agent.speed = bot.RunSpeed;

            // if task is mark as completed, task is successful
            if (taskCompleted) 
            {
                // mark task as completed
                ThisTask.Succeed();
                // reset task completed
                taskCompleted = false;
                return;
            }
            // reset task completed boolean
            taskCompleted = false;

            // start coroutine to ensure dont flee for too long
            if (bot.coroutine == null)
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
                TrappablePositionManager.Instance.IsInCorridor(transform.position))
                    return true;
            return false;
        }

        [Task]
        void LayTrap()
        {
            // set text
            bot.SetText("Lay Trap");

            // if task is mark as completed, task is successful
            if (taskCompleted) 
            {
                // mark task as completed
                ThisTask.Succeed();
                // reset task completed
                taskCompleted = false;
                return;
            }
            // reset task completed boolean
            taskCompleted = false;

            // dont run if coroutine counter is running
            if (bot.coroutine != null) return;
            // do not let agent move when in this state
            bot.Agent.speed = 0f;
            // set can lay trap to false
            bot.CanLayTrap = false;
            // place down trap
            bot.PlaceTrap();
            
            // stay in lay trap for lay trap duration
            bot.coroutine = bot.StartCoroutine(bot.CountDuration(bot.LayTrapDuration, () => {
                    // complete task
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
            // set text
            bot.SetText("Find Nearest Wait Location");

            // task fails if cannot find pushing spot
            if (!bot.GetNearestPushSpot(out Vector3 pushingSpot)) 
            {
                ThisTask.Fail();
                return;
            }
            // set destination if not at pushing position
            bot.Agent.SetDestination(pushingSpot);
            // set task to successful
            ThisTask.Succeed();
        }

        [Task]
        void MoveToWaitLocation()
        {
            // set text
            bot.SetText("Move to Wait Location");
            // set the bot speed to walk speed
            bot.Agent.speed = bot.WalkSpeed;

            // if can see player, fail task and choose another behaviour
            if (bot.PlayerSeen(bot.AlertRadius, out Transform player))
                ThisTask.Fail();
            // when reached waiting location, task is successful
            else if (bot.Agent.remainingDistance <= bot.Agent.stoppingDistance)
                ThisTask.Succeed();
        }

        // push
        [Task]
        void Wait()
        {
            // set text
            bot.SetText("Wait");

            // if task is mark as completed, task is successful
            if (taskCompleted) 
            {
                // mark task as completed
                ThisTask.Fail();
                // reset task completed
                taskCompleted = false;
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
            // set text
            bot.SetText("Push");

            // if task is mark as completed, task is successful
            if (taskCompleted) 
            {
                // mark task as completed
                ThisTask.Succeed();
                // reset task completed
                taskCompleted = false;
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
            // set text
            bot.SetText("Patrol");
            // set the bot speed to walk speed
            bot.Agent.speed = bot.WalkSpeed;
            // set a new destination if reached target location
            if (bot.Agent.remainingDistance <= bot.Agent.stoppingDistance)
            {
                // get a random point to walk to, ensure it is possible to get a position
                if (!bot.RandomPoint(transform.position, bot.PatrolRadius, out Vector3 point)) return;
                // set target position to walk towards
                bot.Agent.SetDestination(point);
            }
            // complete task
            ThisTask.Succeed();
        }
    }
}