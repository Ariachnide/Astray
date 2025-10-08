using System;
using TMPro;
using UnityEngine;

public class RupeeCounter : MonoBehaviour {
    private TMP_Text orbsTxt;
    private Int16 maxOrbs;
    [SerializeField]
    private GameObject uiOrbsIcon;

    private void Awake() {
        orbsTxt = GetComponent<TMP_Text>();
        orbsTxt.text = "0";
        orbsTxt.color = Color.gray;
    }

    public void UpdateText(Int16 v) {
        string strValue = v.ToString();
        orbsTxt.text = strValue;
        UpdateTextColor(v);
    }

    public void SetMaxRupee(Int16 v) {
        maxOrbs = v;
        UpdateTextColor(Int16.Parse(orbsTxt.text));
    }

    private void UpdateTextColor(Int16 v) {
        if (v == 0) {
            orbsTxt.color = Color.gray;
        } else if (v == maxOrbs) {
            orbsTxt.color = Color.cyan;
        } else if (v > 0 && v < maxOrbs) {
            orbsTxt.color = Color.white;
        } else {
            Debug.LogError($"INVALID ORBS VALUE: {v}");
        }
    }
}
