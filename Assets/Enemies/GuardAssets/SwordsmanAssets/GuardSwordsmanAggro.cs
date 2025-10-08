using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AnimatorEnemyFactory))]
public class GuardSwordsmanAggro : MoveTowardsTargetEnemyAggro {
    [SerializeField]
    private GameObject swordDamageAreaGo;
    private SwordDamageArea swordDamageArea;

    protected override void Awake() {
        base.Awake();
        swordDamageArea = swordDamageAreaGo.GetComponent<SwordDamageArea>();
    }

    protected override void LateUpdate() {
        base.LateUpdate();
        if (main.SelfState != EnemyState.Knockbacked && isAggressive)
            if (swordDamageArea.SwordDirection != visionDirection)
                swordDamageArea.SetArea(visionDirection);
    }

    protected override IEnumerator LoadMoveTowardsTarget() {
        yield return new WaitForSeconds(loadingMoveTime / 2);
        HandleSword(true);
        yield return new WaitForSeconds(loadingMoveTime / 2);

        if (Time.time < (moveTowardsTargetPauseTime + moveTowardsTargetPauseDuration))
            yield return new WaitForSeconds(moveTowardsTargetPauseTime + moveTowardsTargetPauseDuration - Time.time);

        EndLoadMoveTowardsTarget();
    }

    public override void ResetAggro() {
        base.ResetAggro();
        HandleSword(false);
    }

    protected override IEnumerator HandleDelayEndMoveTowardsTarget(bool allowRegularMoves) {
        yield return new WaitForSeconds(delayEndMoveTowardsTarget / 2);
        HandleSword(false);
        yield return new WaitForSeconds(delayEndMoveTowardsTarget / 2);

        if (Time.time < (moveTowardsTargetPauseTime + moveTowardsTargetPauseDuration))
            yield return new WaitForSeconds(moveTowardsTargetPauseTime + moveTowardsTargetPauseDuration - Time.time);

        ConcludeMoveTowardsTarget(allowRegularMoves);
    }

    private void HandleSword(bool isAggressiveArg) {
        isAggressive = isAggressiveArg;
        animatorEF.Aggressivity(isAggressive);
        if ((isAggressive && !swordDamageAreaGo.activeSelf) || (!isAggressive && swordDamageAreaGo.activeSelf))
            swordDamageAreaGo.SetActive(isAggressive);
    }

    public override void EndKeepMovingTowardsTarget(
        bool allowRegularMoves = true,
        bool resetPosition = true,
        bool isDelayed = false
    ) {
        base.EndKeepMovingTowardsTarget(allowRegularMoves, resetPosition, isDelayed);
        if (!isDelayed) HandleSword(false);
    }
}
