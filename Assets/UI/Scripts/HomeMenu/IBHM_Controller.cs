using UnityEngine;
using UnityEngine.UI;

public class IBHM_Controller : MonoBehaviour {
    public bool isUsed { get; private set; }

    public void SetIcon(Sprite s) {
        GetComponent<Image>().sprite = s;
        GetComponent<Image>().color = Color.white;
        isUsed = true;
    }

    public void RemoveIcon() {
        GetComponent<Image>().sprite = null;
        GetComponent<Image>().color = new Color(1f, 1f, 1f, 0);
        isUsed = false;
    }
}
