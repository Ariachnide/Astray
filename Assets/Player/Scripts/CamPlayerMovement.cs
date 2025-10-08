using System;
using System.Collections;
using UnityEngine;

public enum CamTargetType {
    none,
    player,
    customPosition
}

public class CamPlayerMovement : MonoBehaviour {
    public Transform target;
    public bool isShaking;
    private float smoothing, oSize;
    [SerializeField]
    private Vector2 minPosition, maxPosition;
    // [SerializeField]
    // private Area currentArea;
    [SerializeField]
    private Transform player;
    private WaitForFixedUpdate waitForFixedUpdate;

    private void Awake() {
        oSize = GetComponent<Camera>().orthographicSize;
        smoothing = 0.05f;
        waitForFixedUpdate = new WaitForFixedUpdate();
        target = null;
    }

    public void LockCameraOnTarget(CamTargetType tt, Vector2 p = default(Vector2)) {
        switch (tt) {
            case CamTargetType.none:
                target = null;
                break;
            case CamTargetType.player:
                target = player;
                transform.position = new Vector3(
                    Mathf.Clamp(target.position.x, minPosition.x, maxPosition.x),
                    Mathf.Clamp(target.position.y, minPosition.y, maxPosition.y),
                    transform.position.z
                );
                break;
            case CamTargetType.customPosition:
                target = transform;
                transform.position = new Vector3(
                    Mathf.Clamp(p.x, minPosition.x, maxPosition.x),
                    Mathf.Clamp(p.y, minPosition.y, maxPosition.y),
                    transform.position.z
                );
                break;
        }
    }

    private void LateUpdate() {
        if (!isShaking && target != null && transform.position != target.position)
            transform.position = Vector3.Lerp(
                transform.position,
                new Vector3(
                    Mathf.Clamp(target.position.x, minPosition.x, maxPosition.x),
                    Mathf.Clamp(target.position.y, minPosition.y, maxPosition.y),
                    transform.position.z
                ),
                smoothing
            );
    }

    public void UpdateArea(Area a) {
        // currentArea = a;
        minPosition.x = a.minPosition.x + oSize * 1.5f;
        maxPosition.x = a.maxPosition.x - oSize * 1.5f;
        minPosition.y = a.minPosition.y + oSize;
        maxPosition.y = a.maxPosition.y - oSize;
    }

    public void Shake() {
        StartCoroutine(HandleShaking());
    }

    private IEnumerator HandleShaking() {
        isShaking = true;
        float timeElapsed = 0;
        float cycleDuration = 0.12f;
        float timer = Time.time + cycleDuration * 4;
        float threshold = 0;
        bool direction = false;
        Int16 magnitude = 5;
        Int16 i = 0;
        Vector3 destination = Vector3.zero;
        while (Time.time < timer) {
            if (Time.time > threshold) {
                if (!direction && i < 4) i++;
                direction = !direction;
                destination = new Vector3(
                    Mathf.Clamp(target.position.x, minPosition.x, maxPosition.x) + ((direction ? -0.1f : 0.1f) * (magnitude / i)),
                    Mathf.Clamp(target.position.y, minPosition.y, maxPosition.y),
                    transform.position.z
                );
                threshold = Time.time + cycleDuration;
            }
            transform.position = Vector3.Lerp(
                transform.position,
                destination,
                timeElapsed / cycleDuration
            );
            timeElapsed += Time.deltaTime;
            yield return waitForFixedUpdate;
        }
        isShaking = false;
    }
}
