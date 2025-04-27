using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
[CreateAssetMenu(fileName = "AttackCondition", menuName = "ConditionSO/Attack")]
public class AttackCondition : ConditionSO
{
    public override bool CheckCondition(EnemyController ec)
    {
        return ec.OnAttackRange;
    }
}