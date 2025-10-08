using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRMArea : MonoBehaviour {
    private int areaOwnerID;
    public bool IsOwnerInArea { get; private set; } = true;
    private Vector2 size, defaultPosition;
    private Coroutine checkingDirectionsCo = null;
    private readonly List<MoveDirections> allDirections = new() {
        MoveDirections.Right,
        MoveDirections.Left,
        MoveDirections.Up,
        MoveDirections.Down,
        MoveDirections.TopRight,
        MoveDirections.TopLeft,
        MoveDirections.BottomRight,
        MoveDirections.BottomLeft
    };

    private void Awake() {
        areaOwnerID = transform.parent.gameObject.GetInstanceID();
        size = GetComponent<BoxCollider2D>().size;
        defaultPosition = transform.position;
    }

    private void OnEnable() {
        transform.position = defaultPosition;
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        if (collider.CompareTag("Enemy") && collider.gameObject.GetInstanceID() == areaOwnerID) {
            transform.parent.GetComponent<EnemyRandomMoves>().RestoreAvailableDirections();
            EndCheckingDirections();
            IsOwnerInArea = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collider) {
        if (collider.CompareTag("Enemy") && collider.gameObject.GetInstanceID() == areaOwnerID) {
            GetAvailableDirections();
            if (gameObject.activeInHierarchy) checkingDirectionsCo = StartCoroutine(CheckingDirections());
            IsOwnerInArea = false;
        }
    }

    private void GetAvailableDirections() {
        List<MoveDirections> directions = new();
        foreach (MoveDirections d in allDirections) directions.Add(d);

        if (transform.parent.position.x > (transform.position.x + size.x / 2)) {
            directions.Remove(MoveDirections.Right);
            if (transform.parent.position.y > (transform.position.y + size.y / 2)) {
                directions.Remove(MoveDirections.TopRight);
            } else if (transform.parent.position.y < (transform.position.y - size.y / 2)) {
                directions.Remove(MoveDirections.BottomRight);
            }
        } else if (transform.parent.position.x < (transform.position.x - size.x / 2)) {
            directions.Remove(MoveDirections.Left);
            if (transform.parent.position.y > (transform.position.y + size.y / 2)) {
                directions.Remove(MoveDirections.TopLeft);
            } else if (transform.parent.position.y < (transform.position.y - size.y / 2)) {
                directions.Remove(MoveDirections.BottomLeft);
            }
        }

        if (transform.parent.position.y > (transform.position.y + size.y / 2)) {
            directions.Remove(MoveDirections.Up);
        } else if (transform.parent.position.y < (transform.position.y - size.y / 2)) {
            directions.Remove(MoveDirections.Down);
        }

        transform.parent.GetComponent<EnemyRandomMoves>().UpdateCurrentMoveDirections(directions);
    }

    private IEnumerator CheckingDirections() {
        while (true) {
            GetAvailableDirections();
            yield return new WaitForSeconds(1f);
        }
    }

    private void EndCheckingDirections() {
        if (checkingDirectionsCo != null) {
            StopCoroutine(checkingDirectionsCo);
            checkingDirectionsCo = null;
        }
    }
}
