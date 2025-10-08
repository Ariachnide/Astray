using System;
using UnityEngine;

public class ManaPotionToCollect : MonoBehaviour, ICollectElement, IGetItemData {
    public Int16 value;

    public void Collect(GameObject go) {
        go.GetComponent<PlayerSpell>().AddMana(value);
    }

    public string GetName() {
        return $"<color=#00FFFF>{(value > 5 ? "une grande quantit� de mana" : "une petite quantit� de mana")}</color>";
    }

    public string GetComment() {
        return "";
    }
}
