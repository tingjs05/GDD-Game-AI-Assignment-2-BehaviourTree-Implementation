using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;

namespace Agent
{
    // push tree
    public class PushTasks : AgentTasks
    {
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
    }
}
