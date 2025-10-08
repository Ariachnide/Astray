using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultEnemyAggro : AggroEnemyFactory {
    [SerializeField]
    protected List<Transform> visionAreas;
    protected Coroutine
        attackLoadingCo = null,
        handleCoolDownCo = null;

    protected virtual void Start() {
        AdjustVisionDirection();
        SetVisionArea(visionDirection);
    }

    protected virtual void LateUpdate() {
        if (main.SelfState != EnemyState.Knockbacked)
            if (isVisionBasedOnSprite && visionDirection != GetVisionDirectionFromSprite()) {
                VisionDirection currentSpriteVD = GetVisionDirectionFromSprite();
                SetVisionArea(currentSpriteVD);
                visionDirection = currentSpriteVD;
            }
    }

    public override void SetAggressiveBehaviour(Transform target = null) {
        if (
            hasAttackEF
            && attackEF.canAttack
            && !isAggressive
            && !isOnCooldown
            && gameObject.activeSelf
        )
            attackLoadingCo = StartCoroutine(AttackLoading());
    }

    public override void EndAggressiveBehaviour() {
        if (isAggressive) EndAttackLoading();
    }

    public override void CheckAggressiveBehaviourVision() {
        Transform target = null;
        
        foreach (Transform va in visionAreas)
            if (!target && va.GetComponent<DefaultVisionPlayerReaction>().SavedTarget != null)
                target = va.GetComponent<DefaultVisionPlayerReaction>().SavedTarget;
        
        if (target != null && !isAggressive) {
            SetAggressiveBehaviour(target);
        } else if (target == null) {
            EndAggressiveBehaviour();
        }
    }

    public override void ResetAggro() {
        EndAttackLoading();
        EndHandleCoolDown();
        if (hasAttackEF && attackEF.isAttacking) attackEF.Interrupt();
    }

    protected override void AdjustVisionDirection() {
        if (!isVisionDirectional) return;
        switch (visionDirection) {
            case VisionDirection.Right:
                animatorEF.ChangeVisionDirection(Vector2.right);
                break;
            case VisionDirection.Left:
                animatorEF.ChangeVisionDirection(Vector2.left);
                break;
            case VisionDirection.Up:
                animatorEF.ChangeVisionDirection(Vector2.up);
                break;
            case VisionDirection.Down:
                animatorEF.ChangeVisionDirection(Vector2.down);
                break;
            default:
                Debug.LogError($"UNHANDLED VISION DIRECTION VALUE: {visionDirection}");
                break;
        }
    }

    protected override void SetVisionArea(VisionDirection vd) {
        if (!isVisionDirectional) return;
        switch (vd) {
            case VisionDirection.Right:
                foreach (Transform va in visionAreas)
                    if (va.GetComponent<VisionEnemyFactory>() != null)
                        va.GetComponent<VisionEnemyFactory>().SetVisionRight();
                break;
            case VisionDirection.Left:
                foreach (Transform va in visionAreas)
                    if (va.GetComponent<VisionEnemyFactory>() != null)
                        va.GetComponent<VisionEnemyFactory>().SetVisionLeft();
                break;
            case VisionDirection.Up:
                foreach (Transform va in visionAreas)
                    if (va.GetComponent<VisionEnemyFactory>() != null)
                        va.GetComponent<VisionEnemyFactory>().SetVisionUp();
                break;
            case VisionDirection.Down:
                foreach (Transform va in visionAreas)
                    if (va.GetComponent<VisionEnemyFactory>() != null)
                        va.GetComponent<VisionEnemyFactory>().SetVisionDown();
                break;
            default:
                Debug.LogError($"UNHANDLED VISION DIRECTION VALUE: {visionDirection}");
                break;
        }
    }

    protected override VisionDirection GetVisionDirectionFromSprite() {
        foreach (SpritesRelatedToDirection element in spritesRelatedToDirection)
            if (element.sprites.Find(sprite => sprite.name == srd.sprite.name))
                return element.direction;
        Debug.LogError($"IN ENEMY {gameObject.name} UNABLE TO FIND VISION DIRECTION IN SPRITE NAMED: {srd.sprite.name}");
        return VisionDirection.None;
    }

    public override void DelayAggressiveBehaviour(float duration) {
        isAggressiveBehaviourOnPause = true;
        attackPauseDuration = duration;
    }

    protected override void CallAttack() {
        if (attackEF.canAttack) {
            attackEF.Execute();
            handleCoolDownCo = StartCoroutine(HandleCoolDown());
        }
    }

    public override void HandleVisionStatus(bool shouldActivate) {
        foreach (Transform area in visionAreas) area.gameObject.SetActive(shouldActivate);
    }

    protected IEnumerator AttackLoading() {
        isAggressive = true;
        if (attackLoadingRange > 0) {
            float timer = Time.time + Random.Range(
                attackMinimumLoadingRange > 0 ? attackMinimumLoadingRange : 0,
                attackLoadingRange
            );
            bool hasHandledPause = false;
            while (Time.time < timer) {
                if (isAggressiveBehaviourOnPause && !hasHandledPause) {
                    timer += attackPauseDuration;
                    hasHandledPause = true;
                }
                yield return null;
            }
        } else {
            yield return null;
        }
        if (hasAttackEF && attackEF.canAttack) CallAttack();
        EndAttackLoading();
    }

    protected void EndAttackLoading() {
        if (attackLoadingCo != null) StopCoroutine(attackLoadingCo);
        attackLoadingCo = null;
        isAggressive = false;
        isAggressiveBehaviourOnPause = false;
        attackPauseDuration = 0f;
    }

    protected IEnumerator HandleCoolDown() {
        isOnCooldown = true;
        if (attackCoolDownRange > 0) {
            yield return new WaitForSeconds(attackCoolDown + Random.Range(0f, attackCoolDownRange));
        } else {
            if (attackCoolDown > 0) {
                yield return new WaitForSeconds(attackCoolDown);
            } else {
                yield return null;
            }
        }
        EndHandleCoolDown();
    }

    protected void EndHandleCoolDown() {
        if (handleCoolDownCo != null) StopCoroutine(handleCoolDownCo);
        handleCoolDownCo = null;
        isOnCooldown = false;
    }
}
