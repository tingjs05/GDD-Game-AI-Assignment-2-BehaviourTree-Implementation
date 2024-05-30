using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;

namespace Agent
{
    // attack tree
    public class AttackTasks : AgentTasks
    {
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
    }
}
