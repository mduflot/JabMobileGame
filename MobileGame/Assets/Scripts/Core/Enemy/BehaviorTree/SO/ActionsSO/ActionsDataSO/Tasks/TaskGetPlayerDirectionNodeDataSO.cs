﻿using System;
using BehaviorTree.Nodes.Actions;

namespace BehaviorTree.SO.Actions
{
    public class TaskGetPlayerDirectionNodeDataSO : ActionNodeDataSO
    {
        protected override void SetDependencyValues()
        {
            ExternValues = new[] { BehaviorTreeEnums.TreeExternValues.EnvironmentGridManager };
            EnemyValues = new[] { BehaviorTreeEnums.TreeEnemyValues.Transform };
        }

        public override Type GetTypeNode()
        {
            return typeof(TaskGetPlayerDirectionNode);
        }
    }
}