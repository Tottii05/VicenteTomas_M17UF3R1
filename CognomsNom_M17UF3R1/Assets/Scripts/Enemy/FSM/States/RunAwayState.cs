using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "RunAwayState", menuName = "StatesSO/RunAway")]
public class RunAwayState : StateSO
{
    public override void OnStateEnter(EnemyController ec)
    {

    }

    public override void OnStateExit(EnemyController ec)
    {
        ec.gameObject.GetComponent<Pathfinding>().StopRunAway();
    }

    public override void OnStateUpdate(EnemyController ec)
    {
        ec.gameObject.GetComponent<Pathfinding>().RunAway();
    }
}