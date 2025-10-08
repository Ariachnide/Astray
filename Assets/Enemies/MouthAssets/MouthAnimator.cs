public class MouthAnimator : AnimatorEnemyFactory {
    public void HandleAttack(bool isAttacking) {
        animator.SetBool("isAttacking", isAttacking);
    }

    public void HandleSwallow(bool isSwallowing) {
        animator.SetBool("isSwallowing", isSwallowing);
    }

    public bool IsAttacking() {
        return animator.GetBool("isAttacking");
    }

    public bool IsSwallowing() {
        return animator.GetBool("isSwallowing");
    }

    public override void Stun(bool isStunned) {
        if (isStunned) {
            animator.speed = 0;
        } else {
            animator.speed = 1;
        }
    }
}
