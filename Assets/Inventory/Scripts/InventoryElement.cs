using UnityEngine;

public enum InventoryElementType {
    spell,
    weapon,
    suit,
    item
}

[CreateAssetMenu(fileName = "New Inventory Element", menuName = "Custom Assets/Inventory Element")]
public class InventoryElement : ScriptableObject {
    public string elementName;
    public string displayName;
    public string description;
    public Sprite icon;
    public InventoryElementType type;
}
