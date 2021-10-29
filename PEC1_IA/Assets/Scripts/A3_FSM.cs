using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class A3_FSM : MonoBehaviour
{
    [HideInInspector] public A3_Interface currentState;
    [HideInInspector] public A3_Wander a3_Wander;
    [HideInInspector] public A3_Persecution a3_Persecution;
    [HideInInspector] public A3_WalkAway a3_WalkAway;

    [HideInInspector] public NavMeshAgent navMeshAgent;
    [HideInInspector] public NavMeshAgent target;

    [HideInInspector] public float speed = 3;

    private void Awake()
    {
        a3_Wander = new A3_Wander(this);
        a3_Persecution = new A3_Persecution(this);
        a3_WalkAway = new A3_WalkAway(this);

        navMeshAgent = GetComponent<NavMeshAgent>();
        target = GameObject.Find("Agent").GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        Debug.Log("WANDER STATE");
        currentState = a3_Wander;

        if (navMeshAgent != null)
        {
            navMeshAgent.speed = speed;
        }
    }

    private void Update()
    {
        currentState.UpdateState();
    }
}
