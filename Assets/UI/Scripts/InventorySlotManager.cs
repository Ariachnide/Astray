using UnityEngine;
using UnityEngine.UI;

public class InventorySlotManager : MonoBehaviour {
    public bool isUsed, isEquipped;
    public InventoryElement element;
    [SerializeField]
    private GameObject equipmentSign;

    public void SetElement(InventoryElement e) {
        element = e;
        GetComponent<Image>().sprite = e.icon;
        GetComponent<Image>().color = Color.white;
        isUsed = true;
    }

    public void HandleIsEquipped(bool shouldEquip) {
        if (shouldEquip) {
            isEquipped = true;
            equipmentSign.SetActive(true);
        } else {
            isEquipped = false;
            equipmentSign.SetActive(false);
        }
    }
}
