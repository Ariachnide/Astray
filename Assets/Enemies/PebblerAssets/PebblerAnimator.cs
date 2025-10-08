using UnityEngine;

public class PebblerAnimator : AnimatorEnemyFactory {
    public override void Move(bool isMoving) {
        animator.SetBool("moving", isMoving);
    }

    public override void ChangeVisionDirection(Vector2 direction = new Vector2()) {
        animator.SetFloat("moveX", direction.x);
        animator.SetFloat("moveY", direction.y);
    }
}
