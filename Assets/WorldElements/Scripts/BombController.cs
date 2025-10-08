using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombController : MonoBehaviour {
    private Renderer rd;
    private Animator animator;
    private Collider2D explosionCollider;
    private List<int> hitTargets;
    [SerializeField]
    private GameObject hit;

    private void Awake() {
        rd = GetComponent<Renderer>();
        animator = GetComponent<Animator>();
        explosionCollider = GetComponent<Collider2D>();
        hitTargets = new List<int>();
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        HurtTarget(collider);
    }

    private void OnTriggerStay2D(Collider2D collider) {
        HurtTarget(collider);
    }

    private void HurtTarget(Collider2D collider) {
        if (collider.CompareTag("Player")) {
            if (!hitTargets.Contains(collider.GetInstanceID())) {
                hitTargets.Add(collider.GetInstanceID());
                collider
                    .GetComponent<PlayerHit>()
                    .GetHit(2, transform.position, kbThrust: 50f, kbDuration: 0.15f);
            }
        } else if (collider.CompareTag("Enemy")) {
            if (!hitTargets.Contains(collider.GetInstanceID())) {
                hitTargets.Add(collider.GetInstanceID());
                collider
                    .GetComponent<IGetHit>()
                    .GetHit(20, gameObject, transform.position, kbThrust: 75f, kbDuration: 0.15f, weaponName: "bomb");
                Instantiate(hit, collider.transform.position, Quaternion.identity);
            }
        }
    }

    public void StartCountDown(float delay = 3f) {
        StartCoroutine(HandleCountDown(delay));
    }

    private IEnumerator HandleCountDown(float d) {
        yield return new WaitForSeconds(d - 1f);
        animator.SetBool("IsBlinking", true);
        yield return new WaitForSeconds(1f);
        rd.sortingLayerName = "Top-Level";
        animator.SetBool("IsExploding", true);
        GameObject.FindGameObjectsWithTag("MainCamera")[0].GetComponent<CamPlayerMovement>().Shake();
    }

    private void ActivateCollider() {
        explosionCollider.enabled = true;
    }

    private void DeactivateCollider() {
        explosionCollider.enabled = false;
    }

    public void End() {
        Destroy(gameObject);
    }

    public void HandleBombSmoke() {
        StartCoroutine(SmokeBomb());
    }

    private IEnumerator SmokeBomb() {
        Int16 blinkCd = 5;
        while (true) {
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
    }
}
