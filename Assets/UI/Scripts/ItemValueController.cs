using System;
using TMPro;
using UnityEngine;

public class ItemValueController : MonoBehaviour {
    private TMP_Text valueTxt;
    private Int16 maxValue;

    private void Awake() {
        valueTxt = GetComponent<TMP_Text>();
        valueTxt.text = "";
        valueTxt.color = Color.gray;
    }

    public void Hide() {
        valueTxt.text = "";
        maxValue = 0;
    }

    public void UpdateText(Int16 v) {
        string strValue = v.ToString();
        valueTxt.text = strValue;
        UpdateTextColor(v);
    }

    public void SetMaxValue(Int16 v) {
        maxValue = v;
        if (valueTxt.text != "")
            UpdateTextColor(Int16.Parse(valueTxt.text));
    }

    private void UpdateTextColor(Int16 v) {
        if (v == 0) {
            valueTxt.color = new Color(0.5f, 0.5f, 0.5f, 0.3f);
        } else if (v == maxValue) {
            valueTxt.color = Color.cyan;
        } else if (v > 0 && v < maxValue) {
            valueTxt.color = Color.white;
        } else {
            Debug.LogError($"INVALID ITEM VALUE: {v}");
        }
    }
}
