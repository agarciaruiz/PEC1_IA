using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class A3_Persecution : A3_Interface
{
    private readonly A3_FSM fsm;

    private float stoppingDistance = 2;
    public static Vector3 lastKnownPosition;

    public A3_Persecution(A3_FSM a3_FSM)
    {
        fsm = a3_FSM;
    }

    public void UpdateState()
    {
        fsm.navMeshAgent.transform.LookAt(fsm.target.transform);
        fsm.navMeshAgent.destination = fsm.target.transform.position;

        if(Vector3.Distance(fsm.navMeshAgent.transform.position, fsm.target.transform.position) <= stoppingDistance)
        {
            fsm.navMeshAgent.destination = -fsm.target.transform.position * stoppingDistance;
            lastKnownPosition = fsm.target.transform.position;
            Debug.Log(lastKnownPosition);
            A3_WalkAway.isWalkingAway = true;
            A2_FOV.detected = false;
            StateWalkAway();
        }
        else if(A2_FOV.detected == false)
        {
            StateWander();
        }
    }

    public void StatePersecution()
    {
        Debug.Log("Already in persecution state");
    }

    public void StateWander()
    {
        Debug.Log("WANDER STATE");
        fsm.currentState = fsm.a3_Wander;
    }

    public void StateWalkAway()
    {
        Debug.Log("WALK AWAY STATE");
        fsm.currentState = fsm.a3_WalkAway;
    }
}
