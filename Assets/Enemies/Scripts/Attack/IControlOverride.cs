using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IControlOverride {
    public NextOrderType CallOverrideInterceptor(PlayerControlType controlType, InputAction.CallbackContext context);
    public List<PlayerControlType> GetOverridenControls();
    public Int16 GetOverridePriority();
    public CtrlOverriderType GetOverriderType();
    public GameObject GetRelatedGameObject();
}
