using UnityEngine;

public abstract class AnimatorEnemyAbstractClass : MonoBehaviour {
    public abstract void Move(bool isMoving);
    public abstract void ChangeVisionDirection(Vector2 direction);
    public abstract void Aggressivity(bool isAggressive);
    public abstract void Attack(bool isAttacking);
    public abstract void Knockback(bool isKnockbacked);
    public abstract void Spin(float globalTimer);
    public abstract void EndSpinning();
    public abstract void Stun(bool isStunned);
}

public class AnimatorEnemyFactory : AnimatorEnemyAbstractClass {
    protected Animator animator;

    protected virtual void Awake() {
        animator = GetComponent<Animator>();
        animator.keepAnimatorStateOnDisable = true;
    }

    public override void Move(bool isMoving) {}
    public override void ChangeVisionDirection(Vector2 direction = new Vector2()) {}
    public override void Aggressivity(bool isAggressive) {}
    public override void Attack(bool isAttacking) {}
    public override void Knockback(bool isKnockbacked) {}
    public override void Spin(float globalTimer) {}
    public override void EndSpinning() {}
    public override void Stun(bool isStunned) {}
}
