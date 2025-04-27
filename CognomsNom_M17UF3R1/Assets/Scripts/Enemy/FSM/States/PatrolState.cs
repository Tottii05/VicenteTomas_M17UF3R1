using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "PatrolState", menuName = "StatesSO/Patrol")]
public class PatrolState : StateSO
{
    public override void OnStateEnter(EnemyController ec)
    {
        ec.gameObject.GetComponent<Pathfinding>().currentDestination = GameObject.FindGameObjectWithTag("PointA");
    }

    public override void OnStateExit(EnemyController ec)
    {

    }

    public override void OnStateUpdate(EnemyController ec)
    {
        ec.gameObject.GetComponent<Pathfinding>().Patrol();
    }
}