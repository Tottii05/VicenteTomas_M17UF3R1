using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
[CreateAssetMenu(fileName = "ChaseCondition", menuName = "ConditionSO/Chase")]
public class ChaseCondition : ConditionSO
{
    public override bool CheckCondition(EnemyController ec)
    {
        return ec.OnVisionRange;
    }
}