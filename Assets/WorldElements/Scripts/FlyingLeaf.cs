using System;
using System.Collections;
using UnityEngine;

public class FlyingLeaf : MonoBehaviour {
    private Int16 delay = 5, timer = 0, speed = 2;
    private float endTimer;
    private SpriteRenderer srd;
    private Rigidbody2D rb;
    private WaitForFixedUpdate waitForFixedUpdate;

    private void Awake() {
        srd = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        waitForFixedUpdate = new WaitForFixedUpdate();
    }

    private void Start() {
        endTimer = Time.time + 1.5f;
    }

    private void Update() {
        if (Time.time > endTimer) Destroy(gameObject);
        if (timer < delay) {
            timer++;
        } else {
            srd.enabled = !srd.enabled;
        }
    }

    public void FlyTopLeft() {
        int choice = UnityEngine.Random.Range(0, 3);
        switch (choice) {
            case 0:
                StartCoroutine(TL0());
                break;
            case 1:
                StartCoroutine(TL1());
                break;
            case 2:
                StartCoroutine(TL2());
                break;
        }
    }

    private IEnumerator TL0() {
        float timer = Time.time + 0.75f;
        Vector3 destination = Vector3.left;
        while (Time.time < timer) {
            rb.MovePosition(transform.position + destination * (speed * 1.5f) * Time.deltaTime);
            yield return waitForFixedUpdate;
        }
        timer = Time.time + 0.75f;
        destination = new Vector3(-0.5f, 0.5f, 0);
        srd.flipY = !srd.flipY;
        float flipTimer = Time.time + 0.2f;
        while (Time.time < timer) {
            rb.MovePosition(transform.position + destination * speed * Time.deltaTime);
            if (Time.time > flipTimer) {
                flipTimer = Time.time + 0.2f;
                srd.flipY = !srd.flipY;
            }
            yield return waitForFixedUpdate;
        }
    }

    private IEnumerator TL1() {
        float timer = Time.time + 1f;
        Vector3 destination = new Vector3(-0.5f, 0.5f, 0);
        while (Time.time < timer) {
            rb.MovePosition(transform.position + destination * speed * Time.deltaTime);
            yield return waitForFixedUpdate;
        }
        timer = Time.time + 0.5f;
        destination = new Vector3(0.5f, 0.5f, 0);
        srd.flipX = !srd.flipX;
        while (Time.time < timer) {
            rb.MovePosition(transform.position + destination * (speed / 2) * Time.deltaTime);
            yield return waitForFixedUpdate;
        }
    }

    private IEnumerator TL2() {
        float timer = Time.time + 0.5f;
        Vector3 destination = Vector3.up;
        while (Time.time < timer) {
            rb.MovePosition(transform.position + destination * (speed * 1.5f) * Time.deltaTime);
            yield return waitForFixedUpdate;
        }
        timer = Time.time + 0.5f;
        destination = new Vector3(-0.5f, 0.5f, 0);
        srd.flipY = !srd.flipY;
        while (Time.time < timer) {
            rb.MovePosition(transform.position + destination * speed * Time.deltaTime);
            yield return waitForFixedUpdate;
        }
        timer = Time.time + 0.5f;
        destination = new Vector3(-0.5f, -0.5f, 0);
        srd.flipX = !srd.flipX;
        while (Time.time < timer) {
            rb.MovePosition(transform.position + destination * speed * Time.deltaTime);
            yield return waitForFixedUpdate;
        }
    }

    public void FlyTopRight() {
        int choice = UnityEngine.Random.Range(0, 3);
        srd.flipX = true;
        switch (choice) {
            case 0:
                StartCoroutine(TR0());
                break;
            case 1:
                StartCoroutine(TR1());
                break;
            case 2:
                StartCoroutine(TR2());
                break;
        }
    }

    private IEnumerator TR0() {
        float timer = Time.time + 1f;
        Vector3 destination = new Vector3(0.5f, 0.5f, 0);
        while (Time.time < timer) {
            rb.MovePosition(transform.position + destination * speed * Time.deltaTime);
            yield return waitForFixedUpdate;
        }
        timer = Time.time + 0.5f;
        destination = new Vector3(-0.5f, 0.5f, 0);
        srd.flipX = !srd.flipX;
        float flipTimer = Time.time + 0.2f;
        while (Time.time < timer) {
            rb.MovePosition(transform.position + destination * (speed / 2) * Time.deltaTime);
            if (Time.time > flipTimer) {
                flipTimer = Time.time + 0.2f;
                srd.flipY = !srd.flipY;
            }
            yield return waitForFixedUpdate;
        }
    }

    private IEnumerator TR1() {
        float timer = Time.time + 0.5f;
        Vector3 destination = Vector3.up;
        while (Time.time < timer) {
            rb.MovePosition(transform.position + destination * speed * Time.deltaTime);
            yield return waitForFixedUpdate;
        }
        timer = Time.time + 0.5f;
        destination = new Vector3(0.5f, 0.5f, 0);
        srd.flipY = !srd.flipY;
        while (Time.time < timer) {
            rb.MovePosition(transform.position + destination * speed * Time.deltaTime);
            yield return waitForFixedUpdate;
        }
        timer = Time.time + 0.5f;
        destination = new Vector3(0.5f, -0.5f, 0);
        srd.flipY = !srd.flipY;
        while (Time.time < timer) {
            rb.MovePosition(transform.position + destination * speed * Time.deltaTime);
            yield return waitForFixedUpdate;
        }
    }

    private IEnumerator TR2() {
        float timer = Time.time + 0.5f;
        Vector3 destination = Vector3.right;
        while (Time.time < timer) {
            rb.MovePosition(transform.position + destination * (speed * 1.5f) * Time.deltaTime);
            yield return waitForFixedUpdate;
        }
        timer = Time.time + 0.5f;
        destination = new Vector3(0.5f, -0.5f, 0);
        srd.flipX = !srd.flipX;
        while (Time.time < timer) {
            rb.MovePosition(transform.position + destination * speed * Time.deltaTime);
            yield return waitForFixedUpdate;
        }
        timer = Time.time + 0.5f;
        destination = new Vector3(-0.5f, -0.5f, 0);
        srd.flipX = !srd.flipX;
        while (Time.time < timer) {
            rb.MovePosition(transform.position + destination * speed * Time.deltaTime);
            yield return waitForFixedUpdate;
        }
    }

    public void FlyBottomLeft() {
        int choice = UnityEngine.Random.Range(0, 3);
        srd.flipY = true;
        switch (choice) {
            case 0:
                StartCoroutine(BL0());
                break;
            case 1:
                StartCoroutine(BL1());
                break;
            case 2:
                StartCoroutine(BL2());
                break;
        }
    }

    private IEnumerator BL0() {
        float timer = Time.time + 0.5f;
        Vector3 destination = new Vector3(-0.5f, -0.5f, 0);
        float flipTimer = Time.time + 0.2f;
        while (Time.time < timer) {
            rb.MovePosition(transform.position + destination * speed * Time.deltaTime);
            if (Time.time > flipTimer) {
                flipTimer = Time.time + 0.2f;
                srd.flipX = !srd.flipX;
            }
            yield return waitForFixedUpdate;
        }
        timer = Time.time + 0.5f;
        destination = new Vector3(0.5f, -0.5f, 0);
        while (Time.time < timer) {
            rb.MovePosition(transform.position + destination * speed * Time.deltaTime);
            if (Time.time > flipTimer) {
                flipTimer = Time.time + 0.2f;
                srd.flipX = !srd.flipX;
            }
            yield return waitForFixedUpdate;
        }
        timer = Time.time + 0.5f;
        destination = new Vector3(-0.5f, -0.5f, 0);
        while (Time.time < timer) {
            rb.MovePosition(transform.position + destination * speed * Time.deltaTime);
            if (Time.time > flipTimer) {
                flipTimer = Time.time + 0.2f;
                srd.flipX = !srd.flipX;
            }
            yield return waitForFixedUpdate;
        }
    }

    private IEnumerator BL1() {
        float timer = Time.time + 1;
        Vector3 destination = Vector3.left;
        while (Time.time < timer) {
            rb.MovePosition(transform.position + destination * (speed * 1.5f) * Time.deltaTime);
            yield return waitForFixedUpdate;
        }
        timer = Time.time + 0.5f;
        destination = new Vector3(-0.5f, -0.5f, 0);
        srd.flipY = !srd.flipY;
        while (Time.time < timer) {
            rb.MovePosition(transform.position + destination * speed * Time.deltaTime);
            yield return waitForFixedUpdate;
        }
    }

    private IEnumerator BL2() {
        float timer = Time.time + 0.5f;
        Vector3 destination = Vector3.down;
        while (Time.time < timer) {
            rb.MovePosition(transform.position + destination * speed * Time.deltaTime);
            yield return waitForFixedUpdate;
        }
        timer = Time.time + 0.5f;
        destination = new Vector3(-0.5f, -0.5f, 0);
        srd.flipY = !srd.flipY;
        while (Time.time < timer) {
            rb.MovePosition(transform.position + destination * speed * Time.deltaTime);
            yield return waitForFixedUpdate;
        }
        timer = Time.time + 0.5f;
        destination = new Vector3(-0.5f, 0.5f, 0);
        srd.flipY = !srd.flipY;
        while (Time.time < timer) {
            rb.MovePosition(transform.position + destination * speed * Time.deltaTime);
            yield return waitForFixedUpdate;
        }
    }

    public void FlyBottomRight() {
        int choice = UnityEngine.Random.Range(0, 3);
        srd.flipX = true;
        srd.flipY = true;
        switch (choice) {
            case 0:
                StartCoroutine(BR0());
                break;
            case 1:
                StartCoroutine(BR1());
                break;
            case 2:
                StartCoroutine(BR2());
                break;
        }
    }

    private IEnumerator BR0() {
        float timer = Time.time + 0.5f;
        Vector3 destination = Vector3.right;
        while (Time.time < timer) {
            rb.MovePosition(transform.position + destination * speed * Time.deltaTime);
            yield return waitForFixedUpdate;
        }
        timer = Time.time + 0.5f;
        destination = new Vector3(0.5f, -0.5f, 0);
        srd.flipY = !srd.flipY;
        while (Time.time < timer) {
            rb.MovePosition(transform.position + destination * speed * Time.deltaTime);
            yield return waitForFixedUpdate;
        }
        timer = Time.time + 0.5f;
        destination = new Vector3(-0.5f, -0.5f, 0);
        srd.flipY = !srd.flipY;
        srd.flipX = !srd.flipX;
        while (Time.time < timer) {
            rb.MovePosition(transform.position + destination * speed * Time.deltaTime);
            yield return waitForFixedUpdate;
        }
    }

    private IEnumerator BR1() {
        float timer = Time.time + 1;
        Vector3 destination = Vector3.down;
        float flipTimer = Time.time + 0.3f;
        while (Time.time < timer) {
            rb.MovePosition(transform.position + destination * (speed * 1.5f) * Time.deltaTime);
            if (Time.time > flipTimer) {
                flipTimer = Time.time + 0.2f;
                srd.flipX = !srd.flipX;
                srd.flipY = !srd.flipY;
            }
            yield return waitForFixedUpdate;
        }
        timer = Time.time + 0.5f;
        destination = new Vector3(0.5f, -0.5f, 0);
        srd.flipY = !srd.flipY;
        while (Time.time < timer) {
            rb.MovePosition(transform.position + destination * speed * Time.deltaTime);
            yield return waitForFixedUpdate;
        }
    }

    private IEnumerator BR2() {
        float timer = Time.time + 0.5f;
        Vector3 destination = new Vector3(0.5f, -0.5f, 0);
        while (Time.time < timer) {
            rb.MovePosition(transform.position + destination * (speed * 2) * Time.deltaTime);
            yield return waitForFixedUpdate;
        }
        timer = Time.time + 1;
        destination = new Vector3(-0.5f, -0.5f, 0);
        srd.flipX = !srd.flipX;
        float flipTimer = Time.time + 0.15f;
        while (Time.time < timer) {
            rb.MovePosition(transform.position + destination * speed * Time.deltaTime);
            if (Time.time > flipTimer) {
                flipTimer = Time.time + 0.15f;
                srd.flipY = !srd.flipY;
            }
            yield return waitForFixedUpdate;
        }
    }
}
