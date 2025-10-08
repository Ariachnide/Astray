using UnityEngine;

public class VisionCapsule : VisionEnemyFactory {
    [SerializeField] private float verticalWidthValue, verticalHeightValue;

    protected override void SetHorizontalCapsule() {
        GetComponent<CapsuleCollider2D>().size = new Vector2(verticalHeightValue, verticalWidthValue);
        GetComponent<CapsuleCollider2D>().direction = CapsuleDirection2D.Horizontal;
    }

    protected override void SetVerticalCapsule() {
        GetComponent<CapsuleCollider2D>().size = new Vector2(verticalWidthValue, verticalHeightValue);
        GetComponent<CapsuleCollider2D>().direction = CapsuleDirection2D.Vertical;
    }

    public override void SetVisionRight() {
        transform.position = new Vector2(
            transform.parent.position.x + verticalHeightValue / 2,
            transform.parent.position.y
        );
        SetHorizontalCapsule();
    }

    public override void SetVisionLeft() {
        transform.position = new Vector2(
            transform.parent.position.x - verticalHeightValue / 2,
            transform.parent.position.y
        );
        SetHorizontalCapsule();
    }

    public override void SetVisionUp() {
        transform.position = new Vector2(
            transform.parent.position.x,
            transform.parent.position.y + verticalHeightValue / 2
        );
        SetVerticalCapsule();
    }

    public override void SetVisionDown() {
        transform.position = new Vector2(
            transform.parent.position.x,
            transform.parent.position.y - verticalHeightValue / 2
        );
        SetVerticalCapsule();
    }
}
