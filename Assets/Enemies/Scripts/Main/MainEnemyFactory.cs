using System;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyState {
    Idle,
    Walk,
    Knockbacked,
    Immobilized
}

public class DefaultSettings {
    public VisionDirection visionDirection = VisionDirection.None;
    public Vector2 position;
};

public class LastPlayerAttackData {
    public GameObject player;
    public int wpnAttackId;
}

[RequireComponent(typeof(LootController), typeof(ActivityPermission))]
public abstract class MainEnemyAbstractClass :
        MonoBehaviour,
        IColliderRef,
        IHandleDefaultSettings,
        IGetHit,
        IHandleSpecialEffects {
    public virtual EnemyState SelfState { get; set; } = EnemyState.Idle;
    protected Rigidbody2D rb;
    protected SpriteRenderer srd;
    protected virtual GameObject WhirlGO { get; set; } = null;
    public Int16 hitPoints;
    public ColliderRef SelfColliderRef { get; protected set; } = ColliderRef.Enemy;
    public DefaultSettings SelfDefaultSettings { get; protected set; } = null;
    protected LootController lootController;
    [SerializeField] protected GameObject destructionAnimation;
    protected bool shouldCheckHP;
    public bool isDirectional;
    public bool canGetKnockedBack = true;
    public bool isSpinning;
    protected LastPlayerAttackData lastPlayerAttackData = new();

    protected AggroEnemyFactory aggroEF;
    protected bool hasAggroEF;
    protected DefaultEnemyCollision defaultEnemyCollision;
    protected bool hasDefaultCollision;
    protected DefaultEnemyTrigger defaultEnemyTrigger;
    protected bool hasDefaultTrigger;
    protected AnimatorEnemyFactory animatorEF;
    protected bool hasAnimatorEF;
    protected MovementEnemyFactory movementEF;
    protected bool hasMovementEF;

    public abstract void HandleSummoning(Transform summoner, List<StoryState> storyStates);
    public abstract void ResetPosition();
    public abstract void GetDamage(Int16 dmg);
    public abstract void UpdateLastBlowGiver(GameObject blowGiver);
    public abstract void GetHit(
        Int16 damage,
        GameObject blowGiver,
        Vector3 blowGiverPosition,
        float kbThrust,
        float kbDuration,
        string weaponName = ""
    );
    public abstract void DirectKnockback(Vector3 argPosition, float duration, float thrust);
    public abstract void EndKnockback(EnemyState newState = EnemyState.Idle, bool allowMoves = true);
    public abstract void Blink(float duration);
    protected abstract void EndBlinking();
    public abstract void CheckHP(bool animate = true);
    public abstract void SetSpecialEffect(
        SpecialEffectType effect,
        float duration,
        GameObject effectGO = null
    );
    public abstract void EndSpecialEffect(SpecialEffectType effect);
    protected abstract void HandleStatusOnHit();
    public abstract void EliminateStatusObjects();
    public abstract void HandleKB();
    public abstract void Spin(float duration);
    public abstract void EndSpinning();
}

public class MainEnemyFactory : MainEnemyAbstractClass {
    protected virtual void Awake() {
        rb = GetComponent<Rigidbody2D>();
        srd = GetComponent<SpriteRenderer>();
        lootController = GetComponent<LootController>();

        aggroEF = GetComponent<AggroEnemyFactory>();
        hasAggroEF = GetComponent<AggroEnemyFactory>() != null;
        defaultEnemyCollision = GetComponent<DefaultEnemyCollision>();
        hasDefaultCollision = GetComponent<DefaultEnemyCollision>() != null;
        defaultEnemyTrigger = GetComponent<DefaultEnemyTrigger>();
        hasDefaultTrigger = GetComponent<DefaultEnemyTrigger>() != null;
        animatorEF = GetComponent<AnimatorEnemyFactory>();
        hasAnimatorEF = GetComponent<AnimatorEnemyFactory>() != null;
        movementEF = GetComponent<MovementEnemyFactory>();
        hasMovementEF = GetComponent<MovementEnemyFactory>() != null;

        if (SelfDefaultSettings == null)
            SelfDefaultSettings = new DefaultSettings() {
                position = transform.position,
                visionDirection = hasAggroEF
                    ? aggroEF.visionDirection
                    : VisionDirection.None
            };
    }

    public override void HandleSummoning(Transform summoner, List<StoryState> storyStates) {
        transform.SetParent(summoner.transform.parent);
        GetComponent<IHandleActivationPermission>().SetPermission(storyStates, true);
    }

    public override void ResetPosition() {
        transform.position = SelfDefaultSettings.position;
        if (hasAggroEF) aggroEF.visionDirection = SelfDefaultSettings.visionDirection;
    }

    public override void GetDamage(Int16 dmg) {
        throw new NotImplementedException();
    }

    public override void UpdateLastBlowGiver(GameObject blowGiver) {
        throw new NotImplementedException();
    }

    public override void GetHit(
        Int16 damage,
        GameObject blowGiver,
        Vector3 blowGiverPosition,
        float kbThrust,
        float kbDuration,
        string weaponName = ""
    ) {
        throw new NotImplementedException();
    }

    public override void DirectKnockback(Vector3 argPosition, float duration, float thrust) {
        throw new NotImplementedException();
    }

    public override void EndKnockback(EnemyState newState = EnemyState.Idle, bool allowMoves = true) {
        throw new NotImplementedException();
    }

    public override void Blink(float duration) {
        throw new NotImplementedException();
    }

    protected override void EndBlinking() {
        throw new NotImplementedException();
    }

    public override void CheckHP(bool animate = true) {
        throw new NotImplementedException();
    }

    public override void SetSpecialEffect(
        SpecialEffectType effect,
        Single duration,
        GameObject effectGO = null
    ) {
        throw new NotImplementedException();
    }

    public override void EndSpecialEffect(SpecialEffectType effect) {
        throw new NotImplementedException();
    }

    protected override void HandleStatusOnHit() {
        throw new NotImplementedException();
    }

    public override void EliminateStatusObjects() {
        throw new NotImplementedException();
    }

    public override void HandleKB() {
        throw new NotImplementedException();
    }

    public override void Spin(float duration) {
        throw new NotImplementedException();
    }

    public override void EndSpinning() {
        throw new NotImplementedException();
    }
}
