using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpellCastState {
    hostile,
    ally
}

public class SpellManager : MonoBehaviour, ICast {
    private Animator animator;
    private Rigidbody2D rb;
    public SpellCastState state;
    public Int16 dmg;
    public string spellName;
    private Coroutine launchCo;
    private List<string> targetableElements;
    private GameObject caster;
    private Collider2D spellCollider;

    private void Awake() {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        targetableElements = new List<string>();
        spellCollider = GetComponent<Collider2D>();
    }

    public List<string> GetTargetableElements() {
        return targetableElements;
    }

    public GameObject GetCaster() {
        return caster;
    }

    public void Cast(Vector2 direction, AnimosityState hostility, GameObject argCaster, float thrust = 10f, float duration = 1.25f, Int16 argDmg = 0) {
        animator.SetFloat("moveX", direction.x);
        animator.SetFloat("moveY", direction.y);
        dmg = argDmg;
        caster = argCaster;
        switch (hostility) {
            case AnimosityState.Player:
                gameObject.tag = "AllySpell";
                state = SpellCastState.ally;
                targetableElements = caster.GetComponent<PlayerTargetableElements>().GetSpellTargetableElements();
                break;
            case AnimosityState.Enemy:
                gameObject.tag = "HostileSpell";
                state = SpellCastState.hostile;
                // handle spell against player
                break;
        }
        GetComponent<IManageSpellCollision>().UpdateCollisions(state);
        launchCo = StartCoroutine(Launch(direction, thrust, duration));
    }

    private IEnumerator Launch(Vector2 direction, float thrust, float duration) {
        rb.AddForce(direction.normalized * thrust, ForceMode2D.Impulse);
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }

    private void EndLaunch() {
        StopCoroutine(launchCo);
        launchCo = null;
    }

    public void Immobilize() {
        rb.velocity = Vector2.zero;
        spellCollider.enabled = false;
        EndLaunch();
    }
}
