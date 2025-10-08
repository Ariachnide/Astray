using UnityEngine;

public class TinyBlobAnimator : AnimatorEnemyFactory {
    protected SpriteRenderer srd;
    [SerializeField]
    private Sprite
        faceSpr,
        backSpr;
    private Sprite tempSprite = null;

    protected override void Awake() {
        base.Awake();
        srd = GetComponent<SpriteRenderer>();
    }

    public override void Knockback(bool isKnockbacked) {
        if (isKnockbacked) {
            tempSprite = srd.sprite;
            srd.sprite = backSpr;
            animator.speed = 0;
        } else {
            srd.sprite = tempSprite;
            tempSprite = null;
            animator.speed = 1;
        }
    }

    public override void Spin(float globalTimer) {
        animator.speed = 2.5f;
    }

    public override void EndSpinning() {
        animator.speed = 1;
    }

    public override void Stun(bool isStunned) {
        if (isStunned) {
            animator.speed = 0;
        } else {
            animator.speed = 1;
        }
    }
}
