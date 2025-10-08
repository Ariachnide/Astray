using System;
using System.Collections;
using UnityEngine;

public class RockProjectile : ProjectileDefault {
    public bool canBeRedirected = true;

    protected override void HandleHitBySword(
        Int16 damage,
        GameObject blowGiver,
        Vector3 enemyPosition,
        float kbThrust,
        float kbDuration,
        string weaponName
    ) {
        if (!canBeRedirected)
            DestroyProjectile();
        EndLaunch();
        rb.velocity = Vector2.zero;
        canBeRedirected = false;
        gameObject.tag = "AllyProjectile";
        Vector2 direction = transform.position - enemyPosition;
        HandleLaunch(direction, blowGiver, kbThrust, argDmg: (Int16)Mathf.FloorToInt(damage / 2), isAlly: true);
    }

    protected override IEnumerator HandleDestruction() {
        if (direction.x > 0f) {
            rb.AddForce(new Vector2(-0.5f, 0f), ForceMode2D.Impulse);
        } else if (direction.x < 0f) {
            rb.AddForce(new Vector2(0.5f, 0f), ForceMode2D.Impulse);
        }
        if (direction.y > 0f) {
            rb.AddForce(new Vector2(0f, -0.5f), ForceMode2D.Impulse);
        } else if (direction.y < 0f) {
            rb.AddForce(new Vector2(0f, 0.5f), ForceMode2D.Impulse);
        }
        float breakAnimDuration = Time.time + 0.5f;
        Int16 blinkCd = 5;
        while (Time.time < breakAnimDuration) {
            if (Time.timeScale < 0.1f) {
                yield return null;
                continue;
            }
            if (blinkCd == 5) {
                blinkCd = 0;
                srd.enabled = !srd.enabled;
            } else {
                blinkCd++;
            }
            yield return null;
        }
        Destroy(gameObject);
    }
}
