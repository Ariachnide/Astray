using System.Collections.Generic;
using UnityEngine;

public class StaffPUAreaActivityPermission : MonoBehaviour, IHandleActivationPermission {
    [SerializeField]
    private GameObject inventoryWindow;

    public bool DoesPlayerHasStaff() {
        return inventoryWindow
            .GetComponent<InventoryManager>()
            .GetElementsInSlots()
            .Contains("staff");
    }

    public bool HasPlayerEquippedWeapon() {
        return inventoryWindow
            .GetComponent<InventoryManager>()
            .equippedWeapon != null;
    }

    public bool CheckPermission() {
        return
            StoryTracker.currentStoryState == StoryState.Prologue
            && (!DoesPlayerHasStaff() || !HasPlayerEquippedWeapon());
    }

    public void SetPermission(List<StoryState> permissions, bool add) { }

    public List<StoryState> GetPermissions() {
        return new List<StoryState>() { StoryState.Prologue };
    }
}
