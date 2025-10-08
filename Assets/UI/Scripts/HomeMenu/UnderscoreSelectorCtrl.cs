using System;
using TMPro;
using UnityEngine;

public class UnderscoreSelectorCtrl : MonoBehaviour {
    private Int16 counter = 0;
    private bool isDisplayed = true;
    private TMP_Text textComponent;

    private void Awake() {
        textComponent = GetComponent<TMP_Text>();
    }

    private void FixedUpdate() {
        if (counter < 30) {
            counter++;
        } else {
            counter = 0;
            textComponent.text = isDisplayed ? "" : "_";
            isDisplayed = !isDisplayed;
        }
    }
}
