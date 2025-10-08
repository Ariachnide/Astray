using TMPro;
using UnityEngine;

public class ActionText : MonoBehaviour {
    private TMP_Text actionTxt;

    private void Awake() {
        actionTxt = GetComponent<TMP_Text>();
    }

    public void UpdateText(string txt) {
        actionTxt.text = txt;
        // anim text
    }
}
