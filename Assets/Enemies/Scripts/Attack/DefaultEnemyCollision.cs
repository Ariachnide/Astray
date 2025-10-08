using System;
using UnityEngine;

[RequireComponent(typeof(MainEnemyFactory))]
public class DefaultEnemyCollision : MonoBehaviour {
    protected MainEnemyFactory mainEF;
    protected AggroEnemyFactory aggroEF;
    protected bool hasAggroEF;
    protected MoveTowardsTargetEnemyAggro moveTowardsTargetEnemyAggro;
    protected bool hasMoveTowardsTargetEnemyAggro;

    public bool isDangerousOnCollision = true;
    public Int16 collisionDamage = 1;

    public bool pauseOnCollision;
    public float pauseDuration = 1f;
    private bool hasDealtDmgOnCollision;

    private void Awake() {
        mainEF = GetComponent<MainEnemyFactory>();
        aggroEF = GetComponent<AggroEnemyFactory>();
        hasAggroEF = GetComponent<AggroEnemyFactory>() != null;
        moveTowardsTargetEnemyAggro = GetComponent<MoveTowardsTargetEnemyAggro>();
        hasMoveTowardsTargetEnemyAggro = GetComponent<MoveTowardsTargetEnemyAggro>() != null;
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.GetComponent<IHandleHostileContact>() != null && isDangerousOnCollision)
            HandleContact(collision.gameObject);
    }

    protected virtual void OnCollisionStay2D(Collision2D collision) {
        if (collision.gameObject.GetComponent<IHandleHostileContact>() != null && isDangerousOnCollision)
            HandleContact(collision.gameObject);
    }

    public virtual void HandleContact(GameObject other) {
        hasDealtDmgOnCollision = other.GetComponent<IHandleHostileContact>().TryDealMeleeDamages(collisionDamage, transform.position);
        if (pauseOnCollision && hasDealtDmgOnCollision) PauseOnCollision();
    }

    protected virtual void PauseOnCollision() {
        if (hasAggroEF) {
            aggroEF.DelayAggressiveBehaviour(pauseDuration);
            if (hasMoveTowardsTargetEnemyAggro) moveTowardsTargetEnemyAggro.DelayMoveTowardsTarget(pauseDuration);
        }
    }
}
