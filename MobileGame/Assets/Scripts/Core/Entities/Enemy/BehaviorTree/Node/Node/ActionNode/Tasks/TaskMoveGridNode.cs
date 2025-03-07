﻿using System.Collections.Generic;
using BehaviorTree.SO.Actions;
using Environment.MoveGrid;
using Player.Handler;
using UnityEngine;

namespace BehaviorTree.Nodes.Actions
{
    public class TaskMoveGridNode : ActionNode
    {
        private TaskMoveGridNodeSO _so;
        private TaskMoveGridNodeDataSO _data;
        private GridManager _gridManager;
        private PlayerMovementHandler _playerMovementHandler;

        public override NodeSO GetNodeSO()
        {
            return _so;
        }

        public override void SetNodeSO(NodeSO nodeSO)
        {
            _so = (TaskMoveGridNodeSO)nodeSO;
            _data = (TaskMoveGridNodeDataSO)_so.Data;
        }

        public override void Evaluate()
        {
            base.Evaluate();
            _gridManager.MoveGrid((Vector3)Sharer.InternValues[_so.InternValues[0].HashCode]);
            _playerMovementHandler.SetCurrentMovePoint((int)Sharer.InternValues[_so.InternValues[1].HashCode]);
            State = BehaviorTreeEnums.NodeState.SUCCESS;
            ReturnedEvent?.Invoke();
        }

        public override void SetDependencyValues(
            Dictionary<BehaviorTreeEnums.TreeExternValues, object> externDependencyValues,
            Dictionary<BehaviorTreeEnums.TreeEnemyValues, object> enemyDependencyValues)
        {
            _gridManager =
                (GridManager)externDependencyValues[
                    BehaviorTreeEnums.TreeExternValues.GridManager];
            _playerMovementHandler =
                (PlayerMovementHandler)externDependencyValues[
                    BehaviorTreeEnums.TreeExternValues.PlayerMovementHandler];
        }

        public override ActionNodeDataSO GetDataSO()
        {
            return _data;
        }
    }
}