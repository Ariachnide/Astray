using System;
using UnityEngine;

public class BombsToCollect : MonoBehaviour, ICollectElement, IGetItemData {
    public Int16 value;

    public void Collect(GameObject go) {
        go.GetComponent<PlayerItem>().UpdateBombs(value);
    }

    public string GetName() {
        return $"<color=#00FFFF>{value} bombes</color>";
    }

    public string GetComment() {
        return "";
    }
}
