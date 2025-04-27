using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "ChaseState", menuName = "StatesSO/Chase")]
public class ChaseState : StateSO
{
    public override void OnStateEnter(EnemyController ec)
    {
    }

    public override void OnStateExit(EnemyController ec)
    {
        ec.gameObject.GetComponent<Pathfinding>().StopChase();
    }

    public override void OnStateUpdate(EnemyController ec)
    {
        ec.gameObject.GetComponent<Pathfinding>().Chase();
    }
}