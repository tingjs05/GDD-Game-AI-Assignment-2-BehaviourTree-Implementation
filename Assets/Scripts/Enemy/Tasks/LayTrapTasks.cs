using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;

namespace Agent
{
    // lay trap tree
    public class LayTrapTasks : AgentTasks
    {
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
    }
}
