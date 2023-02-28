﻿using System;
using BehaviorTree.Nodes.Actions;

namespace BehaviorTree.SO.Actions
{
    public class GetMovePointOfLineNodeDataSO : ActionNodeDataSO
    {
        public int indexMovedAmount;
        
        protected override void SetDependencyValues()
        {
            ExternValues = new[] { BehaviourTreeEnums.TreeExternValues.EnvironmentGridManager };
        }

        public override Type GetTypeNode()
        {
            return typeof(GetMovePointOfLineNode);
        }
    }
}