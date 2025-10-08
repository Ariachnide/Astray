using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ProjectileState {
    HostileLaunched,
    AllyLaunched,
    WornOut
}

[RequireComponent(typeof(HandleColliders))]
public class ProjectileDefault :
    MonoBehaviour,
    IColliderRef,
    IGetHit,
    IDestroyProjectile,
    ILaunchProjectile {
    public ColliderRef SelfColliderRef { get; private set; }
    public ProjectileState state;
    public GameObject launcher;
    protected Rigidbody2D rb;
    protected Collider2D col;
    protected SpriteRenderer srd;
    public Int16 dmg;
    public Vector2 direction;
    private Coroutine launchCo;

    private HandleColliders handleColliders;
    [SerializeField]
    private Sprite wornOutSprite;
    [SerializeField]
    private GameObject hit;
    [SerializeField]
    protected List<string>
        hostileTargetableElements,
        allyTargetableElements;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        srd = GetComponent<SpriteRenderer>();
        handleColliders = GetComponent<HandleColliders>();
    }

    private void OnDisable() {
        Destroy(gameObject);
    }

    // MANAGE HIT METHODS
    public virtual void GetHit(
        Int16 damage,
        GameObject blowGiver,
        Vector3 enemyPosition,
        float kbThrust = 25f,
        float kbDuration = 0.1f,
        string weaponName = "") {
        switch (weaponName) {
            case "":
                HandleHitByUnspecifiedWeapon(damage, blowGiver, enemyPosition, kbThrust, kbDuration, weaponName);
                break;
            case "sword":
                HandleHitBySword(damage, blowGiver, enemyPosition, kbThrust, kbDuration, weaponName);
                break;
        }
    }

    protected virtual void HandleHitByUnspecifiedWeapon(
        Int16 damage,
        GameObject blowGiver,
        Vector3 enemyPosition,
        float kbThrust,
        float kbDuration,
        string weaponName
    ) {

    }

    protected virtual void HandleHitBySword(
        Int16 damage,
        GameObject blowGiver,
        Vector3 enemyPosition,
        float kbThrust,
        float kbDuration,
        string weaponName
    ) {

    }
    // END MANAGE HIT

    // PROJECTILE DESTRUCTION METHODS
    public virtual void DestroyProjectile() {
        state = ProjectileState.WornOut;
        if (wornOutSprite != null) srd.sprite = wornOutSprite;
        rb.velocity = Vector2.zero;
        col.enabled = false;
        StartCoroutine(HandleDestruction());
    }

    protected virtual IEnumerator HandleDestruction() {
        yield return null;
        Destroy(gameObject);
    }
    // END PROJECTILE DESTRUCTION

    // ENTER COLLISION METHODS
    private void OnCollisionEnter2D(Collision2D collision) {
        switch (state) {
            case ProjectileState.HostileLaunched:
                HandleCollisionHostileLaunched(collision); break;
            case ProjectileState.AllyLaunched:
                HandleCollisionAllyLaunched(collision); break;
            case ProjectileState.WornOut:
                HandleCollisionWornOut(collision); break;
        }
        HandleCollisionEnd(collision);
    }

    protected virtual void HandleCollisionHostileLaunched(Collision2D collision) {
        if (hostileTargetableElements.Contains(collision.gameObject.tag)) {
            if (collision.gameObject.CompareTag("AllyProjectile")) {
                collision.gameObject.GetComponent<IDestroyProjectile>().DestroyProjectile();
                Instantiate(hit, collision.GetContact(0).point, Quaternion.identity);
            } else if (collision.gameObject.CompareTag("Player")) {
                collision.gameObject.GetComponent<PlayerHit>().GetHit(dmg, transform.position);
                Instantiate(hit, collision.GetContact(0).point, Quaternion.identity);
            }
        }
    }

    protected virtual void HandleCollisionAllyLaunched(Collision2D collision) {
        if (allyTargetableElements.Contains(collision.gameObject.tag)) {
            if (collision.gameObject.CompareTag("HostileProjectile")) {
                collision.gameObject.GetComponent<IDestroyProjectile>().DestroyProjectile();
                Instantiate(hit, collision.GetContact(0).point, Quaternion.identity);
            } else {
                collision.gameObject.GetComponent<IGetHit>().GetHit(dmg, launcher, transform.position);
                Instantiate(hit, collision.GetContact(0).point, Quaternion.identity);
            }
        }
    }

    protected virtual void HandleCollisionWornOut(Collision2D collision) {

    }

    protected virtual void HandleCollisionEnd(Collision2D collision) {
        DestroyProjectile();
    }
    // END ENTER COLLISION

    // LAUNCH MANAGER METHODS
    public virtual void HandleLaunch(
        Vector2 argDirection,
        GameObject argLauncher,
        float thrust = 8.5f,
        float flyDuration = 2.5f,
        Int16 argDmg = 1,
        bool isAlly = false
    ) {
        launcher = argLauncher;
        state = isAlly ? ProjectileState.AllyLaunched : ProjectileState.HostileLaunched;
        UpdateCollisions();
        dmg = argDmg;
        direction = argDirection;
        launchCo = StartCoroutine(Launch(direction, thrust >= 15f ? 15f : thrust, flyDuration));
    }

    private IEnumerator Launch(Vector2 direction, float thrust, float duration) {
        rb.AddForce(direction.normalized * thrust, ForceMode2D.Impulse);
        yield return new WaitForSeconds(duration);
        DestroyProjectile();
    }

    protected void EndLaunch() {
        if (launchCo != null) {
            StopCoroutine(launchCo);
            launchCo = null;
        }
    }
    // END LAUNCH MANAGER

    // COLLISIONS MANAGER METHODS
    private void UpdateCollisions() {
        List<GameObject> elements = handleColliders.GetCollidersObjects();

        foreach (GameObject e in elements) {
            CheckCollisionRulesByElement(e);
        }

        switch(state) {
            case ProjectileState.HostileLaunched:
                CheckCollisionRulesHostileLaunched(elements);
                break;
            case ProjectileState.AllyLaunched:
                CheckCollisionRulesAllyLaunched(elements);
                break;
            case ProjectileState.WornOut:
                CheckCollisionRulesWornOut(elements);
                break;
        }
    }

    protected virtual void CheckCollisionRulesByElement(GameObject element) {
        switch (element.GetComponent<IColliderRef>().SelfColliderRef) {
            case ColliderRef.Fireball:
                Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), element.GetComponent<Collider2D>(), ShouldIgnoreCollisionWithFireball(element));
                break;
            case ColliderRef.Whirlwind:
                Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), element.GetComponent<Collider2D>(), ShouldIgnoreCollisionWithWhirlwind(element));
                break;
        }
    }

    protected virtual bool ShouldIgnoreCollisionWithFireball(GameObject element) {
        return true;
    }

    protected virtual bool ShouldIgnoreCollisionWithWhirlwind(GameObject element) {
        return true;
    }

    protected virtual void CheckCollisionRulesHostileLaunched(List<GameObject> elements) {
        foreach (GameObject e in elements) {
            if (e.CompareTag("Enemy")) {
                Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), e.GetComponent<Collider2D>(), true);
            } else if (e.CompareTag("HostileProjectile")) {
                switch (e.GetComponent<IColliderRef>().SelfColliderRef) {
                    case ColliderRef.RockProjectile:
                        if (e.GetComponent<RockProjectile>().state == ProjectileState.HostileLaunched) {
                            Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), e.GetComponent<Collider2D>(), true);
                        } else if (e.GetComponent<RockProjectile>().state == ProjectileState.AllyLaunched) {
                            Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), e.GetComponent<Collider2D>(), false);
                        }
                        break;
                }
            }
        }

        Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), elements.Find(e => e.CompareTag("Player")).GetComponent<Collider2D>(), false);
    }

    protected virtual void CheckCollisionRulesAllyLaunched(List<GameObject> elements) {
        foreach (GameObject e in elements) {
            if (e.CompareTag("Enemy")) {
                Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), e.GetComponent<Collider2D>(), false);
            } else if (e.CompareTag("HostileProjectile")) {
                switch (e.GetComponent<IColliderRef>().SelfColliderRef) {
                    case ColliderRef.RockProjectile:
                        if (e.GetComponent<RockProjectile>().state == ProjectileState.HostileLaunched) {
                            Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), e.GetComponent<Collider2D>(), false);
                        } else if (e.GetComponent<RockProjectile>().state == ProjectileState.AllyLaunched) {
                            Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), e.GetComponent<Collider2D>(), true);
                        }
                        break;
                }
            }
        }

        Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), elements.Find(e => e.CompareTag("Player")).GetComponent<Collider2D>(), true);
    }

    protected virtual void CheckCollisionRulesWornOut(List<GameObject> elements) {

    }
    // END COLLISIONS MANAGER
}
