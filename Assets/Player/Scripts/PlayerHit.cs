using System;
using System.Collections;
using UnityEngine;

public enum AnimosityState {
    Player,
    Enemy
}

public class PlayerHit : MonoBehaviour, IHandleHostileContact {
    public bool isImmune, canGetKnockedBack;
    private PlayerWeapon weapon;
    private PlayerSpell spell;
    private PlayerItem pItem;
    private PlayerMain main;
    private Animator anim;
    private Rigidbody2D rb;
    private Renderer srd;
    private WaitForFixedUpdate waitForFixedUpdate;

    public GameObject OwnRestrainer { get; private set; } = null;
    private Coroutine handleHitEffetCo = null;

    private void Awake() {
        weapon = GetComponent<PlayerWeapon>();
        spell = GetComponent<PlayerSpell>();
        pItem = GetComponent<PlayerItem>();
        main = GetComponent<PlayerMain>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        srd = GetComponent<SpriteRenderer>();
        waitForFixedUpdate = new WaitForFixedUpdate();
        canGetKnockedBack = true;
    }

    public bool TryDealMeleeDamages(
        Int16 damage,
        Vector2 enemyPosition,
        float kbThrust = 25f,
        float kbDuration = 0.1f
    ) {
        return GetHit(damage, enemyPosition, kbThrust, kbDuration);
    }

    public void TryRestrain(GameObject restrainer) {
        SetRestrainer(restrainer);
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    public bool GetHit(
        Int16 damage,
        Vector3 enemyPosition,
        float kbThrust = 25f,
        float kbDuration = 0.1f
    ) {
        if (!isImmune) {
            main.GetDamage(damage);
            isImmune = true;
            if (OwnRestrainer != null) {
                OwnRestrainer.GetComponent<ICustomRestrainer>().CustomHit(gameObject);
                return true;
            }
            handleHitEffetCo = StartCoroutine(HandleHitEffet());
            DirectKnockback(enemyPosition, kbDuration, kbThrust);
            return true;
        } else {
            return false;
        }
    }

    public void DirectKnockback(Vector3 argEnemyPosition, float duration, float thrust) {
        if (canGetKnockedBack)
            StartCoroutine(Knockback(argEnemyPosition, duration, thrust));
    }

    public void GiveImmunity(float duration = 1.5f) {
        if (handleHitEffetCo != null) EndHandleHitEffet();
        isImmune = true;
        handleHitEffetCo = StartCoroutine(HandleHitEffet(duration));
    }

    private IEnumerator HandleHitEffet(float duration = 1.5f) {
        float blinkDuration = Time.time + duration;
        Int16 blinkCd = 5;
        while (Time.time < blinkDuration) {
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
        EndHandleHitEffet();
    }

    private void EndHandleHitEffet() {
        StopCoroutine(handleHitEffetCo);
        handleHitEffetCo = null;
        srd.enabled = true;
        isImmune = false;
    }

    private IEnumerator Knockback(Vector3 argPosition, float duration, float thrust) {
        if (main.state == PlayerState.usingWeapon) {
            weapon.isComboAllowed = false;
            weapon.InterruptAttack(stateArg: PlayerState.knockbacked);
        } else if (main.state == PlayerState.usingSpell) {
            spell.InterruptSpellAnim(stateArg: PlayerState.knockbacked);
        } else if (main.state == PlayerState.usingItem) {
            pItem.InterruptItemAnim(stateArg: PlayerState.knockbacked);
        } else {
            main.state = PlayerState.knockbacked;
        }
        weapon.isComboFailed = true;
        anim.SetBool("isHurt", true);
        Vector2 direction = transform.position - argPosition;
        Vector2 force = direction.normalized * thrust;
        float kbDuration = Time.time + duration;
        while (Time.time < kbDuration) {
            rb.AddForce(force, ForceMode2D.Impulse);
            yield return waitForFixedUpdate;
        }
        anim.SetBool("isHurt", false);
        main.state = PlayerState.idle;
    }

    public bool IsVulnerableToRestrain(GameObject aggressor) {
        if (OwnRestrainer == null) {
            return true;
        } else {
            return !OwnRestrainer.GetComponent<ICustomRestrainer>().CanKeepTarget(aggressor);
        }
    }

    public GameObject GetRestrainedPlayer() {
        return gameObject;
    }

    public void SetRestrainer(GameObject go) {
        if (OwnRestrainer != null) OwnRestrainer.GetComponent<ICustomRestrainer>().RestoreDefaultBehavior();
        OwnRestrainer = go;
    }

    public void CleanRestrainer() {
        OwnRestrainer = null;
    }
}
