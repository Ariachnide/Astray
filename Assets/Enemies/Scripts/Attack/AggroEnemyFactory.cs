using System.Collections.Generic;
using System;
using UnityEngine;

public enum VisionDirection {
    None,
    Right,
    Left,
    Up,
    Down
}

[RequireComponent(typeof(MainEnemyFactory))]
public abstract class AggroEnemyAbstractClass : MonoBehaviour {
    public bool
        isVisionBasedOnSprite,
        isAggressive,
        isOnCooldown,
        isAggressiveBehaviourOnPause,
        isVisionDirectional;
    public float
        attackCoolDown,
        attackMinimumLoadingRange,
        attackLoadingRange,
        attackCoolDownRange;
    protected float attackPauseDuration;
    public VisionDirection visionDirection;
    [Serializable]
    protected struct SpritesRelatedToDirection {
        public VisionDirection direction;
        public List<Sprite> sprites;
    }
    [SerializeField]
    protected List<SpritesRelatedToDirection> spritesRelatedToDirection;

    public abstract void SetAggressiveBehaviour(Transform target = null);
    public abstract void EndAggressiveBehaviour();
    public abstract void CheckAggressiveBehaviourVision();
    public abstract void ResetAggro();
    protected abstract void AdjustVisionDirection();
    protected abstract void SetVisionArea(VisionDirection vd);
    protected abstract VisionDirection GetVisionDirectionFromSprite();
    public abstract void DelayAggressiveBehaviour(float duration);
    protected abstract void CallAttack();
    public abstract void HandleVisionStatus(bool shouldActivate);
}

public class AggroEnemyFactory : AggroEnemyAbstractClass {
    protected MainEnemyFactory main;
    protected SpriteRenderer srd;

    protected AnimatorEnemyFactory animatorEF;
    protected bool hasAnimatorEF;
    protected MovementEnemyFactory movementEF;
    protected bool hasMovementEF;
    protected AttackEnemyFactory attackEF;
    protected bool hasAttackEF;
    protected EnemyRandomMoves enemyRM;
    protected bool hasEnemyRM;

    protected virtual void Awake() {
        main = GetComponent<MainEnemyFactory>();
        srd = GetComponent<SpriteRenderer>();

        animatorEF = GetComponent<AnimatorEnemyFactory>();
        hasAnimatorEF = GetComponent<AnimatorEnemyFactory>() != null;
        movementEF = GetComponent<MovementEnemyFactory>();
        hasMovementEF = GetComponent<MovementEnemyFactory>() != null;
        attackEF = GetComponent<AttackEnemyFactory>();
        hasAttackEF = GetComponent<AttackEnemyFactory>() != null;
        enemyRM = GetComponent<EnemyRandomMoves>();
        hasEnemyRM = GetComponent<EnemyRandomMoves>() != null;
    }

    protected virtual void OnDisable() {
        ResetAggro();
    }

    public override void SetAggressiveBehaviour(Transform target = null) {
        throw new NotImplementedException();
    }

    public override void EndAggressiveBehaviour() {
        throw new NotImplementedException();
    }

    public override void CheckAggressiveBehaviourVision() {
        throw new NotImplementedException();
    }

    public override void ResetAggro() {
        throw new NotImplementedException();
    }

    protected override void AdjustVisionDirection() {
        if (isVisionDirectional) throw new NotImplementedException();
    }

    protected override void SetVisionArea(VisionDirection vd) {
        if (isVisionDirectional) throw new NotImplementedException();
    }

    protected override VisionDirection GetVisionDirectionFromSprite() {
        if (isVisionBasedOnSprite) throw new NotImplementedException();
        return VisionDirection.None;
    }

    public override void DelayAggressiveBehaviour(float duration) {
        throw new NotImplementedException();
    }

    protected override void CallAttack() {
        if (hasAttackEF) throw new NotImplementedException();
    }

    public override void HandleVisionStatus(bool shouldActivate) {
        throw new NotImplementedException();
    }
}
