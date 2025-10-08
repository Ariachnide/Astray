using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhirlwindManager : MonoBehaviour, IColliderRef, IManageSpellCollision {
    public ColliderRef SelfColliderRef { get; private set; } = ColliderRef.Whirlwind;
    private SpellManager spellManager;
    private HandleColliders handleColliders;
    private Coroutine whirlCo;
    [SerializeField]
    private GameObject hit;
    private GameObject target;
    private Animator animator;
    public float whirlwindDuration;
    public bool isTriggerred;

    private void Awake() {
        spellManager = GetComponent<SpellManager>();
        handleColliders = GetComponent<HandleColliders>();
        animator = GetComponent<Animator>();
        if (whirlwindDuration <= 0f)
            whirlwindDuration = 2f;
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (!isTriggerred) {
            if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("PlayerInteraction")) {
                Destroy(gameObject);
            }
            if (spellManager.state == SpellCastState.ally) {
                if (spellManager.GetTargetableElements().Contains(collision.gameObject.tag)) {
                    isTriggerred = true;
                    target = collision.gameObject;
                    gameObject.transform.parent = target.transform;
                    transform.position = target.transform.position;
                    whirlCo = StartCoroutine(Whirl());
                    target.GetComponent<IHandleSpecialEffects>().SetSpecialEffect(SpecialEffectType.Whirl, 2f, gameObject);
                } else {
                    Destroy(gameObject);
                }
            }
        }
    }

    private IEnumerator Whirl() {
        spellManager.Immobilize();
        animator.SetBool("isActive", true);
        yield return new WaitForSeconds(whirlwindDuration);
        EndWhirl();
    }

    private void EndWhirl() {
        StopCoroutine(whirlCo);
        whirlCo = null;
        target.GetComponent<IHandleSpecialEffects>().EndSpecialEffect(SpecialEffectType.Whirl);
        Destroy(gameObject);
    }

    public void UpdateCollisions(SpellCastState state) {
        List<GameObject> elements = handleColliders.GetCollidersObjects();

        foreach (GameObject e in elements) {
            if (e.CompareTag("HostileProjectile") || e.CompareTag("AllyProjectile") || e.CompareTag("HostileSpell") || e.CompareTag("AllySpell")) {
                Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), e.GetComponent<Collider2D>(), true);
            }
        }
        switch (state) {
            case SpellCastState.ally:
                foreach (GameObject e in elements) {
                    if (e.CompareTag("Enemy")) {
                        Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), e.GetComponent<Collider2D>(), false);
                    }
                }
                Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), elements.Find(e => e.CompareTag("Player")).GetComponent<Collider2D>(), true);
                break;
            case SpellCastState.hostile:
                foreach (GameObject e in elements) {
                    if (e.CompareTag("Enemy")) {
                        Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), e.GetComponent<Collider2D>(), true);
                    }
                }
                Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), elements.Find(e => e.CompareTag("Player")).GetComponent<Collider2D>(), false);
                break;
        }
    }
}
