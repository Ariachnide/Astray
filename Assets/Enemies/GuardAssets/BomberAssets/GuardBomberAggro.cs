using System.Collections;
using UnityEngine;

public class GuardBomberAggro : MoveTowardsTargetEnemyAggro, ILauchDirection {
    [SerializeField] private float range;
    private float targetDist, timer = 0f;
    private Vector3 currentDirection = Vector3.zero;
    private bool isPositionFixed;
    private Coroutine holdFixedPositionCo = null;

    protected override Vector2 ProcessDirectionToTarget() {
        if (holdFixedPositionCo != null) return Vector3.zero;
        if (timer > Time.time) return currentDirection;
        timer = Time.time + 0.2f;

        targetDist = Vector2.Distance(target.position, transform.position);

        if (targetDist > range) {
            currentDirection = target.position;
        } else if (targetDist < (range / 2)) {
            currentDirection = -target.position;
        } else {
            HandleFixPosition(true);
            return Vector3.zero;
        }
        return (currentDirection - transform.position).normalized;
    }

    public Vector2 GetLauchDirection() {
        return (target.position - transform.position).normalized;
    }

    private void HandleFixPosition(bool isPositionFixed) {
        if (this.isPositionFixed == isPositionFixed) return;
        this.isPositionFixed = isPositionFixed;
        attackEF.canAttack = isPositionFixed;
        if (isPositionFixed && (holdFixedPositionCo == null)) holdFixedPositionCo = StartCoroutine(HoldFixedPosition());
    }

    private IEnumerator HoldFixedPosition() {
        yield return new WaitForSeconds(GetComponent<LaunchBombAttack>().bombCountDown + 0.8f);
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
        targetDist = timer = 0f;
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
