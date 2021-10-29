using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class A3_WalkAway : A3_Interface
{
    private readonly A3_FSM fsm;
    private float stoppingDistance = 10;
    public static bool isWalkingAway = false;
    //private Vector3 lastKnownPos;

    public A3_WalkAway(A3_FSM a3_FSM)
    {
        fsm = a3_FSM;
    }

    public void UpdateState()
    {
        if (Vector3.Distance(fsm.navMeshAgent.transform.position, fsm.target.transform.position) >= stoppingDistance)
        {
            isWalkingAway = false;

            fsm.navMeshAgent.destination = A3_Persecution.lastKnownPosition;
            float remainingDist = fsm.navMeshAgent.remainingDistance;

            if(A2_FOV.detected == true) 
            {
                fsm.navMeshAgent.isStopped = true;
                fsm.navMeshAgent.ResetPath();
                StatePersecution();
                Debug.Log("DECTECTED BEFORE LAST KNOWN POS");
            }
            else if(remainingDist != Mathf.Infinity && fsm.navMeshAgent.pathStatus == NavMeshPathStatus.PathComplete && fsm.navMeshAgent.remainingDistance == 0)
            {
                Debug.Log("LAST KNOWN POS REACHED");
                Debug.Log(A3_Persecution.lastKnownPosition);
                StateWander();
            }
        }
    }

    public void StateWalkAway()
    {
        Debug.Log("Already in walk away state");
    }

    public void StatePersecution()
    {
        Debug.Log("PERSECUTION STATE");
        fsm.currentState = fsm.a3_Persecution;
    }

    public void StateWander()
    {
        Debug.Log("WANDER STATE");
        fsm.currentState = fsm.a3_Wander;
    }
}
