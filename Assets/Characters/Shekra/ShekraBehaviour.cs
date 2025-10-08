using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ShekraAttackMove {
    Attack1,
    Attack2,
    ClockwiseAttack,
    CounterclockwiseAttack
}

public class ShekraBehaviour : MonoBehaviour {
    public float speed;
    private bool hasExecutedOrder, interruptAttack = false;
    private Animator animator;
    private Rigidbody2D rb;
    private WaitForFixedUpdate waitFFU = new WaitForFixedUpdate();
    public Coroutine handleMoveCharacterCo { get; private set; } = null;
    private Coroutine handleAttackCo = null;
    [SerializeField] private GameObject threatArea;
    public List<int> hitTargetList = new List<int>();

    private void Awake() {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void MoveCharacter(
        Vector2 destination,
        CharDirection moveDirection,
        bool changeDirection = true
    ) {
        if (hasExecutedOrder) hasExecutedOrder = false;
        if (changeDirection) SetAnimatorDirection(moveDirection);
        handleMoveCharacterCo = StartCoroutine(HandleMoveCharacter(destination, moveDirection));
    }

    public void EndHandleMoveCharacter() {
        if (handleMoveCharacterCo != null) StopCoroutine(handleMoveCharacterCo);
        animator.SetBool("isMoving", false);
        handleMoveCharacterCo = null;
    }

    public void TurnCharacter(CharDirection moveDirection) {
        SetAnimatorDirection(moveDirection);
    }

    public void SetArm(bool wearArm) {
        animator.SetBool("isArmed", wearArm);
    }

    public void Attack(
        ShekraAttackMove attackMove,
        Vector3 targetPosition,
        float duration,
        float loadingTime,
        float endTime,
        float thrust,
        bool isDashing
    ) {
        if (hasExecutedOrder) hasExecutedOrder = false;
        handleAttackCo = StartCoroutine(HandleAttack(attackMove, targetPosition, duration, loadingTime, endTime, thrust, isDashing));
    }

    public void EndHandleAttackCo(bool softStop = false) {
        if (softStop) {
            interruptAttack = true;
            return;
        }
        if (handleAttackCo != null) StopCoroutine(handleAttackCo);
        handleAttackCo = null;
        animator.SetInteger("attackValue", 0);
        animator.SetInteger("whirlwindStatus", 0);
    }

    public bool CheckOrder(bool shouldReset = true) {
        if (hasExecutedOrder) {
            if (shouldReset) hasExecutedOrder = false;
            return true;
        } else {
            return false;
        }
    }

    public void ResetOrder() {
        hasExecutedOrder = false;
    }

    public void ManageThreatAreaActivation(bool isActivated) {
        threatArea.SetActive(isActivated);
    }

    private IEnumerator HandleMoveCharacter(Vector2 destination, CharDirection direction) {
        animator.SetBool("isMoving", true);
        switch (direction) {
            case CharDirection.Left:
                while (transform.position.x > destination.x) {
                    rb.MovePosition(transform.position + Vector3.left * speed * Time.deltaTime);
                    yield return waitFFU;
                }
                break;
            case CharDirection.Right:
                while (transform.position.x < destination.x) {
                    rb.MovePosition(transform.position + Vector3.right * speed * Time.deltaTime);
                    yield return waitFFU;
                }
                break;
            case CharDirection.Down:
                while (transform.position.y > destination.y) {
                    rb.MovePosition(transform.position + Vector3.down * speed * Time.deltaTime);
                    yield return waitFFU;
                }
                break;
            case CharDirection.Up:
                while (transform.position.y < destination.y) {
                    rb.MovePosition(transform.position + Vector3.up * speed * Time.deltaTime);
                    yield return waitFFU;
                }
                break;
        }
        hasExecutedOrder = true;
        EndHandleMoveCharacter();
    }

    private IEnumerator HandleAttack(
        ShekraAttackMove attackMove,
        Vector3 targetPosition,
        float duration,
        float loadingTime,
        float endTime,
        float thrust,
        bool isDashing
    ) {
        interruptAttack = false;
        float attackDuration = Time.time + duration;
        Vector2 direction = targetPosition - transform.position;
        Vector2 force = direction.normalized * thrust;

        switch (attackMove) {
            case ShekraAttackMove.Attack1:
                animator.SetInteger("attackValue", 1);
                if (isDashing) {
                    while (Time.time < attackDuration && !interruptAttack) {
                        rb.AddForce(force, ForceMode2D.Impulse);
                        yield return waitFFU;
                    }
                } else {
                    while (Time.time < attackDuration && !interruptAttack) yield return waitFFU;
                }
                break;
            case ShekraAttackMove.Attack2:
                animator.SetInteger("attackValue", 2);
                if (isDashing) {
                    while (Time.time < attackDuration && !interruptAttack) {
                        rb.AddForce(force, ForceMode2D.Impulse);
                        yield return waitFFU;
                    }
                } else {
                    while (Time.time < attackDuration && !interruptAttack) yield return waitFFU;
                }
                break;
            case ShekraAttackMove.ClockwiseAttack:
                animator.SetInteger("attackValue", 3);
                yield return new WaitForSeconds(loadingTime);
                animator.SetInteger("whirlwindStatus", 1);
                if (isDashing) {
                    while (Time.time < attackDuration && !interruptAttack) {
                        rb.AddForce(force, ForceMode2D.Impulse);
                        yield return waitFFU;
                    }
                } else {
                    while (Time.time < attackDuration && !interruptAttack) yield return waitFFU;
                }
                break;
            case ShekraAttackMove.CounterclockwiseAttack:
                animator.SetInteger("attackValue", 4);
                yield return new WaitForSeconds(loadingTime);
                animator.SetInteger("whirlwindStatus", 1);
                if (isDashing) {
                    while (Time.time < attackDuration && !interruptAttack) {
                        rb.AddForce(force, ForceMode2D.Impulse);
                        yield return waitFFU;
                    }
                } else {
                    while (Time.time < attackDuration && !interruptAttack) yield return waitFFU;
                }
                break;
        }
        
        if (interruptAttack) interruptAttack = false;
        animator.SetInteger("attackValue", 0);
        yield return new WaitForSeconds(endTime);
        animator.SetInteger("whirlwindStatus", 0);

        hasExecutedOrder = true;
        EndHandleAttackCo();
    }

    private void SetAnimatorDirection(CharDirection moveDirection) {
        switch (moveDirection) {
            case CharDirection.Left:
                animator.SetFloat("moveX", -1);
                animator.SetFloat("moveY", 0);
                break;
            case CharDirection.Right:
                animator.SetFloat("moveX", 1);
                animator.SetFloat("moveY", 0);
                break;
            case CharDirection.Down:
                animator.SetFloat("moveX", 0);
                animator.SetFloat("moveY", -1);
                break;
            case CharDirection.Up:
                animator.SetFloat("moveX", 0);
                animator.SetFloat("moveY", 1);
                break;
        }
    }

    private void UpdateWhirlwindStatus(int value) {
        animator.SetInteger("whirlwindStatus", value);
    }
}
