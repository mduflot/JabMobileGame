﻿using UnityEngine;

namespace BehaviorTree
{
    [CreateAssetMenu(menuName = "BehaviorTree/Actions/Tasks/InstantiateFXNodeSO", fileName = "new T_InstantiateFX_Spe")]
    public class TaskInstantiateFXNodeSO : TaskNodeSO
    {
        public override void UpdateInterValues()
        {
            base.UpdateInterValues();
            _internValuesCount = 1;
            if (InternValues.Count > 0)
            {
                InternValues[0].SetInternValueWithoutKey(BehaviorTreeEnums.InternValueType.INT,
                    BehaviorTreeEnums.InternValuePropertyType.GET, "MovePoint(Int) index of position");
            }
        }

        public override void UpdateComment()
        {
            Comment = "Nœud qui permet d'instancier un FX";
        }
    }
}