﻿using System;

namespace Service.Inputs
{
    public interface IInputService : IService
    {
        void EnablePlayerMap(bool value);

        void AddSwipe(SwipeSO swipeSo, Action<Swipe> successEvent);

        void RemoveSwipe(Swipe swipe); 
        
    }
}