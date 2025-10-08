using System;
using UnityEngine;

public class SwordDamageArea : MonoBehaviour, IGetHit {
    [SerializeField] private GameObject hitEffect;
    public VisionDirection SwordDirection { get; private set; } = VisionDirection.None;

    private void OnTriggerEnter2D(Collider2D collider) {
        if (
            collider.gameObject.GetComponent<IHandleHostileContact>() != null
            && transform.parent.GetComponent<DefaultEnemyCollision>().isDangerousOnCollision
        )
            transform.parent.GetComponent<DefaultEnemyCollision>().HandleContact(collider.gameObject);
    }

    private void OnTriggerStay2D(Collider2D collider) {
        if (
            collider.gameObject.GetComponent<IHandleHostileContact>() != null
            && transform.parent.GetComponent<DefaultEnemyCollision>().isDangerousOnCollision
        )
            transform.parent.GetComponent<DefaultEnemyCollision>().HandleContact(collider.gameObject);
    }

    private void OnEnable() {
        SetArea(transform.parent.GetComponent<AggroEnemyFactory>().visionDirection);
    }

    private void OnDisable() {
        SwordDirection = VisionDirection.None;
    }

    public void GetHit(Int16 damage, GameObject blowGiver, Vector3 enemyPosition, float kbThrust = 25f, float kbDuration = 0.1f, string weaponName = "") {
        if (
            blowGiver
                .GetComponent<PlayerWeapon>()
                .GetRegEnemiesList()
                .Contains(transform.parent.gameObject.GetInstanceID())
        ) {
            return;
        } else {
            GameObject hitEffectInst = Instantiate(hitEffect, SetEffectPosition(), Quaternion.identity);
            hitEffectInst.transform.SetParent(transform.parent.parent);
            blowGiver.GetComponent<IHandleHostileContact>().DirectKnockback(transform.parent.position, 0.1f, 25f);
            transform.parent.GetComponent<DefaultEnemyMain>().DirectKnockback(transform.position, 0.1f, 25f);
            transform.parent.GetComponent<DefaultEnemyMain>().UpdateLastBlowGiver(blowGiver);
        }
    }

    private Vector2 SetEffectPosition() {
        switch (SwordDirection) {
            case VisionDirection.Right: return new Vector2(transform.position.x - 0.2f, transform.position.y);
            case VisionDirection.Left:  return new Vector2(transform.position.x + 0.2f, transform.position.y);
            case VisionDirection.Up:    return new Vector2(transform.position.x, transform.position.y - 0.2f);
            case VisionDirection.Down:  return new Vector2(transform.position.x, transform.position.y + 0.2f);
            default: Debug.LogError($"UNHANDLED VISION DIRECTION VALUE: {SwordDirection}"); return transform.position;
        }
    }

    public void SetArea(VisionDirection direction) {
        switch (direction) {
            case VisionDirection.Right: SetRight(); break;
            case VisionDirection.Left:  SetLeft(); break;
            case VisionDirection.Up:    SetUp(); break;
            case VisionDirection.Down:  SetDown(); break;
            default: Debug.LogError($"UNHANDLED VISION DIRECTION VALUE: {direction}"); break;
        }
    }

    private void SetRight() {
        transform.position = new Vector2(
            transform.parent.position.x + 0.6875f,
            transform.parent.position.y - 0.25f
        );
        GetComponent<CapsuleCollider2D>().size = new Vector2(0.6f, 0.2f);
        GetComponent<CapsuleCollider2D>().direction = CapsuleDirection2D.Horizontal;
        SwordDirection = VisionDirection.Right;
    }

    private void SetLeft() {
        transform.position = new Vector2(
            transform.parent.position.x - 0.6875f,
            transform.parent.position.y - 0.25f
        );
        GetComponent<CapsuleCollider2D>().size = new Vector2(0.6f, 0.2f);
        GetComponent<CapsuleCollider2D>().direction = CapsuleDirection2D.Horizontal;
        SwordDirection = VisionDirection.Left;
    }

    private void SetUp() {
        transform.position = new Vector2(
            transform.parent.position.x - 0.25f,
            transform.parent.position.y + 0.6875f
        );
        GetComponent<CapsuleCollider2D>().size = new Vector2(0.2f, 0.6f);
        GetComponent<CapsuleCollider2D>().direction = CapsuleDirection2D.Vertical;
        SwordDirection = VisionDirection.Up;
    }

    private void SetDown() {
        transform.position = new Vector2(
            transform.parent.position.x + 0.25f,
            transform.parent.position.y - 0.6875f
        );
        GetComponent<CapsuleCollider2D>().size = new Vector2(0.2f, 0.6f);
        GetComponent<CapsuleCollider2D>().direction = CapsuleDirection2D.Vertical;
        SwordDirection = VisionDirection.Down;
    }
}
