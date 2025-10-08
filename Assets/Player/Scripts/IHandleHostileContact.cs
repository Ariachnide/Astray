using System;
using UnityEngine;

public interface IHandleHostileContact {
    public bool IsVulnerableToRestrain(GameObject aggressor);
    public void TryRestrain(GameObject restrainer);
    public GameObject GetRestrainedPlayer();
    public bool TryDealMeleeDamages(
        Int16 contactDamage,
        Vector2 hostilePosition,
        float kbThrust = 25f,
        float kbDuration = 0.1f
    );
    public void DirectKnockback(Vector3 argEnemyPosition, float duration, float thrust);
}
