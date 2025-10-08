using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(RestrainPlayer), typeof(MouthAnimator))]
public class MouthCollision :
    DefaultEnemyCollision,
    ICustomRestrainer,
    IHandleHostileContact {
    private Collider2D cld;
    private Rigidbody2D rb;
    private SpriteRenderer srd;
    private RestrainPlayer restrainPlayer;
    private MouthAnimator animatorHandler;
    private MainEnemyFactory main;
    private Coroutine handleHitEffetCo = null;

    private void Awake() {
        main = GetComponent<MainEnemyFactory>();
        cld = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        srd = GetComponent<SpriteRenderer>();
        restrainPlayer = GetComponent<RestrainPlayer>();
        animatorHandler = GetComponent<MouthAnimator>();
    }

    public override void HandleContact(GameObject contactTarget) {
        if (restrainPlayer.IsHoldingTarget()) return;
        if (!contactTarget.GetComponent<IHandleHostileContact>().IsVulnerableToRestrain(gameObject)) return;
        
        GameObject contactRestrainedPlayer = contactTarget.GetComponent<IHandleHostileContact>().GetRestrainedPlayer();
        if (contactRestrainedPlayer.GetComponent<PlayerHit>().isImmune) return;
        
        contactTarget.GetComponent<IHandleHostileContact>().TryRestrain(gameObject);

        main.SelfState = EnemyState.Immobilized;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        contactRestrainedPlayer.transform.position = transform.position;
        contactRestrainedPlayer.GetComponent<PlayerMain>().SetMovement(Vector2.zero);
        contactRestrainedPlayer.GetComponent<SpriteRenderer>().enabled = false;
        contactRestrainedPlayer.GetComponent<PlayerHit>().canGetKnockedBack = false;

        contactRestrainedPlayer.GetComponent<PlayerMain>().GetPlayerControl().UpdateOverridenControls(gameObject);

        restrainPlayer.Restrain(contactRestrainedPlayer);
        animatorHandler.HandleAttack(true);
    }

    // IHandleHostileContact methods
    public bool IsVulnerableToRestrain(GameObject aggressor) {
        if (!restrainPlayer.IsHoldingTarget()) return false;
        return !CanKeepTarget(aggressor);
    }

    public void TryRestrain(GameObject restrainer) {
        GameObject player = restrainPlayer.GetRestrainedPlayer();
        player.GetComponent<PlayerHit>().SetRestrainer(restrainer);

        RestoreDefaultBehavior();

        if (handleHitEffetCo != null) EndHandleHitEffect();

        StartCoroutine(TurnCollisionOff(player));
    }

    public GameObject GetRestrainedPlayer() {
        return restrainPlayer.GetRestrainedPlayer();
    }

    public bool TryDealMeleeDamages(
        Int16 contactDamage,
        Vector2 hostilePosition,
        float kbThrust = 0,
        float kbDuration = 0
    ) {
        if (!restrainPlayer.IsHoldingTarget()) return false;
        restrainPlayer.GetRestrainedPlayer().GetComponent<PlayerHit>().GetHit(contactDamage, hostilePosition);
        return false;
    }

    public void DirectKnockback(Vector3 argEnemyPosition, float duration, float thrust) {
        mainEF.DirectKnockback(argEnemyPosition, duration, thrust);
    }
    // end IHandleHostileContact methods

    public void CustomEffect(GameObject player) {
        if (player.GetComponent<PlayerInventory>().rupees > 0) {
            player.GetComponent<PlayerInventory>().UpdateRupees(-1);
        } else if (player.GetComponent<PlayerSpell>().manaPoints > 0) {
            player.GetComponent<PlayerSpell>().RemoveMana(2);
        } else {
            restrainPlayer.RegainStrength();
            animatorHandler.HandleSwallow(true);
        }
    }

    public void CustomHit(GameObject player) {
        player.GetComponent<PlayerHit>().isImmune = true;
        handleHitEffetCo = StartCoroutine(HandleHitEffet(player));
    }

    private IEnumerator HandleHitEffet(GameObject player, float duration = 2f) {
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
        restrainPlayer.GetRestrainedPlayer().GetComponent<PlayerHit>().isImmune = false;
        EndHandleHitEffect();
    }

    public void EndHandleHitEffect() {
        StopCoroutine(handleHitEffetCo);
        handleHitEffetCo = null;
        srd.enabled = true;
    }

    public bool CanKeepTarget(GameObject otherRestrainer) {
        return restrainPlayer.CanKeepTargetByDefault(otherRestrainer.GetComponent<RestrainPlayer>().restrainerPriority);
    }

    public void End() {
        GameObject player = restrainPlayer.GetRestrainedPlayer();

        RestoreDefaultBehavior();

        player.GetComponent<SpriteRenderer>().enabled = true;
        player.GetComponent<PlayerHit>().CleanRestrainer();
        player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;

        if (handleHitEffetCo != null) EndHandleHitEffect();
        player.GetComponent<PlayerHit>().GiveImmunity();

        if (gameObject.activeSelf)
            StartCoroutine(TurnCollisionOff(player));
    }

    public void RestoreDefaultBehavior() {
        animatorHandler.HandleAttack(false);
        animatorHandler.HandleSwallow(false);
        main.SelfState = EnemyState.Idle;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        restrainPlayer.ClearRestrainedPlayer();
    }

    private IEnumerator TurnCollisionOff(GameObject player, float duration = 1.5f) {
        Physics2D.IgnoreCollision(cld, player.GetComponent<Collider2D>(), true);
        yield return new WaitForSeconds(duration);
        Physics2D.IgnoreCollision(cld, player.GetComponent<Collider2D>(), false);
    }
}
