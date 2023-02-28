﻿using System.Collections.Generic;
using BehaviorTree.SO.Actions;
using Core.Enemy.BehaviorTree.SO.ActionsSO;
using Environment.MoveGrid;

namespace BehaviorTree.Nodes.Actions
{
    public class GetMovePointOfLineNode : ActionNode
    {
        private GetMovePointOfLineNodeSO _getMovePointOfLineNodeSo;
        private GetMovePointOfLineNodeDataSO _getMovePointOfLineNodeDataSo;
        private EnvironmentGridManager _environmentGridManager;

        public override void SetNodeSO(NodeSO nodeSO)
        {
            _getMovePointOfLineNodeSo = (GetMovePointOfLineNodeSO)nodeSO;
            _getMovePointOfLineNodeDataSo = (GetMovePointOfLineNodeDataSO)_getMovePointOfLineNodeSo.Data;
        }

        public override NodeSO GetNodeSO()
        {
            return _getMovePointOfLineNodeSo;
        }

        public override BehaviourTreeEnums.NodeState Evaluate()
        {
            int startIndex = (int)Sharer.InternValues[_getMovePointOfLineNodeSo.StartIndexKey.HashCode];
            Sharer.InternValues[_getMovePointOfLineNodeSo.ResultIndexKey.HashCode] =
                _environmentGridManager.GetIndexMovePointFromStartMovePointLine(startIndex,
                    _getMovePointOfLineNodeDataSo.indexMovedAmount);
            
            return BehaviourTreeEnums.NodeState.SUCCESS;
        }

        public override void SetDependencyValues(
            Dictionary<BehaviourTreeEnums.TreeExternValues, object> externDependencyValues,
            Dictionary<BehaviourTreeEnums.TreeEnemyValues, object> enemyDependencyValues)
        {
            _environmentGridManager =
                (EnvironmentGridManager)externDependencyValues[
                    BehaviourTreeEnums.TreeExternValues.EnvironmentGridManager];
        }

        public override ActionNodeDataSO GetDataSO()
        {
            return _getMovePointOfLineNodeDataSo;
        }
    }
}