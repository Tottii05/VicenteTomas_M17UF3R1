using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
[CreateAssetMenu(fileName = "DieCondition", menuName = "ConditionSO/Die")]
public class DieCondition : ConditionSO
{
    public override bool CheckCondition(EnemyController ec)
    {
        return ec.HP <=0;
    }
}