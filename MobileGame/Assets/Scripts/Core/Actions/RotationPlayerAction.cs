using HelperPSR.MonoLoopFunctions;
using UnityEngine;
using UnityEngine.Serialization;

namespace Actions
{
    public class RotationPlayerAction : PlayerAction, IUpdatable
    {
        private Transform _lookTarget;

        public void OnUpdate()
        {
            Vector3 newForward = _lookTarget.position - transform.position;
            newForward.y = transform.position.y;
            transform.forward = newForward.normalized;
        }

        public override bool IsInAction { get; }

        public override void MakeAction()
        {
            UpdateManager.Register(this);
        }

        public override void SetupAction(params object[] arguments)
        {
            _lookTarget = (Transform)arguments[0];
        }

 
      
    }
}