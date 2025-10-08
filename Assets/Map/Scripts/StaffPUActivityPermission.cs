using System.Collections.Generic;
using UnityEngine;

public class StaffPUActivityPermission : MonoBehaviour, IHandleActivationPermission {
    [SerializeField]
    private GameObject inventoryWindow;

    public bool DoesPlayerHasStaff() {
        return inventoryWindow
            .GetComponent<InventoryManager>()
            .GetElementsInSlots()
            .Contains("staff");
    }

    public bool CheckPermission() {
        return
            StoryTracker.currentStoryState == StoryState.Prologue
            && !DoesPlayerHasStaff();
    }

    public void SetPermission(List<StoryState> permissions, bool add) { }

    public List<StoryState> GetPermissions() {
        return new List<StoryState>() { StoryState.Prologue };
    }
}
