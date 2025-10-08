using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DisplayUIElement : MonoBehaviour {
    private Image img;

    private void Awake() {
        img = GetComponent<Image>();
    }

    public void DisplayScene(float fadingTime = 1f) {
        StartCoroutine(Fade(true, new Color(img.color.r, img.color.g, img.color.b, 0), fadingTime));
    }

    public void HideScene(Color color, float fadingTime = 1f) {
        StartCoroutine(Fade(false, color, fadingTime));
    }

    public void SetColor(Color color) {
        img.color = color;
    }

    private IEnumerator Fade(bool displayScene, Color c, float t) {
        float delay = 0f;
        Color startColor = img.color;
        while (true) {
            delay += Time.unscaledDeltaTime / t;
            img.color = Color.Lerp(startColor, c, delay);
            if ((displayScene && img.color.a > 0) || (!displayScene && img.color.a < 1f)) {
                yield return null;
            } else {
                break;
            }
        }
    }
}
