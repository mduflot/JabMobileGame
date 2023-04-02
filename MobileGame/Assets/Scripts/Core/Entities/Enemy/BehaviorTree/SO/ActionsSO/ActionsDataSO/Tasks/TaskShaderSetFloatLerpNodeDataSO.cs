﻿using System;
using BehaviorTree.Nodes.Actions;
using UnityEngine;

namespace BehaviorTree.SO.Actions
{
    [CreateAssetMenu(menuName = "BehaviorTree/Data/Tasks/ShaderSetFloatLerpNodeDataSO",
        fileName = "new T_ShaderSetFloatLerp_Spe_Data")]
    public class TaskShaderSetFloatLerpNodeDataSO : ActionNodeDataSO
    {
        protected override void SetDependencyValues()
        {
            
        }

        public override Type GetTypeNode()
        {
            return typeof(TaskShaderSetFloatLerpNode);
        }
    }
}