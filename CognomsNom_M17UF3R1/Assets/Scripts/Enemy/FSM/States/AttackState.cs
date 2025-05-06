using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "AttackState", menuName = "StatesSO/Attack")]
public class AttackState : StateSO
{
    private bool isAttacking = false;

    public override void OnStateEnter(EnemyController ec)
    {
        NavMeshAgent agent = ec.gameObject.GetComponent<NavMeshAgent>();
        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        if (!isAttacking)
        {
            isAttacking = true;
            ec.StartCoroutine(AttackCoroutine(ec));
        }
    }

    public override void OnStateExit(EnemyController ec)
    {
        isAttacking = false;
        NavMeshAgent agent = ec.gameObject.GetComponent<NavMeshAgent>();
        agent.isStopped = false;
        if (ec.GetComponent<EnemyDmgSource>() != null)
        {
            ec.GetComponent<EnemyDmgSource>().canDealDamage = false;
        }
    }

    public override void OnStateUpdate(EnemyController ec)
    {
    }

    public IEnumerator AttackCoroutine(EnemyController ec)
    {
        while (ec.OnAttackRange)
        {
            ec.animator.SetTrigger("attack1");

            AnimatorClipInfo[] clipInfo = ec.animator.GetCurrentAnimatorClipInfo(0);
            float animationDuration = clipInfo.Length > 0 ? clipInfo[0].clip.length : 1f;
            float impactTime = animationDuration * 0.5f;
            float damageWindow = 0.2f;

            if (ec.GetComponent<EnemyDmgSource>() != null)
            {
                ec.GetComponent<EnemyDmgSource>().canDealDamage = false;
            }
            yield return new WaitForSeconds(impactTime);
            if (ec.GetComponent<EnemyDmgSource>() != null)
            {
                ec.GetComponent<EnemyDmgSource>().canDealDamage = true;
            }
            yield return new WaitForSeconds(damageWindow);
            if (ec.GetComponent<EnemyDmgSource>() != null)
            {
                ec.GetComponent<EnemyDmgSource>().canDealDamage = false;
            }

            float remainingTime = animationDuration - (impactTime + damageWindow);
            if (remainingTime > 0)
            {
                yield return new WaitForSeconds(remainingTime);
            }
        }

        ec.ExitCurrentNode();
    }
}