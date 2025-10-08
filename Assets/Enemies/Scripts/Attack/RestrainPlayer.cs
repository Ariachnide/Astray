using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RestrainState {
    private Int16 _strength;
    public Int16 Strength { 
        get {
            return _strength;
        }
        private set {
            _strength = value > MaxStrength
                ? MaxStrength
                : value;
        }
    }
    public Int16 MaxStrength { get; private set; }
    public float Modifier { get; private set; }
    public GameObject Player { get; private set; }
    public float time = 0f;

    public RestrainState(Int16 s, Int16 ms, float m, GameObject p, float d) {
        MaxStrength = ms;
        Strength = s;
        Modifier = m;
        Player = p;
        time = Time.time + d;
    }

    public void RegainStrength(Int16 amount) {
        Strength += amount;
    }

    public void ReduceStrength(Int16 amount) {
        Strength -= (Int16)(amount * Modifier);
    }
}

public class RestrainPlayer : MonoBehaviour, IControlOverride {
    private RestrainState state = null;
    public Int16 restrainerPriority = 0;

    [SerializeField]
    private CtrlOverriderType ctrlOverriderType;
    public float restrainMvtTimer, restrainBtnTimer;
    private float tempRestrainMvtTimer, tempRestrainBtnTimer;
    public Int16 restrainMvtReduction, restrainBtnReduction;
    public Int16 ctrlOverridePriority;
    public Int16 defaultStrength, defaultMaxStrength;
    [SerializeField]
    private List<PlayerControlType> overridenControls = new();

    private void Awake() {
        if (ctrlOverridePriority == 0) Debug.LogError("WARNING: NO PRIORITY DEFINED FOR ctrlOverridePriority");
        if (restrainMvtTimer == 0) Debug.LogError("WARNING: NO PRIORITY DEFINED FOR restrainMvtTimer");
        if (restrainBtnTimer == 0) Debug.LogError("WARNING: NO PRIORITY DEFINED FOR restrainBtnTimer");
        if (restrainMvtReduction == 0) Debug.LogError("WARNING: NO PRIORITY DEFINED FOR restrainMvtReduction");
        if (restrainBtnReduction == 0) Debug.LogError("WARNING: NO PRIORITY DEFINED FOR restrainBtnReduction");

        tempRestrainMvtTimer = 0.1f - restrainMvtTimer;
        tempRestrainBtnTimer = 0.1f - restrainBtnTimer;
    }

    private void Update() {
        if (state != null) {
            if (Time.time > state.time) {
                gameObject.GetComponent<ICustomRestrainer>().CustomEffect(state.Player);
                state.RegainStrength(100);
                state.time = Time.time + 1f;
            }
        }
    }

    public void Restrain(
        GameObject player,
        Int16 strength = 0,
        Int16 maxStrength = 0,
        float resistance = 1f,
        float delay = 1f
    ) {
        state = new RestrainState(
            strength > 0 ? strength : defaultStrength,
            maxStrength > 0 ? maxStrength : defaultMaxStrength,
            resistance,
            player,
            delay
        );
    }

    public void BreakFree() {
        tempRestrainMvtTimer = 0.1f - restrainMvtTimer;
        tempRestrainBtnTimer = 0.1f - restrainBtnTimer;
        state.Player.GetComponent<PlayerHit>().canGetKnockedBack = true;
        state.Player.GetComponent<PlayerMain>().GetPlayerControl().DeleteOverridenControls(GetOverriderType());
        gameObject.GetComponent<ICustomRestrainer>().End();
        state = null;
    }

    public void RegainStrength(Int16 amount = 50) {
        state.RegainStrength(amount);
    }

    public bool IsHoldingTarget() {
        return state != null;
    }

    public bool CanKeepTargetByDefault(Int16 otherRestrainerPriority) {
        return restrainerPriority >= otherRestrainerPriority;
    }

    public GameObject GetRestrainedPlayer() {
        return state.Player;
    }

    public void ClearRestrainedPlayer() {
        state = null;
    }

    protected void OnDestroy() {
        if (IsHoldingTarget() && state.Player != null) BreakFree();
    }

    // IControlOverride methods
    public NextOrderType CallOverrideInterceptor(PlayerControlType controlType, InputAction.CallbackContext context) {
        NextOrderType response = NextOrderType.Stop;
        if (controlType == PlayerControlType.Movement) {
            if (Time.time > tempRestrainMvtTimer) {
                state.ReduceStrength(restrainMvtReduction);
                tempRestrainMvtTimer = Time.time + restrainMvtTimer;
            }
        } else {
            if (Time.time > tempRestrainBtnTimer) {
                state.ReduceStrength(restrainBtnReduction);
                tempRestrainBtnTimer = Time.time + restrainBtnTimer;
            }
        }
        if (state.Strength < 0) {
            BreakFree();
            response = NextOrderType.Continue;
        }
        return response;
    }

    public List<PlayerControlType> GetOverridenControls() {
        return overridenControls;
    }

    public Int16 GetOverridePriority() {
        return ctrlOverridePriority;
    }

    public CtrlOverriderType GetOverriderType() {
        return ctrlOverriderType;
    }

    public GameObject GetRelatedGameObject() {
        return gameObject;
    }
    // end IControlOverride methods
}
