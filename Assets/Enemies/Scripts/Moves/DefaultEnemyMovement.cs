using System;
using System.Collections;
using UnityEngine;

public class DefaultEnemyMovement : MovementEnemyFactory {
    protected Coroutine spinningCo = null;

    protected virtual void FixedUpdate() {
        if (main.SelfState != EnemyState.Knockbacked)
            HandleMove();
    }

    protected override void HandleMove() {
        if (movement != Vector3.zero) {
            if (canMove) {
                main.SelfState = EnemyState.Walk;
                rb.MovePosition(transform.position + Time.deltaTime * speed * movement.normalized);
                if (hasAnimator) animatorEF.Move(true);
            } else {
                main.SelfState = EnemyState.Idle;
                if (hasAnimator) animatorEF.Move(false);
            }
            if (hasAggroEF && main.isDirectional) {
                if (hasAnimator) animatorEF.ChangeVisionDirection(movement);
            }
        } else {
            main.SelfState = EnemyState.Idle;
            animatorEF.Move(false);
        }
    }

    public override void HandleMobility(bool allow) {
        if (allow) {
            main.SelfState = EnemyState.Idle;
            canMove = true;
        } else {
            if (main.SelfState != EnemyState.Immobilized) {
                main.SelfState = EnemyState.Immobilized;
                InterruptMoves();
                canMove = false;
                main.HandleKB();
            }
        }
        if (hasAttackEF) attackEF.HandleAttackPermission(allow);
    }

    public override void HandleRegularMoves(bool allow) {
        if (!allow) InterruptMoves();
    }

    public override void InterruptMoves() {
        movement = Vector3.zero;
    }

    public override void Spin(float globalTimer) {
        spinningCo = StartCoroutine(Spinning(globalTimer));
    }

    protected IEnumerator Spinning(float globalTimer) {
        Int16 count = -1;
        WaitForSeconds waiting = new(0.25f);
        while (Time.time < globalTimer) {
            count++;
            switch (count) {
                case 0:
                    movement = Vector3.down;
                    break;
                case 1:
                    movement = Vector3.left;
                    break;
                case 2:
                    movement = Vector3.up;
                    break;
                case 3:
                    movement = Vector3.right;
                    count = -1;
                    break;
            }
            yield return waiting;
        }
        EndSpinning();
    }

    public override void EndSpinning() {
        if (spinningCo != null) StopCoroutine(spinningCo);
        spinningCo = null;
    }
}
