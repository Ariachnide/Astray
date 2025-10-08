using System;
using System.Collections;
using UnityEngine;

public class DefaultEnemyMain : MainEnemyFactory {
    protected Coroutine
        blinkingCo = null,
        knockbackCo = null,
        spinningCo = null;

    protected virtual void OnDestroy() {
        EliminateStatusObjects();
    }

    protected virtual void OnDisable() {
        if (knockbackCo != null) {
            EndKnockback();
            CheckHP(false);
        }
        if (blinkingCo != null) EndBlinking();
        if (WhirlGO != null) EndSpecialEffect(SpecialEffectType.Whirl);
    }

    public override void SetSpecialEffect(
        SpecialEffectType effect,
        Single duration,
        GameObject effectGO = null
    ) {
        switch (effect) {
            case SpecialEffectType.Whirl:
                WhirlGO = effectGO;
                Spin(duration);
                if (hasMovementEF) movementEF.HandleMobility(false);
                Blink(duration);
                break;
        }
    }

    public override void GetDamage(Int16 dmg) {
        hitPoints -= dmg;
    }

    public override void UpdateLastBlowGiver(GameObject blowGiver) {
        if (blowGiver.name == "Player") {
            lastPlayerAttackData.player = blowGiver;
            lastPlayerAttackData.wpnAttackId = blowGiver.GetComponent<PlayerWeapon>().GetAttackId();
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
        UpdateLastBlowGiver(blowGiver);
        GetDamage(damage);
        Blink(1f);
        HandleStatusOnHit();
        DirectKnockback(blowGiverPosition, kbDuration, kbThrust);
        if (!canGetKnockedBack) CheckHP();
    }

    public override void DirectKnockback(Vector3 argPosition, float duration, float thrust) {
        if (canGetKnockedBack) knockbackCo = StartCoroutine(Knockback(argPosition, duration, thrust));
    }

    protected virtual IEnumerator Knockback(Vector3 argPosition, float duration, float thrust) {
        if (hasDefaultCollision) defaultEnemyCollision.isDangerousOnCollision = false;
        if (hasDefaultTrigger) defaultEnemyTrigger.isDangerousOnTrigger = false;
        if (hasAnimatorEF) animatorEF.Knockback(true);
        SelfState = EnemyState.Knockbacked;
        Vector2 direction = transform.position - argPosition;
        Vector2 force = direction.normalized * (thrust * rb.mass);
        float kbDuration = Time.time + duration;
        while (Time.time < kbDuration) {
            rb.AddForce(force, ForceMode2D.Impulse);
            yield return new WaitForFixedUpdate();
        }
        CheckHP();
        EndKnockback();
    }

    public override void EndKnockback(EnemyState newState = EnemyState.Idle, bool allowMoves = true) {
        if (knockbackCo != null) StopCoroutine(knockbackCo);
        knockbackCo = null;
        SelfState = newState;

        if (hasMovementEF && allowMoves) {
            movementEF.HandleMobility(true);
            movementEF.HandleRegularMoves(true);
        }
        if (hasDefaultCollision) defaultEnemyCollision.isDangerousOnCollision = true;
        if (hasDefaultTrigger) defaultEnemyTrigger.isDangerousOnTrigger = true;
        if (hasAnimatorEF) animatorEF.Knockback(false);
    }

    public override void Blink(float duration) {
        if (blinkingCo != null) EndBlinking();
        blinkingCo = StartCoroutine(Blinking(duration));
    }

    protected IEnumerator Blinking(float duration) {
        float blinkDuration = Time.time + duration;
        Int16 blinkCD = 5;
        while (Time.time < blinkDuration) {
            if (Time.timeScale < 0.1f) {
                yield return null;
                continue;
            }
            if (blinkCD == 5) {
                blinkCD = 0;
                srd.enabled = !srd.enabled;
            } else {
                blinkCD++;
            }
            yield return null;
        }
        EndBlinking();
    }

    protected override void EndBlinking() {
        StopCoroutine(blinkingCo);
        blinkingCo = null;
        srd.enabled = true;
    }

    public override void CheckHP(bool animate = true) {
        if (hitPoints <= 0) {
            if (animate) {
                GameObject destructionAnimatonGO = Instantiate(destructionAnimation, transform.position, Quaternion.identity);
                destructionAnimatonGO.transform.SetParent(transform.parent);
                destructionAnimatonGO.GetComponent<EnemyDestructionEffect>().loot = lootController.HandleLoot();
                Destroy(gameObject);
            } else {
                Destroy(gameObject);
            }
        }
    }

    public override void EndSpecialEffect(SpecialEffectType effect) {
        switch (effect) {
            case SpecialEffectType.Whirl:
                EndSpinning();
                if (hasMovementEF) {
                    movementEF.HandleMobility(true);
                    movementEF.HandleRegularMoves(true);
                }
                Destroy(WhirlGO);
                break;
        }
        if (shouldCheckHP) {
            CheckHP();
            shouldCheckHP = false;
        }
    }

    protected override void HandleStatusOnHit() {
        if (WhirlGO != null) EndSpecialEffect(SpecialEffectType.Whirl);
    }

    public override void EliminateStatusObjects() {
        if (WhirlGO != null) Destroy(WhirlGO);
    }

    public override void HandleKB() {
        if (SelfState == EnemyState.Knockbacked) {
            EndKnockback();
            shouldCheckHP = true;
        }
    }

    public override void Spin(float duration) {
        spinningCo = StartCoroutine(Spinning(duration));
    }

    protected IEnumerator Spinning(float duration) {
        isSpinning = true;
        float globalTimer = Time.time + duration;

        if (hasDefaultCollision) defaultEnemyCollision.isDangerousOnCollision = false;
        if (hasDefaultTrigger) defaultEnemyTrigger.isDangerousOnTrigger = false;
        if (isDirectional && hasMovementEF) movementEF.Spin(globalTimer);
        if (hasAggroEF) {
            aggroEF.EndAggressiveBehaviour();
            aggroEF.HandleVisionStatus(false);
        }
        if (hasAnimatorEF) animatorEF.Spin(globalTimer);
        yield return new WaitForSeconds(duration);
        EndSpinning();
    }

    public override void EndSpinning() {
        if (spinningCo != null) {
            StopCoroutine(spinningCo);
            spinningCo = null;
            isSpinning = false;

            if (isDirectional && hasMovementEF) movementEF.EndSpinning();
            if (hasAnimatorEF) animatorEF.EndSpinning();
            if (hasDefaultCollision) defaultEnemyCollision.isDangerousOnCollision = true;
            if (hasDefaultTrigger) defaultEnemyTrigger.isDangerousOnTrigger = true;
            if (hasAggroEF) aggroEF.HandleVisionStatus(true);
        }
    }
}
