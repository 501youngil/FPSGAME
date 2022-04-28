using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveAgent : MonoBehaviour
{
    public List<Transform> wayPoints;
    public int nextIdx;

    private NavMeshAgent agent;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = false;

        var group = GameObject.Find("WayPointGroup");
        if(group != null)
        {
            group.GetComponentsInChildren<Transform>(wayPoints);
            wayPoints.RemoveAt(0);
        }
        MoveWayPoint();
    }

    void MoveWayPoint()
    {
        if (agent.isPathStale) return;

        agent.destination = wayPoints[nextIdx].position;
        agent.isStopped = false;
    }

    private void Update()
    {
        if(agent.velocity.sqrMagnitude >= 0.15f * 0.15f && agent.remainingDistance <= 0.5f)
        {
            nextIdx = ++nextIdx % wayPoints.Count;
            MoveWayPoint();
        }
    }
}
