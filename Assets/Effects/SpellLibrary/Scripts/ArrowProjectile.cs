using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowProjectile : ProjectileDefault {
    private bool softDestruction;
    [SerializeField]
    private Sprite upArrow, leftArrow, downArrow, rightArrow;
    private MoveDirections enumDirection;

    protected override void HandleCollisionEnd(Collision2D collision) {
        if (
            !hostileTargetableElements.Contains(collision.gameObject.tag)
            && !allyTargetableElements.Contains(collision.gameObject.tag)
        )
            softDestruction = true;
        base.HandleCollisionEnd(collision);
    }

    public override void DestroyProjectile() {
        if (!softDestruction) {
            Destroy(gameObject);
        } else {
            base.DestroyProjectile();
        }
    }

    protected override IEnumerator HandleDestruction() {
        if (direction.x > 0f) {
            rb.AddForce(new Vector2(-0.5f, 0f), ForceMode2D.Impulse);
        } else if (direction.x < 0f) {
            rb.AddForce(new Vector2(0.5f, 0f), ForceMode2D.Impulse);
        }
        if (direction.y > 0f) {
            rb.AddForce(new Vector2(0f, -0.5f), ForceMode2D.Impulse);
        } else if (direction.y < 0f) {
            rb.AddForce(new Vector2(0f, 0.5f), ForceMode2D.Impulse);
        }

        List<Sprite> wornOutSprites = new();
        switch(enumDirection) {
            case MoveDirections.Right:
                wornOutSprites = new() { upArrow, leftArrow, downArrow };
                break;
            case MoveDirections.Left:
                wornOutSprites = new() { upArrow, rightArrow, downArrow };
                break;
            case MoveDirections.Up:
                wornOutSprites = new() { rightArrow, downArrow, leftArrow };
                break;
            case MoveDirections.Down:
                wornOutSprites = new() { downArrow, leftArrow, rightArrow };
                break;
        }

        foreach (Sprite s in wornOutSprites) {
            srd.sprite = s;
            yield return new WaitForSeconds(0.2f);
        }

        Destroy(gameObject);
    }

    public override void HandleLaunch(
        Vector2 argDirection,
        GameObject argLauncher,
        float thrust = 8.5f,
        float flyDuration = 2.5f,
        Int16 argDmg = 1,
        bool isAlly = false
    ) {
        if (argDirection.x > 0.5f) {
            ArrowGoRight();
        } else if (argDirection.x < -0.5f) {
            ArrowGoLeft();
        }
        if (argDirection.y > 0.5f) {
            ArrowGoUp();
        } else if (argDirection.y < -0.5f) {
            ArrowGoDown();
        }
        base.HandleLaunch(argDirection, argLauncher, thrust, flyDuration, argDmg, isAlly);
    }

    private void ArrowGoUp() {
        enumDirection = MoveDirections.Up;
        srd.sprite = upArrow;
        col.offset = new Vector2(0, 0.25f);
    }

    private void ArrowGoDown() {
        enumDirection = MoveDirections.Down;
        srd.sprite = downArrow;
        col.offset = new Vector2(0, -0.25f);
    }

    private void ArrowGoLeft() {
        enumDirection = MoveDirections.Left;
        srd.sprite = leftArrow;
        col.offset = new Vector2(-0.25f, 0);
    }

    private void ArrowGoRight() {
        enumDirection = MoveDirections.Right;
        srd.sprite = rightArrow;
        col.offset = new Vector2(0.25f, 0);
    }
}
