using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        ec.animator.SetTrigger("die");
        yield return new WaitForSeconds(1f);
        GameObject.Destroy(ec.gameObject);
    }
}