using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
[CreateAssetMenu(fileName = "DieState", menuName = "StatesSO/Die")]
public class DieState : StateSO
{
    public override void OnStateEnter(EnemyController ec)
    {
        ec.StartCoroutine(DieCoroutine(ec));
    }

    public override void OnStateExit(EnemyController ec)
    {

    }

    public override void OnStateUpdate(EnemyController ec)
    {

    }
    public IEnumerator DieCoroutine(EnemyController ec)
    {
        ec.GetComponent<Pathfinding>().StopCoroutine("PatrolRoutine");
        ec.animator.SetTrigger("die");
        ec.GetComponent<NavMeshAgent>().isStopped = true;
        ec.GetComponent<NavMeshAgent>().GetComponent<Collider>().enabled = false;
        yield return new WaitForSeconds(2f);
        GameObject.Destroy(ec.gameObject);
    }
}