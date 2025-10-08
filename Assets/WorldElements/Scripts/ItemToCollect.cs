using System;
using System.Collections;
using UnityEngine;

public enum ITCSpawnAnimType {
    common,
    lightWeight
}

public enum ITCHeight {
    common,
    small
}

public class ItemToCollect : MonoBehaviour {
    private Rigidbody2D rb;
    private Renderer rd;
    private GameObject shadow;
    private WaitForFixedUpdate waitForFixedUpdate;
    public bool canBeCollected;
    private ICollectElement collectElement;
    [SerializeField]
    private ITCSpawnAnimType spawnAnimType;
    [SerializeField]
    private ITCHeight itemHeight;

    private void Awake() {
        waitForFixedUpdate = new WaitForFixedUpdate();
        rb = GetComponent<Rigidbody2D>();
        rd = GetComponent<Renderer>();
        collectElement = GetComponent<ICollectElement>();
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        if (collider.CompareTag("Player") && canBeCollected)
            HandleCollect(collider.gameObject);
    }

    private void OnDestroy() {
        if (shadow != null)
            shadow.GetComponent<Shadow>().End();
    }

    public void HandleCollect(GameObject go, bool shouldDestroy = true) {
        collectElement.Collect(go);
        if (shouldDestroy)
            Destroy(gameObject);
    }

    public void SetAuth(bool auth) {
        canBeCollected = auth;
    }

    public bool GetAuth() {
        return canBeCollected;
    }

    public void Fade(float delay = 20f) {
        StartCoroutine(HandleFade(delay));
    }

    private IEnumerator HandleFade(float delay) {
        float blinkDelay = 5f;
        yield return new WaitForSeconds(delay - blinkDelay);
        Int16 blinkCd = 5;
        float timer = Time.time + blinkDelay;
        while (Time.time < timer) {
            if (Time.timeScale < 0.1f) {
                yield return null;
                continue;
            }
            if (blinkCd == 5) {
                blinkCd = 0;
                rd.enabled = !rd.enabled;
            } else {
                blinkCd++;
            }
            yield return null;
        }
        Destroy(gameObject);
    }

    public void SpawnAnim(GameObject shadowArg) {
        shadow = shadowArg;
        StartCoroutine(HandleSpawnAnim());
    }

    private IEnumerator HandleSpawnAnim() {
        float timer = Time.time + 0.1f;
        switch (spawnAnimType) {
            case ITCSpawnAnimType.common:
                Int16 speed = 10;
                while (Time.time < timer) {
                    rb.MovePosition(transform.position + Vector3.up * speed * Time.deltaTime);
                    yield return waitForFixedUpdate;
                }
                yield return new WaitForSeconds(0.05f);
                timer = Time.time + 0.1f;
                while (Time.time < timer) {
                    rb.MovePosition(transform.position + Vector3.down * speed * Time.deltaTime);
                    yield return waitForFixedUpdate;
                }
                timer = Time.time + 0.04f;
                while (Time.time < timer) {
                    rb.MovePosition(transform.position + Vector3.up * speed * Time.deltaTime);
                    yield return waitForFixedUpdate;
                }
                timer = Time.time + 0.04f;
                while (Time.time < timer) {
                    rb.MovePosition(transform.position + Vector3.down * speed * Time.deltaTime);
                    yield return waitForFixedUpdate;
                }
                break;
            case ITCSpawnAnimType.lightWeight:
                while (Time.time < timer) {
                    rb.MovePosition(transform.position + Vector3.up * 10 * Time.deltaTime);
                    yield return waitForFixedUpdate;
                }
                switch (itemHeight) {
                    case ITCHeight.common:
                        timer = Time.time + 1f;
                        break;
                    case ITCHeight.small:
                        timer = Time.time + 1.15f;
                        break;
                }
                while (Time.time < timer) {
                    rb.MovePosition(transform.position + Vector3.down * 1 * Time.deltaTime);
                    yield return waitForFixedUpdate;
                }
                break;
        }
        shadow.GetComponent<Shadow>().End();
    }

    public void ChestAnim(float delay) {
        StartCoroutine(HandleChestAnim(delay));
    }

    private IEnumerator HandleChestAnim(float delay) {
        float timer = Time.unscaledTime + delay;
        while (Time.unscaledTime < timer) {
            transform.Translate(new Vector3(0, 0.02f, 0));
            yield return new WaitForSecondsRealtime(0.02f);
        }
    }
}
