using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "AttackState", menuName = "StatesSO/Attack")]
public class AttackState : StateSO
{
    private bool isAttacking = false;
    private bool useFirstAttack = true;

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
    }

    public override void OnStateUpdate(EnemyController ec)
    {
    }

    public IEnumerator AttackCoroutine(EnemyController ec)
    {
        while (ec.OnAttackRange)
        {
            if (useFirstAttack)
            {
                ec.animator.SetTrigger("attack1");
            }
            else
            {
                ec.animator.SetTrigger("attack2");
            }

            useFirstAttack = !useFirstAttack;

            AnimatorClipInfo[] clipInfo = ec.animator.GetCurrentAnimatorClipInfo(0);
            float animationDuration = clipInfo.Length > 0 ? clipInfo[0].clip.length : 1f;

            yield return new WaitForSeconds(animationDuration);
        }

        ec.ExitCurrentNode();
    }
}
