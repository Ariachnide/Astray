using System;
using UnityEngine;

[RequireComponent(typeof(MainEnemyFactory))]
public class DefaultEnemyTrigger : MonoBehaviour {
    public bool isDangerousOnTrigger = true;
    public Int16 triggerDamage = 1;

    protected virtual void OnTriggerEnter2D(Collider2D collider) {
        if (collider.gameObject.GetComponent<IHandleHostileContact>() != null && isDangerousOnTrigger)
            HandleContact(collider.gameObject);
    }

    protected virtual void OnTriggerStay2D(Collider2D collider) {
        if (collider.gameObject.GetComponent<IHandleHostileContact>() != null && isDangerousOnTrigger)
            HandleContact(collider.gameObject);
    }

    protected virtual void HandleContact(GameObject other) {
        other.GetComponent<IHandleHostileContact>().TryDealMeleeDamages(triggerDamage, transform.position);
    }
}
