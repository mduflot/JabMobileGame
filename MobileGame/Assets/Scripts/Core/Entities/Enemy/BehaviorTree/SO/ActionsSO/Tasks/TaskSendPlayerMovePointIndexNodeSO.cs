﻿using UnityEngine;

namespace BehaviorTree
{
    [CreateAssetMenu(menuName = "BehaviorTree/Actions/Tasks/SendPlayerMovePointIndexNodeSO",
        fileName = "new Tree_T_SendPlayerMovePointIndex_Spe")]
    public class TaskSendPlayerMovePointIndexNodeSO : TaskNodeSO
    {
        public override void UpdateInterValues()
        {
            base.UpdateInterValues();
            _internValuesCount = 1;
            if (InternValues.Count > 0)
            {
                InternValues[0].SetInternValueWithoutKey(BehaviorTreeEnums.InternValueType.INT,
                    BehaviorTreeEnums.InternValuePropertyType.SET, "Index(int) of the movepoint where the player is");
            }
        }

        public override void UpdateComment()
        {
            Comment = "Nœud qui permet d'enregistrer la node du joueur durant une partie";
        }
    }
}