using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;

namespace Agent
{
    // patrol tree
    public class PatrolTasks : AgentTasks
    {
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
