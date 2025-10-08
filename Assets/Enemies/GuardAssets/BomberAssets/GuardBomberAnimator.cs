using UnityEngine;

public class GuardBomberAnimator : AnimatorEnemyFactory {
    public override void Move(bool isMoving) {
        animator.SetBool("moving", isMoving);
    }

    public override void ChangeVisionDirection(Vector2 direction = new Vector2()) {
        animator.SetFloat("moveX", direction.x);
        animator.SetFloat("moveY", direction.y);
    }

    public override void Attack(bool isAttacking) {
        animator.SetBool("isAttacking", isAttacking);
    }

    public void Curse(bool isCursed) {
        animator.SetBool("isCursed", isCursed);
    }
}
