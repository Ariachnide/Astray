using System.Collections.Generic;
using UnityEngine;

public class PlayerTargetableElements : MonoBehaviour {
    public List<string> weaponMeleeTargetableElements, spellTargetableElements;
    // public List<string> weaponRangeTargetableElements;

    private void Awake() {
        weaponMeleeTargetableElements = new List<string> {
            "Enemy",
            "HostileProjectile",
            "CollectiblesCommon",
            "BreakableElm"
        };
        /* weaponRangeTargetableElements = new List<string> {
            "Enemy",
            "HostileProjectile"
        }; */
        spellTargetableElements = new List<string> {
            "Enemy",
            "BreakableElm"
        };
    }

    /* public List<string> GetWeaponRangeTargetableElements() {
        return weaponRangeTargetableElements;
    } */

    public List<string> GetWeaponMeleeTargetableElements() {
        return weaponMeleeTargetableElements;
    }

    public List<string> GetSpellTargetableElements() {
        return spellTargetableElements;
    }
}
