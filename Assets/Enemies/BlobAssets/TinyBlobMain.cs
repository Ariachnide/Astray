using System;
using System.Collections;
using UnityEngine;

public class TinyBlobMain : DefaultEnemyMain {
    private GameObject player;
    public Int16 persistance;

    protected override void Awake() {
        base.Awake();
        if (persistance <= 0) persistance = 1500;
    }

    protected override IEnumerator Knockback(Vector3 argPosition, float duration, float thrust) {
        yield return base.Knockback(argPosition, duration, thrust * 1.5f);
    }

    public override void GetDamage(Int16 dmg) {
        base.GetDamage(dmg);
        if (hitPoints > 1) {
            canGetKnockedBack = true;
        } else {
            canGetKnockedBack = false;
        }
    }

    public override void GetHit(
        Int16 damage,
        GameObject blowGiver,
        Vector3 blowGiverPosition,
        float kbThrust,
        float kbDuration,
        string weaponName = ""
    ) {
        base.GetHit(damage, blowGiver, blowGiverPosition, kbThrust, kbDuration, weaponName);
        if (canGetKnockedBack) Physics2D.IgnoreCollision(GetComponent<Collider2D>(), AccessPlayerValue().GetComponent<Collider2D>(), true);
    }

    private GameObject AccessPlayerValue() {
        if (player == null) player = GameObject.FindWithTag("Player");
        return player;
    }

    public override void EndKnockback(EnemyState newState = EnemyState.Idle, bool allowMoves = true) {
        base.EndKnockback(newState);
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), AccessPlayerValue().GetComponent<Collider2D>(), false);
    }
}
