using System.Collections;
using UnityEngine;

[RequireComponent(typeof(TinyBlobMain))]
public class TinyBlobCollision : DefaultEnemyCollision {
    private Collider2D col;
    private Rigidbody2D rb;
    private MovementEnemyFactory movementEnemyFactory;
    private bool hasMovementEnemyFactory;
    [SerializeField]
    private TinyBlobAlterationType alterationType;

    private void Awake() {
        movementEnemyFactory = GetComponent<MovementEnemyFactory>();
        hasMovementEnemyFactory = GetComponent<MovementEnemyFactory>() != null;
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    public override void HandleContact(GameObject other) {
        if (!other.CompareTag("Player")) return;
        other.GetComponent<PlayerAlteration>().AffectTinyBlob(gameObject, GetComponent<TinyBlobMain>().persistance, alterationType);
        gameObject.SetActive(false);
    }

    public void ReactivateSelfAfterPlayerAlt(GameObject affectedPlayer, Vector2 altPosition) {
        Physics2D.IgnoreCollision(col, affectedPlayer.GetComponent<Collider2D>(), true);
        StartCoroutine(ReactivateColliderWithDelay(2, affectedPlayer));
        if (hasMovementEnemyFactory) {
            movementEnemyFactory.HandleRegularMoves(false);
            StartCoroutine(HandleJumpOut(affectedPlayer.transform.position, altPosition));
        }
    }

    private IEnumerator ReactivateColliderWithDelay(float delay, GameObject player) {
        yield return new WaitForSeconds(delay);
        Physics2D.IgnoreCollision(col, player.GetComponent<Collider2D>(), false);
    }

    private IEnumerator HandleJumpOut(Vector2 affectedPlayerPosition, Vector2 altPosition) {
        bool isRight = altPosition.x > affectedPlayerPosition.x;
        float
            angle = 0f,
            speed = 10f,
            radiusOffset = 0.3f,
            direction = isRight ? 1f : -1f,
            prevSinus = 1f,
            nextSinus = 0f;
        Vector2 movement;

        rb.position = isRight
            ? new Vector2(affectedPlayerPosition.x + 0.3f, affectedPlayerPosition.y - 0.175f)
            : new Vector2(affectedPlayerPosition.x - 0.3f, affectedPlayerPosition.y - 0.175f);

        yield return new WaitForFixedUpdate();

        while (prevSinus > nextSinus) {
            angle += Time.deltaTime * speed;
            prevSinus = nextSinus;
            nextSinus = Mathf.Sin(angle) * -radiusOffset;
            movement = speed * new Vector3(Mathf.Cos(angle) * radiusOffset * direction, nextSinus);
            movementEnemyFactory.movement = movement.normalized;
            yield return new WaitForFixedUpdate();
        }

        if (hasMovementEnemyFactory) movementEnemyFactory.HandleRegularMoves(true);
    }
}
