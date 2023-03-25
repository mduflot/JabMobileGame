using BehaviorTree.SO.Decorator;
using Cysharp.Threading.Tasks;

namespace BehaviorTree.Nodes.Decorator
{
    public class DecoratorWhileNode : DecoratorNode
    {
        private DecoratorWhileSO _so;
        private BehaviorTreeEnums.NodeState _childEvaluate;

        public override NodeSO GetNodeSO()
        {
            return _so;
        }

        public override void SetNodeSO(NodeSO nodeSO)
        {
            _so = (DecoratorWhileSO)nodeSO;
        }

        public override BehaviorTreeEnums.NodeState Evaluate()
        {
            ChildEvaluateAsync();
            return _childEvaluate;
        }

        private async void ChildEvaluateAsync()
        {
            _childEvaluate = Child.Evaluate();
            while (_childEvaluate == _so.WhileStateCondition)
            {
                _childEvaluate = Child.Evaluate();
                await UniTask.DelayFrame(0);
            }
        }
    }
}