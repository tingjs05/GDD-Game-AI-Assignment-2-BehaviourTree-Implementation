using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;

namespace Agent
{
    // priorities tree
    public class PriorityTasks : AgentTasks
    {
        // Start is called before the first frame update
        void Start()
        {
            // subscribe to stun event
            bot.StunEvent += HandleStun;
        }

        // stun event handler
        void HandleStun()
        {
            // when stun, mark current task as completed to immediately enter stun action node
            taskCompleted = true;
        }
        
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
            // log end of game
            Debug.Log("Enemy Died: Mission Sucessful!");
            // destroy enemy
            Destroy(gameObject);
            // complete task
            ThisTask.Succeed();
        }

        // stun
        [Task]
        bool IsStunned()
        {
            if (bot.Stunned)
            {
                // reset task completed boolean when entering stun action node
                taskCompleted = false;
                return true;
            }
            return false;
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
    }
}
