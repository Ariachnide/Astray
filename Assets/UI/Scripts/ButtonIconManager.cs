using UnityEngine;
using UnityEngine.UI;

public class ButtonIconManager : MonoBehaviour {
    private bool isIconDisplayed;
    private Image img;

    private void Awake() {
        isIconDisplayed = true;
        img = GetComponent<Image>();
    }

    public void HandleVisibility(bool displayIcon) {
        isIconDisplayed = displayIcon;
        SetVisibility();
    }

    public void HandleSprite(Sprite s) {
        img.sprite = s;
        SetVisibility();
    }

    private void SetVisibility() {
        img.color = img.sprite == null
            ? new Color(1, 1, 1, 0)
            : isIconDisplayed
            ? Color.white
            : new Color(1, 1, 1, 0.3f);
    }
}
