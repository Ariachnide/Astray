using UnityEngine;

public class DefaultVisionPlayerReaction : MonoBehaviour {
    public Transform SavedTarget { get; protected set; } = null;

    private void OnTriggerEnter2D(Collider2D collider) {
        if (collider.CompareTag("Player")) {
            SavedTarget = collider.transform;
            HandleReactionOnPlayer();
        }
    }

    private void OnTriggerStay2D(Collider2D collider) {
        if (collider.CompareTag("Player")) {
            SavedTarget = collider.transform;
            HandleReactionOnPlayer();
        }
    }

    private void OnTriggerExit2D(Collider2D collider) {
        if (collider.CompareTag("Player")) {
            SavedTarget = null;
            HandleReactionOnPlayer();
        }
    }

    protected virtual void HandleReactionOnPlayer() {
        transform.parent.GetComponent<DefaultEnemyAggro>().CheckAggressiveBehaviourVision();
    }
}
