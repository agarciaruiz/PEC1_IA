using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class A3_Wander : A3_Interface
{
    private readonly A3_FSM fsm;

    public A3_Wander(A3_FSM a3_FSM)
    {
        fsm = a3_FSM;
    }

    public void UpdateState()
    {
        Vector3 randPos = Random.insideUnitSphere * A1_Logic.wanderRadius;

        if (fsm.navMeshAgent != null && fsm.navMeshAgent.remainingDistance <= fsm.navMeshAgent.stoppingDistance)
        {
            fsm.navMeshAgent.destination = randPos;
        }

        if (A2_FOV.detected == true)
        {
            StatePersecution();
        }
    }

    public void StateWander()
    {
        Debug.Log("Already in wander state"); 
    }

    public void StatePersecution()
    {
        Debug.Log("PERSECUTION STATE");
        fsm.currentState = fsm.a3_Persecution;
    }

    public void StateWalkAway()
    {
        Debug.Log("WALK AWAY STATE");
        fsm.currentState = fsm.a3_WalkAway;
    }
}
