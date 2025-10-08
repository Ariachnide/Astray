using System.Collections.Generic;
using UnityEngine;

public class AvailableElements : MonoBehaviour {
    [SerializeField]
    private List<InventoryElement> availableElements;

    public List<InventoryElement> GetAllAvailableElements() {
        return availableElements;
    }

    public List<InventoryElement> GetAvailableElementsByType(InventoryElementType t) {
        List<InventoryElement> elements = new List<InventoryElement>();
        foreach (InventoryElement e in availableElements) {
            if (e.type == t) {
                elements.Add(e);
            }
        }
        return elements;
    }

    public InventoryElement GetElementByName(string elementName) {
        return availableElements.Find(e => e.elementName == elementName);
    }
}
