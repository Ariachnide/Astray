using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackHitbox : MonoBehaviour {
    private List<string> targetableElements;
    private PlayerWeapon weapon;
    public int WpnCount { get; private set; }

    private void Awake() {
        weapon = transform.parent.GetComponent<PlayerWeapon>();
    }

    private void Start() {
        targetableElements = transform
            .GetComponentInParent<PlayerTargetableElements>()
            .GetWeaponMeleeTargetableElements();
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        if (targetableElements.Contains(collider.gameObject.tag))
            weapon.Strike(collider);
    }

    private void OnTriggerStay2D(Collider2D collider) {
        if (targetableElements.Contains(collider.gameObject.tag))
            weapon.Strike(collider);
    }

    private void OnEnable() {
        WpnCount++;
    }
}
