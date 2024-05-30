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
        protected PandaBehaviour panda;
        protected AgentController bot;

        // boolean to manage task completion
        protected bool taskCompleted;

        void Awake()
        {
            // get componenets
            panda = GetComponent<PandaBehaviour>();
            bot = GetComponent<AgentController>();

            // set task completed boolean
            taskCompleted = false;
        }
    }
}