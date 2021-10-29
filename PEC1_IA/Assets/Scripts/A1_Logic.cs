using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class A1_Logic : MonoBehaviour
{
    private NavMeshAgent agent;
    private float speed = 3;
    public static float wanderRadius = 20;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if(agent != null)
        {
            agent.speed = speed;
        }
    }

    void Update()
    {
        Vector3 randPos = Random.insideUnitSphere * wanderRadius;

        if (agent != null && agent.remainingDistance <= agent.stoppingDistance)
        {
            agent.destination = randPos;
        }
    }
}
