using System;
using UnityEngine;

namespace BehaviorTree
{
    public abstract class NodeSO : BehaviourTreeSO
    {
        [TextArea] public string Comment;
       
        public abstract Type GetTypeNode();

        protected virtual void OnValidate()
        {
            UpdateComment();
        }

        public abstract void UpdateComment();
    }
}