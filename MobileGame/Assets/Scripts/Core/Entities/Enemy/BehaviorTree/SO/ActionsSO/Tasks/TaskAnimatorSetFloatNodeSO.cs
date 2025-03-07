﻿using UnityEngine;

namespace BehaviorTree
{
    [CreateAssetMenu(menuName = "BehaviorTree/Actions/Tasks/AnimatorSetFloatNodeSO",
        fileName = "new Tree_T_AnimatorSetFloat_Spe")]
    public class TaskAnimatorSetFloatNodeSO : TaskNodeSO
    {
        public override void UpdateInterValues()
        {
            base.UpdateInterValues();
            _internValuesCount = 1;
            if (InternValues.Count > 0)
            {
                InternValues[0].SetInternValueWithoutKey(BehaviorTreeEnums.InternValueType.FLOAT,
                    BehaviorTreeEnums.InternValuePropertyType.GET, "Value(float) of a parameter");
            }
        }

        public override void UpdateComment()
        {
            Comment = "Nœud qui permet de modifier la valeur float d'un paramètre dans un animator";
        }
    }
}