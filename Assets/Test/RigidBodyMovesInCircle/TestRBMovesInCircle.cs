using UnityEngine;

public class TestRBMovesInCircle : MonoBehaviour {
    private Rigidbody2D rb;

    [SerializeField] private float speed = 2f;
    [SerializeField] private bool moveClockwise = true;
    [SerializeField] private float radiusOffset = 1f;
    private float
        angle = 0f,
        direction = -1f,
        x = 0f,
        y = 0f;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate() {
        direction = moveClockwise ? -1f : 1f;
        angle += Time.deltaTime * direction * speed;
        x = Mathf.Cos(angle) * radiusOffset;
        y = Mathf.Sin(angle) * radiusOffset;
        rb.MovePosition(transform.position + Time.deltaTime * speed * new Vector3(x, y));
    }
}
