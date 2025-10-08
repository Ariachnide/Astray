using System.Collections.Generic;
using UnityEngine;

public class FireballManager : MonoBehaviour, IColliderRef, IManageSpellCollision {
    public ColliderRef SelfColliderRef { get; private set; } = ColliderRef.Fireball;
    private SpellManager spellManager;
    private HandleColliders handleColliders;
    [SerializeField]
    private GameObject hit;

    private void Awake() {
        spellManager = GetComponent<SpellManager>();
        handleColliders = GetComponent<HandleColliders>();
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag == "Wall" || collision.gameObject.tag == "PlayerInteraction")
            Destroy(gameObject);
        if (spellManager.state == SpellCastState.ally) {
            if (spellManager.GetTargetableElements().Contains(collision.gameObject.tag)) {
                collision.collider
                    .GetComponent<IGetHit>()
                    .GetHit(spellManager.dmg, spellManager.GetCaster(), transform.position);
                Instantiate(hit, collision.transform.position, Quaternion.identity);
                Destroy(gameObject);
            } else {
                Destroy(gameObject);
            }
        }
    }

    public void UpdateCollisions(SpellCastState state) {
        List<GameObject> elements = handleColliders.GetCollidersObjects();

        foreach (GameObject e in elements) {
            if (e.tag == "HostileProjectile" || e.tag == "AllyProjectile" || e.tag == "HostileSpell" || e.tag == "AllySpell") {
                Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), e.GetComponent<Collider2D>(), true);
            }
        }
        switch (state) {
            case SpellCastState.ally:
                foreach (GameObject e in elements) {
                    if (e.tag == "Enemy") {
                        Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), e.GetComponent<Collider2D>(), false);
                    }
                }
                Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), elements.Find(e => e.tag == "Player").GetComponent<Collider2D>(), true);
                break;
            case SpellCastState.hostile:
                foreach (GameObject e in elements) {
                    if (e.tag == "Enemy") {
                        Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), e.GetComponent<Collider2D>(), true);
                    }
                }
                Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), elements.Find(e => e.tag == "Player").GetComponent<Collider2D>(), false);
                break;
        }
    }
}
