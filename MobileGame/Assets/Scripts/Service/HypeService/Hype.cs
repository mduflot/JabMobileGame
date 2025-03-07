﻿using System;

namespace Service.Hype
{
    public class Hype
    {
        public HypeSO HypeSo;
        public float CurrentValue;
        public float UltimateCurrentValue;
        public bool IsInUltimateArea;
        public Action<float> IncreaseHypeEvent;
        public Action<float> DecreaseHypeEvent;
        public Action<float> GainUltimateEvent;
        public Action<float> LoseUltimateEvent;
        public Action<float> SetHypeEvent;
    }
}