using System;
using UnityEngine;

[RequireComponent(typeof(MainEnemyFactory))]
public abstract class AttackEnemyAbstractClass : MonoBehaviour {
    public Int16 attackDmg = 1;
    public bool
        canAttack = true,
        isAttacking = false;

    public abstract void Execute();
    public abstract void Interrupt();
    public abstract void HandleAttackPermission(bool allow);
}

public class AttackEnemyFactory : AttackEnemyAbstractClass {
    protected AggroEnemyFactory aggroEF;
    protected bool hasAggroEF;
    protected MovementEnemyFactory movementEF;
    protected bool hasMovementEF;
    protected AnimatorEnemyFactory animatorEF;
    protected bool hasAnimatorEF;

    protected virtual void Awake() {
        aggroEF = GetComponent<AggroEnemyFactory>();
        hasAggroEF = GetComponent<AggroEnemyFactory>() != null;
        movementEF = GetComponent<MovementEnemyFactory>();
        hasMovementEF = GetComponent<MovementEnemyFactory>() != null;
        animatorEF = GetComponent<AnimatorEnemyFactory>();
        hasAnimatorEF = GetComponent<AnimatorEnemyFactory>() != null;
    }

    public override void Execute() {
        throw new NotImplementedException();
    }

    public override void Interrupt() {}

    public override void HandleAttackPermission(bool allow) {
        throw new NotImplementedException();
    }
}
