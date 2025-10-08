using System.Collections;
using UnityEngine;

public class GuardArcherAggro : MoveTowardsTargetEnemyAggro {
    [SerializeField] private float range;
    private float tempDistX, tempDistY, timer = 0f;
    private Vector3 currentDirection = Vector3.zero;
    private bool isPositionFixed;
    private Coroutine holdFixedPositionCo = null;

    protected override Vector2 ProcessDirectionToTarget() {
        if (holdFixedPositionCo != null) return Vector3.zero;
        if (timer > Time.time) return currentDirection;
        timer = Time.time + 0.2f;

        tempDistX = Mathf.Abs(target.position.x - transform.position.x);
        tempDistY = Mathf.Abs(target.position.y - transform.position.y);
        if (tempDistX <= tempDistY) {
            if (tempDistX < 0.4f) {
                HandleFixPosition(true);
                return Vector3.zero;
            }

            currentDirection.x = target.position.x;
            if (tempDistY > range) {
                if (transform.position.y <= target.position.y) {
                    currentDirection.y = target.position.y - range;
                } else {
                    currentDirection.y = target.position.y + range;
                }
            } else if (tempDistY < (range / 2)) {
                if (transform.position.y <= target.position.y) {
                    currentDirection.y = target.position.y - range;
                } else {
                    currentDirection.y = target.position.y + range;
                }
            } else {
                currentDirection.y = transform.position.y;
            }
        } else {
            if (tempDistY < 0.4f) {
                HandleFixPosition(true);
                return Vector3.zero;
            }

            currentDirection.y = target.position.y;
            if (tempDistX > range) {
                if (transform.position.x <= target.position.x) {
                    currentDirection.x = target.position.x - range;
                } else {
                    currentDirection.x = target.position.x + range;
                }
            } else if (tempDistX < (range / 2)) {
                if (transform.position.x <= target.position.x) {
                    currentDirection.x = target.position.x - range;
                } else {
                    currentDirection.x = target.position.x + range;
                }
            } else {
                currentDirection.x = transform.position.x;
            }
        }

        return (currentDirection - transform.position).normalized;
    }

    private void HandleFixPosition(bool isPositionFixed) {
        if (this.isPositionFixed == isPositionFixed) return;
        this.isPositionFixed = isPositionFixed;
        attackEF.canAttack = isPositionFixed;
        if (isPositionFixed && (holdFixedPositionCo == null)) holdFixedPositionCo = StartCoroutine(HoldFixedPosition());
    }

    private IEnumerator HoldFixedPosition() {
        yield return new WaitForSeconds(attackCoolDown);
        EndHoldFixedPosition();
    }

    private void EndHoldFixedPosition() {
        if (holdFixedPositionCo != null) {
            StopCoroutine(holdFixedPositionCo);
            holdFixedPositionCo = null;
        }
        HandleFixPosition(false);
    }

    private void ResetTempStats() {
        currentDirection = Vector3.zero;
        tempDistX = tempDistY = timer = 0f;
    }

    public override void EndKeepMovingTowardsTarget(
        bool allowRegularMoves = true,
        bool resetPosition = true,
        bool isDelayed = false
    ) {
        base.EndKeepMovingTowardsTarget(allowRegularMoves, resetPosition, isDelayed);
        ResetTempStats();
    }
}
