using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Pathfinding : MonoBehaviour
{
    public NavMeshAgent agent;
    public GameObject target;
    public GameObject pointA, pointB;
    public GameObject currentDestination;
    public bool patrol = true;

    public void Start()
    {
        currentDestination = pointA;
        Patrol();
    }

    public void Chase()
    {
        StopAllCoroutines();
        agent.SetDestination(target.transform.position);
    }

    public void StopChase()
    {
        StopAllCoroutines();
    }

    public void Patrol()
    {
        StopAllCoroutines();
        StartCoroutine(PatrolRoutine());
    }

    private IEnumerator PatrolRoutine()
    {
        while (patrol)
        {
            agent.SetDestination(currentDestination.transform.position);
            while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
            {
                yield return null;
            }
            currentDestination = currentDestination == pointA ? pointB : pointA;
            yield return new WaitForSeconds(1f);
        }
    }
}