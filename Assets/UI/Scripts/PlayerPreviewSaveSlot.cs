using UnityEngine;

public class PlayerPreviewSaveSlot : MonoBehaviour {
    private Animator animator;

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    public void SetSuitValue(string suitName) {
        switch (suitName) {
            case "black_dress":
                animator.SetInteger("suit", 0);
                break;
            case "green_dress":
                animator.SetInteger("suit", 1);
                break;
            default:
                Debug.LogError($"UNKNOWN SUIT NAME: {suitName} - DEFAULT WILL BE BLACK DRESS");
                animator.SetInteger("suit", 0);
                break;
        }
    }

    public void HandleSelection(bool isActivated) {
        animator.SetBool("isSelected", isActivated);
    }
}
