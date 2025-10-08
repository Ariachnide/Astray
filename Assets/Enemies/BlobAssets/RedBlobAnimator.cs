public class RedBlobAnimator : AnimatorEnemyFactory {
    public override void Aggressivity(bool isAggressive) {
        if (isAggressive) {
            animator.speed = 1.5f;
        } else {
            animator.speed = 1;
        }
    }

    public override void Stun(bool isStunned) {
        if (isStunned) {
            animator.speed = 0;
        } else {
            animator.speed = 1;
        }
    }
}
