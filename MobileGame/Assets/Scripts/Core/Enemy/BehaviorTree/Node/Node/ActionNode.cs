using System.Collections.Generic;
using BehaviorTree.Data;
using BehaviorTree.Trees;
using Object = System.Object;

namespace BehaviorTree.Nodes
{
    public abstract class ActionNode : Node
    {
        public NodeValuesSharer Sharer;
        
        public abstract ActionNodeDataSO GetDataSO();
        
        public abstract void SetDataSO(ActionNodeDataSO so);

        public (BehaviourTreeEnums.TreeEnemyValues[] enemyValues, BehaviourTreeEnums.TreeExternValues[] externValues)
            GetDependencyValues()
        {
            var so = GetDataSO();
            return (so.EnemyValues, so.ExternValues);
        }

        public abstract void SetDependencyValues(
            Dictionary<BehaviourTreeEnums.TreeExternValues, Object> externDependencyValues,
            Dictionary<BehaviourTreeEnums.TreeEnemyValues, Object> enemyDependencyValues);
    }
}