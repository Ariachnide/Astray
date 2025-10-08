using System;
using UnityEngine;

public enum MoveDirections {
    None,
    Right,
    Left,
    Up,
    Down,
    TopLeft,
    TopRight,
    BottomLeft,
    BottomRight
}

[RequireComponent(typeof(MainEnemyFactory))]
public abstract class MovementEnemyAbstractClass : MonoBehaviour {
    protected MainEnemyFactory main;
    protected Rigidbody2D rb;
    public Vector3 movement = Vector3.zero;
    public float speed = 3f;
    public bool
        canMove = true,
        isSpinning = false;
    
    protected AnimatorEnemyFactory animatorEF;
    protected AggroEnemyFactory aggroEF;
    protected AttackEnemyFactory attackEF;
    protected bool hasAnimator, hasAggroEF, hasAttackEF;

    protected abstract void HandleMove();
    public abstract void HandleMobility(bool allow);
    public abstract void HandleRegularMoves(bool allow);
    public abstract void InterruptMoves();
    public abstract void Spin(float globalTimer);
    public abstract void EndSpinning();
}

public class MovementEnemyFactory : MovementEnemyAbstractClass {
    protected virtual void Awake() {
        main = GetComponent<MainEnemyFactory>();
        rb = GetComponent<Rigidbody2D>();
        hasAnimator = GetComponent<AnimatorEnemyFactory>() != null;
        animatorEF = GetComponent<AnimatorEnemyFactory>();
        hasAggroEF = GetComponent<AggroEnemyFactory>() != null;
        aggroEF = GetComponent<AggroEnemyFactory>();
        hasAttackEF = GetComponent<AttackEnemyFactory>() != null;
        attackEF = GetComponent<AttackEnemyFactory>();
    }

    protected override void HandleMove() {
        throw new NotImplementedException();
    }

    public override void HandleMobility(bool allow) {
        throw new NotImplementedException();
    }

    public override void HandleRegularMoves(bool allow) {
        throw new NotImplementedException();
    }

    public override void InterruptMoves() {
        throw new NotImplementedException();
    }

    public override void Spin(float globalTimer) {}

    public override void EndSpinning() {}
}
