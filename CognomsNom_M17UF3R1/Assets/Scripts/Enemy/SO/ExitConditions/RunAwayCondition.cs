
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
[CreateAssetMenu(fileName = "RunAwayCondition", menuName = "ConditionSO/RunAway")]
public class RunAwayCondition : ConditionSO
{
    public override bool CheckCondition(EnemyController ec)
    {
        if (ec.HP <= 25)
        {
            return ec.runAway == true;
        }
        return false;
    }
}