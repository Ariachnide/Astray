using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum PMenuActionType {
    none,
    resume,
    accessSaveSection,
    accessLoadSection,
    save,
    load,
    accessLeaveSection,
    accessDefaultSection,
    confirmLeave
}

public class PlayerMenuAction : MonoBehaviour {
    private PlayerMain main;
    private PlayerMenu pMenu;

    private void Awake() {
        main = GetComponent<PlayerMain>();
        pMenu = GetComponent<PlayerMenu>();
    }

    public void HandleAction(PMenuActionType t) {
        switch (t) {
            case PMenuActionType.resume:
                Resume();
                break;
            case PMenuActionType.accessSaveSection:
                AccessSaveSection();
                break;
            case PMenuActionType.accessLoadSection:
                AccessLoadSection();
                break;
            case PMenuActionType.save:
                Save();
                break;
            case PMenuActionType.load:
                Load();
                break;
            case PMenuActionType.accessLeaveSection:
                AccessLeaveSection();
                break;
            case PMenuActionType.accessDefaultSection:
                AccessDefaultSection();
                break;
            case PMenuActionType.confirmLeave:
                ConfirmLeave();
                break;
            case PMenuActionType.none:
                break;
        }
    }

    private void Resume() {
        pMenu.HandleMainMenu();
    }

    private void AccessSaveSection() {
        pMenu.ChangeSet(MainMenuState.saveSet);
    }

    private void AccessLoadSection() {
        pMenu.ChangeSet(MainMenuState.loadSet);
    }

    private void Save() {
        PlayerData d = main.SavePlayer(pMenu.selectedElement.GetComponent<SaveSlotShort>().slotValue);
        pMenu.selectedElement.GetComponent<SaveSlotShort>().SetData(d);
        pMenu.UpdateSelection();
    }

    private void Load() {
        if (pMenu.selectedElement.GetComponent<SaveSlotShort>().hasData)
            StartCoroutine(LeavePage(pMenu.selectedElement.GetComponent<SaveSlotShort>().slotValue));
    }

    private IEnumerator LeavePage(Int16 slotValue) {
        pMenu.selectedElement = null;
        main.backgroundLayer.GetComponent<DisplayUIElement>().HideScene(Color.black);
        yield return new WaitForSecondsRealtime(1);
        SaveSystem.currentSlot = slotValue;
        SceneManager.LoadScene("Main");
    }

    private void AccessLeaveSection() {
        pMenu.ChangeSet(MainMenuState.confirmLeaveSet);
    }

    private void AccessDefaultSection() {
        pMenu.ChangeSet(MainMenuState.defaultSet);
    }

    private void ConfirmLeave() {
        Application.Quit();
    }
}
