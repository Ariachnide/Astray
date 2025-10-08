using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public enum PlayerControlType {
    Movement,
    Action,
    Weapon,
    Spell,
    OtherObject,
    Inventory,
    MainMenu
}

public enum NextOrderType {
    AllowAction,
    Continue,
    Stop
}

public enum CtrlOverriderType {
    Mouth,
    TinyBlob
}

public class PlayerControl : MonoBehaviour {
    private PlayerMain main;
    private PlayerItem pItem;
    private PlayerSpell spell;
    private PlayerWeapon weapon;
    private PlayerAction action;
    private PlayerInventory inventory;
    private PlayerMenu pMenu;

    private Dictionary<PlayerControlType, SortedDictionary<Int16, GameObject>> overridenControls = new() {
        { PlayerControlType.Movement, new() },
        { PlayerControlType.Action, new() },
        { PlayerControlType.Weapon, new() },
        { PlayerControlType.Spell, new() },
        { PlayerControlType.OtherObject, new() },
        { PlayerControlType.Inventory, new() },
        { PlayerControlType.MainMenu, new() }
    };
    private NextOrderType cachedNOT;

    private List<CtrlOverriderType> tempCtrlOverriders = new();
    private List<Int16>
        tempOverriderToDeleteOnCheckAction = new(),
        tempOverriderToDeleteOnDeleteAction = new();

    private bool tempOverridenBlockControl;

    private void Awake() {
        main = transform.parent.GetComponent<PlayerMain>();
        pItem = transform.parent.GetComponent<PlayerItem>();
        spell = transform.parent.GetComponent<PlayerSpell>();
        weapon = transform.parent.GetComponent<PlayerWeapon>();
        action = transform.parent.GetComponent<PlayerAction>();
        inventory = transform.parent.GetComponent<PlayerInventory>();
        pMenu = transform.parent.GetComponent<PlayerMenu>();
    }

    private bool CheckOverridenControls(PlayerControlType pct, InputAction.CallbackContext context) {
        if (overridenControls[pct].Count > 0 && Time.timeScale > 0.1f) {
            tempOverridenBlockControl = false;

            foreach (KeyValuePair<Int16, GameObject> kvp in overridenControls[pct].Reverse()) {
                if (kvp.Value == null) {
                    tempOverriderToDeleteOnCheckAction.Add(kvp.Key);
                    continue;
                }
                if (tempCtrlOverriders.Contains(kvp.Value.GetComponent<IControlOverride>().GetOverriderType())) {
                    continue;
                } else {
                    tempCtrlOverriders.Add(kvp.Value.GetComponent<IControlOverride>().GetOverriderType());
                }

                cachedNOT = kvp.Value.GetComponent<IControlOverride>().CallOverrideInterceptor(pct, context);

                if (cachedNOT == NextOrderType.AllowAction) {
                    tempOverridenBlockControl = true;
                    continue;
                } else if (cachedNOT == NextOrderType.Continue) {
                    continue;
                } else if (cachedNOT == NextOrderType.Stop) {
                    tempOverridenBlockControl = false;
                    break;
                }
            }

            foreach (Int16 i in tempOverriderToDeleteOnCheckAction) overridenControls[pct].Remove(i);
            if (tempCtrlOverriders.Count > 0) tempCtrlOverriders.Clear();
            tempCtrlOverriders.Clear();

            return tempOverridenBlockControl;
        } else {
            return true;
        }
    }

    public void OnObject(InputAction.CallbackContext context) {
        if (!CheckOverridenControls(PlayerControlType.OtherObject, context)) return;
        if (context.started)
            pItem.Use();
    }

    public void OnSpell(InputAction.CallbackContext context) {
        if (!CheckOverridenControls(PlayerControlType.Spell, context)) return;
        if (context.started) {
            if (pMenu.state == MainMenuState.none) {
                spell.Use();
            } else {
                pMenu.HandleReturn();
            }
        }
    }

    public void OnWeapon(InputAction.CallbackContext context) {
        if (!CheckOverridenControls(PlayerControlType.Weapon, context)) return;
        if (context.started) {
            if (pMenu.state == MainMenuState.none) {
                weapon.HandleAttack();
            } else {
                pMenu.HandleReturn();
            }
        }
    }

    public void OnAction(InputAction.CallbackContext context) {
        if (!CheckOverridenControls(PlayerControlType.Action, context)) return;
        if (context.started)
            action.HandleAction();
    }

    public void OnMovement(InputAction.CallbackContext context) {
        if (!CheckOverridenControls(PlayerControlType.Movement, context)) return;
        if (main.state == PlayerState.inInventory) {
            if (context.started)
                inventory.HandleInventorySelector(context.ReadValue<Vector2>().normalized);
        } else if (main.state == PlayerState.inMenu) {
            if (context.started)
                pMenu.HandleMenuSelector(context.ReadValue<Vector2>().normalized);
        } else {
            main.SetMovement(context.ReadValue<Vector2>().normalized);
        }
    }

    public void OnInventory(InputAction.CallbackContext context) {
        if (!CheckOverridenControls(PlayerControlType.Inventory, context)) return;
        if (context.started)
            inventory.HandleInventoryWindow();
    }

    public void OnMainMenu(InputAction.CallbackContext context) {
        if (!CheckOverridenControls(PlayerControlType.MainMenu, context)) return;
        if (context.started)
            pMenu.HandleMainMenu();
    }

    public void OverrideControls(IControlOverride iControlOverride) {
        foreach (PlayerControlType pco in iControlOverride.GetOverridenControls())
            overridenControls[pco][iControlOverride.GetOverridePriority()] = iControlOverride.GetRelatedGameObject();
    }

    public void UpdateOverridenControls(GameObject newGoRef) {
        foreach (PlayerControlType newGO_PCT in newGoRef.GetComponent<IControlOverride>().GetOverridenControls())
            overridenControls[newGO_PCT][newGoRef.GetComponent<IControlOverride>().GetOverridePriority()] = newGoRef;
    }

    public void DeleteOverridenControls(CtrlOverriderType type) {
        foreach (KeyValuePair<PlayerControlType, SortedDictionary<Int16, GameObject>> kvpCtrlType in overridenControls) {
            foreach (KeyValuePair<Int16, GameObject> kvpOverrider in kvpCtrlType.Value) {
                if (kvpOverrider.Value == null) {
                    tempOverriderToDeleteOnDeleteAction.Add(kvpOverrider.Key);
                } else if (kvpOverrider.Value.GetComponent<IControlOverride>().GetOverriderType() == type) {
                    tempOverriderToDeleteOnDeleteAction.Add(kvpOverrider.Key);
                }
            }
            if (tempOverriderToDeleteOnDeleteAction.Count > 0) {
                foreach (Int16 i in tempOverriderToDeleteOnDeleteAction) kvpCtrlType.Value.Remove(i);
                tempOverriderToDeleteOnDeleteAction.Clear();
            }
        }
    }
}
