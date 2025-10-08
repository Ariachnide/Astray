using System.Collections;
using UnityEngine;

public class GuardBehaviour : MonoBehaviour, IHitByShekra {
    public float speed;
    private bool hasExecutedOrder;
    private Rigidbody2D rb;
    private Animator animator;
    private WaitForFixedUpdate waitFFU = new WaitForFixedUpdate();
    public Coroutine handleMoveCharacterCo { get; private set; } = null;
    [SerializeField] private GameObject enemyDestructionEffect;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
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

    public void TurnCharacter(CharDirection moveDirection) {
        SetAnimatorDirection(moveDirection);
    }

    public void SetArm(bool isArmed) {
        animator.SetBool("isArmed", isArmed);
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

    public void EndHandleMoveCharacter() {
        if (handleMoveCharacterCo != null) StopCoroutine(handleMoveCharacterCo);
        animator.SetBool("isMoving", false);
        handleMoveCharacterCo = null;
    }

    public void Blink(float d) {
        StartCoroutine(Blinking(d));
    }

    public void HandleShekraHit(Vector3 shekraPosition, float duration, float thrust, bool destroyAfterHit) {
        StartCoroutine(HandleKnockback(transform.position - shekraPosition, duration, thrust, destroyAfterHit));
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

    private IEnumerator Blinking(float d) {
        animator.SetBool("isBlinking", true);
        float blinkDuration = Time.time + d;
        while (Time.time < blinkDuration) yield return null;
        animator.SetBool("isBlinking", false);
    }

    private IEnumerator HandleKnockback(Vector3 kbDirection, float duration, float thrust, bool destroyAfterHit) {
        EndHandleMoveCharacter();
        Vector2 force = kbDirection.normalized * (thrust * rb.mass);
        float kbDuration = Time.time + duration;
        while (Time.time < kbDuration) {
            rb.AddForce(force, ForceMode2D.Impulse);
            yield return waitFFU;
        }

        if (destroyAfterHit) {
            Instantiate(enemyDestructionEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    private void SetAnimatorDirection(CharDirection direction) {
        switch (direction) {
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
}
