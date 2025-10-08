using System;
using UnityEngine;

public class RupeeToCollect : MonoBehaviour, ICollectElement, IGetItemData {
    public Int16 value;

    public void Collect(GameObject go) {
        go.GetComponent<PlayerInventory>().UpdateRupees(value);
    }

    public string GetName() {
        return $"<color=#00FFFF>{value} rubis</color>";
    }

    public string GetComment() {
        return "";
    }
}
