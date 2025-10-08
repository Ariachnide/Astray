using UnityEngine;

public class GuardSwordsmanAnimator : AnimatorEnemyFactory {
    public override void Move(bool isMoving) {
        animator.SetBool("moving", isMoving);
    }

    public override void ChangeVisionDirection(Vector2 direction = new Vector2()) {
        animator.SetFloat("moveX", direction.x);
        animator.SetFloat("moveY", direction.y);
    }

    public override void Aggressivity(bool isAggressive) {
        animator.SetBool("isAggro", isAggressive);
    }

    public void Curse(bool isCursed) {
        animator.SetBool("isCursed", isCursed);
    }
}
