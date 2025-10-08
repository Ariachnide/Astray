using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MovementEnemyFactory))]
public class MoveTowardsTargetEnemyAggro : DefaultEnemyAggro {
    public float
        chaseOutsideAreaDuration,
        delayEndMoveTowardsTarget,
        speedModifierOnAggro = 1f;
    private Coroutine
        keepMovingTowardsTargetCo = null,
        loadMoveTowardsTargetCo = null,
        handleDelayEndMoveTowardsTargetCo = null;
    private bool isMovingAggressively;
    protected float
        moveTowardsTargetPauseDuration,
        moveTowardsTargetPauseTime;
    public bool
        isMoveTowardsTargetOnPause,
        allowAggroOutsideRMArea;
    public float loadingMoveTime;
    protected Transform target = null;
    private Vector2 targetDirection = Vector2.zero;
    private float tempMoveSpeed;

    private void FixedUpdate() {
        if (hasAnimatorEF && target != null) {
            targetDirection = target.position - transform.position;
            animatorEF.ChangeVisionDirection(targetDirection.normalized);
        }
    }

    public override void SetAggressiveBehaviour(Transform target) {
        base.SetAggressiveBehaviour();
        if (target == null ) throw new ArgumentException("Parameter cannot be null", nameof(target));
        if (
            keepMovingTowardsTargetCo == null
            && !isMovingAggressively
            && (
                !hasEnemyRM
                || allowAggroOutsideRMArea
                || (hasEnemyRM && enemyRM.CheckIfStillInRMArea())
            ) && gameObject.activeSelf
        )
            keepMovingTowardsTargetCo = StartCoroutine(KeepMovingTowardsTarget(target));
    }

    public override void EndAggressiveBehaviour() {
        base.EndAggressiveBehaviour();
        EndLoadMoveTowardsTarget();
        EndKeepMovingTowardsTarget(isDelayed: true);
    }

    public override void ResetAggro() {
        base.ResetAggro();
        EndKeepMovingTowardsTarget();
    }

    public virtual void DelayMoveTowardsTarget(float duration) {
        if (!isMovingAggressively) return;
        moveTowardsTargetPauseTime = Time.time;
        isMoveTowardsTargetOnPause = true;
        moveTowardsTargetPauseDuration = duration;
    }

    protected virtual void ResetDelay() {
        isMoveTowardsTargetOnPause = false;
        moveTowardsTargetPauseDuration = 0f;
        moveTowardsTargetPauseTime = 0f;
    }

    protected virtual Vector2 ProcessDirectionToTarget() {
        return (target.position - transform.position).normalized;
    }

    protected virtual IEnumerator KeepMovingTowardsTarget(Transform argTarget) {
        isMovingAggressively = true;
        movementEF.HandleRegularMoves(false);
        target = argTarget;

        if (handleDelayEndMoveTowardsTargetCo != null) EndHandleDelayEndMoveTowardsTargetCo();
        
        loadMoveTowardsTargetCo = StartCoroutine(LoadMoveTowardsTarget());
        while (loadMoveTowardsTargetCo != null) yield return null;
        
        float adjustTargetCD = 0f;
        float timeOutsideAreaCounter = 0f;
        float pauseTimer = 0f;
        bool countTimeOutsideArea = false;
        bool hasSetUpPause = false;

        if (speedModifierOnAggro > 1.01f || speedModifierOnAggro < 0.99f) {
            tempMoveSpeed = movementEF.speed;
            movementEF.speed = tempMoveSpeed * speedModifierOnAggro;
        }

        while (isMovingAggressively) {
            if (isMoveTowardsTargetOnPause) {

                if (!hasSetUpPause) {
                    pauseTimer = Time.time + moveTowardsTargetPauseDuration;
                    movementEF.movement = Vector2.zero;
                    timeOutsideAreaCounter += moveTowardsTargetPauseDuration;
                    hasSetUpPause = true;
                }

                if (Time.time >= pauseTimer) {
                    hasSetUpPause = false;
                    pauseTimer = 0f;
                    ResetDelay();
                }

            } else {

                if (Time.time > adjustTargetCD) {
                    movementEF.movement = ProcessDirectionToTarget();
                    adjustTargetCD = Time.time + 0.2f;
                }

                if (hasEnemyRM) {
                    if (enemyRM.CheckIfStillInRMArea()) {
                        if (countTimeOutsideArea) {
                            timeOutsideAreaCounter = 0f;
                            countTimeOutsideArea = false;
                        }
                    } else {
                        if (!countTimeOutsideArea) {
                            timeOutsideAreaCounter = Time.time + chaseOutsideAreaDuration;
                            countTimeOutsideArea = true;
                        } else if (Time.time > timeOutsideAreaCounter) {
                            foreach (Transform area in visionAreas)
                                if (area.GetComponent<ExtendingVisionPlayerReaction>() != null)
                                    area.GetComponent<ExtendingVisionPlayerReaction>().ForceRestoreDefaultRadius();
                            enemyRM.ReturnToRMArea();
                            EndKeepMovingTowardsTarget(
                                allowRegularMoves: false,
                                resetPosition: false,
                                isDelayed: true
                            );
                        }
                    }
                }

            }

            yield return null;
        }

        EndKeepMovingTowardsTarget();
    }

    protected virtual IEnumerator LoadMoveTowardsTarget() {
        yield return new WaitForSeconds(loadingMoveTime);
        if (Time.time < (moveTowardsTargetPauseTime + moveTowardsTargetPauseDuration))
            yield return new WaitForSeconds(moveTowardsTargetPauseTime + moveTowardsTargetPauseDuration - Time.time);
        EndLoadMoveTowardsTarget();
    }

    protected void EndLoadMoveTowardsTarget() {
        if (loadMoveTowardsTargetCo != null) {
            StopCoroutine(loadMoveTowardsTargetCo);
            loadMoveTowardsTargetCo = null;

            ResetDelay();
        }
    }

    public virtual void EndKeepMovingTowardsTarget(
        bool allowRegularMoves = true,
        bool resetPosition = true,
        bool isDelayed = false
    ) {
        if (keepMovingTowardsTargetCo != null) {
            StopCoroutine(keepMovingTowardsTargetCo);
            keepMovingTowardsTargetCo = null;

            if (resetPosition) movementEF.movement = Vector2.zero;

            target = null;
            targetDirection = Vector2.zero;
            if ((speedModifierOnAggro > 1.01f || speedModifierOnAggro < 0.99f) && tempMoveSpeed > 0f) {
                movementEF.speed = tempMoveSpeed;
                tempMoveSpeed = 0f;
            }

            if (isDelayed && delayEndMoveTowardsTarget > 0.1f) {
                StartCoroutine(HandleDelayEndMoveTowardsTarget(allowRegularMoves));
            } else {
                ConcludeMoveTowardsTarget(allowRegularMoves);
            }
        }
    }

    protected void ConcludeMoveTowardsTarget(bool allowRegularMoves) {
        isMovingAggressively = false;
        if (allowRegularMoves) movementEF.HandleRegularMoves(true);

        ResetDelay();
    }

    protected virtual IEnumerator HandleDelayEndMoveTowardsTarget(bool allowRegularMoves) {
        yield return new WaitForSeconds(delayEndMoveTowardsTarget);
        if (Time.time < (moveTowardsTargetPauseTime + moveTowardsTargetPauseDuration))
            yield return new WaitForSeconds(moveTowardsTargetPauseTime + moveTowardsTargetPauseDuration - Time.time);
        ConcludeMoveTowardsTarget(allowRegularMoves);
        EndHandleDelayEndMoveTowardsTargetCo();
    }

    protected void EndHandleDelayEndMoveTowardsTargetCo() {
        if (handleDelayEndMoveTowardsTargetCo != null) {
            StopCoroutine(handleDelayEndMoveTowardsTargetCo);
            handleDelayEndMoveTowardsTargetCo = null;

            isMoveTowardsTargetOnPause = false;
            moveTowardsTargetPauseDuration = 0f;
            moveTowardsTargetPauseTime = 0f;
        }
    }
}
