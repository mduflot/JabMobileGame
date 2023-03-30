using System.Collections.Generic;
using BehaviorTree.SO.Actions;
using Environment.MoveGrid;
using UnityEngine;

namespace BehaviorTree.Nodes.Actions
{
    public class CheckCirclesAreOccupiedNode : ActionNode
    {
        private CheckCirclesAreOccupiedNodeSO _so;
        private CheckCirclesAreOccupiedNodeDataSO _data;
        private EnvironmentGridManager _environmentGridManager;

        public override void Evaluate()
        {
            if (!_environmentGridManager.CheckIfOneMovePointInCirclesIsOccupied(_data.CircleIndexes,
                    (Vector3)Sharer.InternValues[_so.InternValues[0].HashCode]))
            {
                State = BehaviorTreeEnums.NodeState.SUCCESS;
                ReturnedEvent?.Invoke();
                return;
            }

            State = BehaviorTreeEnums.NodeState.FAILURE;
            ReturnedEvent?.Invoke();
        }

        public override void SetNodeSO(NodeSO nodeSO)
        {
            _so = (CheckCirclesAreOccupiedNodeSO)nodeSO;
            _data = (CheckCirclesAreOccupiedNodeDataSO)_so.Data;
        }

        public override void SetDependencyValues(
            Dictionary<BehaviorTreeEnums.TreeExternValues, object> externDependencyValues,
            Dictionary<BehaviorTreeEnums.TreeEnemyValues, object> enemyDependencyValues)
        {
            _environmentGridManager = (EnvironmentGridManager)
                externDependencyValues[BehaviorTreeEnums.TreeExternValues.EnvironmentGridManager];
        }

        public override NodeSO GetNodeSO()
        {
            return _so;
        }

        public override ActionNodeDataSO GetDataSO()
        {
            return _data;
        }
    }
}