using System;
using UnityEngine;

public class HeartToCollect : MonoBehaviour, ICollectElement, IGetItemData {
    private Int16 value;

    private void Awake() {
        value = 4;
    }

    public void Collect(GameObject go) {
        go.GetComponent<PlayerMain>().GetHeal(value);
    }

    public string GetName() {
        return "<color=\"red\">un cœur de soin</color>";
    }

    public string GetComment() {
        return "Vous récupérez un cœur de vie.";
    }
}
