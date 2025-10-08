using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour {
    private bool display, isChanging/*, hide */;
    private Color inactiveColor, activeColor, startColor;
    private Image uiImage;
    private float delay;

    private void Awake() {
        uiImage = GetComponent<Image>();
        inactiveColor = new Color(1, 1, 1, 0.3f);
        activeColor = Color.white;
        startColor = GetComponent<Image>().color;
        delay = 0f;
    }

    public void HandleActivation(bool shouldDisplay) {
        if (shouldDisplay != display) {
            delay = 0f;
            startColor = GetComponent<Image>().color;
            display = shouldDisplay;
            if (!isChanging)
                StartCoroutine(Activation());
        }
    }

    /* public void HandleHiding(bool shouldHide) {
        hide = shouldHide;
    } */

    private IEnumerator Activation() {
        isChanging = true;
        while (true) {
            delay += Time.unscaledDeltaTime / 0.15f;
            if (display) {
                /* if (hide) {
                    uiImage.color = Color.Lerp(startColor, inactiveColor, delay);
                    if (uiImage.color.a > 0.3f) {
                        yield return null;
                    } else {
                        break;
                    }
                } */
                uiImage.color = Color.Lerp(startColor, activeColor, delay);
                if (uiImage.color.a < 1f) {
                    yield return null;
                } else {
                    break;
                }
            } else {
                uiImage.color = Color.Lerp(startColor, inactiveColor, delay);
                if (uiImage.color.a > 0.3f) {
                    yield return null;
                } else {
                    break;
                }
            }
        }
        isChanging = false;
    }
}
