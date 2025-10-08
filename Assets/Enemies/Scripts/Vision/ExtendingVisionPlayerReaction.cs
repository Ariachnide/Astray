using UnityEngine;

public class ExtendingVisionPlayerReaction : DefaultVisionPlayerReaction {
    [SerializeField] private float radiusMultiplier = 1f;
    private float defaultRadius;
    private CircleCollider2D circleCollider;

    protected virtual void Awake() {
        circleCollider = GetComponent<CircleCollider2D>();
        defaultRadius = circleCollider.radius;
    }

    protected override void HandleReactionOnPlayer() {
        base.HandleReactionOnPlayer();
        circleCollider.radius = SavedTarget != null
            ? defaultRadius * radiusMultiplier
            : defaultRadius;
    }

    public void ForceRestoreDefaultRadius() {
        circleCollider.radius = defaultRadius;
        SavedTarget = null;
        base.HandleReactionOnPlayer();
    }
}
