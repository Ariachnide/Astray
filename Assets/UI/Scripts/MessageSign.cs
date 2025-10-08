using UnityEngine;
using UnityEngine.UI;

public enum MsgBoxSign {
    arrow,
    square
}

public class MessageSign : MonoBehaviour {
    [SerializeField]
    private Sprite arrow, square;
    private Image img;

    private void Awake() {
        img = GetComponent<Image>();
    }

    public void DisplaySign(MsgBoxSign shape) {
        switch(shape) {
            case MsgBoxSign.arrow:
                img.sprite = arrow;
                break;
            case MsgBoxSign.square:
                img.sprite = square;
                break;
        }
        img.color = Color.cyan;
    }

    public void HideSign() {
        img.color = new Color(0f, 0f, 0f, 0f);
    }
}
