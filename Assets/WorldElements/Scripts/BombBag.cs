using System;
using UnityEngine;

public class BombBag : MonoBehaviour, ICollectElement, IGetItemData {
    public Int16 value;

    public void Collect(GameObject go) {
        go.GetComponent<PlayerItem>().SetMaxBomb(value);
        go.GetComponent<PlayerItem>().UpdateBombs(value);
    }

    public string GetName() {
        return $"<color=#00FFFF>un sac de bombe</color>";
    }

    public string GetComment() {
        return $"Il vous permettra de porter un maximum de {value} bombes.";
    }
}
