using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;

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

    // attack tree

    // lay trap tree

    // patrol tree
}
