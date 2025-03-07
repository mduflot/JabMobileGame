﻿using System;
using BehaviorTree.SO.Actions;
using UnityEngine;

namespace BehaviorTree.Nodes.Actions
{
    public class TaskInvokeCallbackNode : ActionNode
    {
        private TaskInvokeCallbackNodeSO _so;
        private TaskInvokeCallbackNodeDataSO _dataSo;

        public override void Evaluate()
        {
            var eventToInvoke = (Action)Sharer.InternValues[_so.InternValues[0].HashCode];
            eventToInvoke?.Invoke();
            State = BehaviorTreeEnums.NodeState.SUCCESS;
            ReturnedEvent?.Invoke();
        }

        public override void SetNodeSO(NodeSO nodeSO)
        {
            base.Evaluate();
            _so = (TaskInvokeCallbackNodeSO)nodeSO;
            _dataSo = (TaskInvokeCallbackNodeDataSO)_so.Data;
        }

        public override NodeSO GetNodeSO()
        {
            return _so;
        }

        public override ActionNodeDataSO GetDataSO()
        {
            return _dataSo;
        }
    }
}