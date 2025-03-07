﻿using System;
using UnityEngine.InputSystem;

namespace Service.Inputs
{
    public interface IInputService : IService
    {
        void AddTap(Action<InputAction.CallbackContext> successEvent);
        void RemoveTap(Action<InputAction.CallbackContext> successEvent);
        void AddSwipe(SwipeSO swipeSo, Action<Swipe> successEvent);
        void RemoveSwipe(SwipeSO swipeSo);

        void SetHold(Action<InputAction.CallbackContext> successHoldEvent, Action<InputAction.CallbackContext> successCancelHoldEvent);

        void ClearHold();
    }
}