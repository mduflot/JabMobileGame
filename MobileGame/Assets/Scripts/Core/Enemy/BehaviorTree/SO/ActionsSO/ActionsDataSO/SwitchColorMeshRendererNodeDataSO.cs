using System;
using BehaviorTree.Nodes.Actions;
using UnityEngine;

namespace BehaviorTree.SO.Actions
{
    [CreateAssetMenu(menuName = "BehaviorTree/Data/SwitchColorMeshRendererNodeDataSO",
        fileName = "new SwitchColorMeshRendererNodeData")]
    public class SwitchColorMeshRendererNodeDataSO : ActionNodeDataSO
    {
        public Color SwitchableColor;

        public override Type GetTypeNode()
        {
            return typeof(SwitchColorMeshRendererNode);
        }

        protected override void SetDependencyValues()
        {
            EnemyValues = new[] { BehaviourTreeEnums.TreeEnemyValues.MeshRenderer };
        }
    }
}