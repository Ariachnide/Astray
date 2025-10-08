public class SkullheadAnimator : AnimatorEnemyFactory {
    public override void Move(bool isMoving) {
        animator.SetBool("moving", isMoving);
    }
}
