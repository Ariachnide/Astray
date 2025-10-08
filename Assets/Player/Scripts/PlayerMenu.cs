using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum MainMenuState {
    none,
    defaultSet,
    saveSet,
    loadSet,
    confirmLeaveSet
}

public class PlayerMenu : MonoBehaviour {
    public MainMenuState state;
    private PlayerMain main;
    private PlayerInventory inventory;
    private PlayerAction action;
    private PlayerMenuAction pMenuAction;
    public bool canControlMainMenu;
    private Int16 selectionIndex;
    private bool isMenuLoading, prevInventoryAuth;
    private float prevTimeScale;
    private List<PlayerState> menuAuthorizedStates;
    private PlayerState prevState;
    [SerializeField]
    private GameObject mainMenuBackground, mainMenuWindow, defaultSet, saveSet, loadSet, confirmLeaveSet;
    public GameObject selectedElement;
    private List<GameObject> selectedElements;

    private void Awake() {
        main = GetComponent<PlayerMain>();
        inventory = GetComponent<PlayerInventory>();
        action = GetComponent<PlayerAction>();
        pMenuAction = GetComponent<PlayerMenuAction>();
        canControlMainMenu = true;
        menuAuthorizedStates = new List<PlayerState>() {
            PlayerState.idle,
            PlayerState.walk,
            PlayerState.usingWeapon,
            PlayerState.usingSpell,
            PlayerState.usingItem,
            PlayerState.knockbacked,
            PlayerState.busy,
            PlayerState.inMenu
        };
        selectedElements = new List<GameObject>();
        mainMenuWindow.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -Screen.height);
    }

    public void HandleMainMenu() {
        if (!canControlMainMenu || isMenuLoading || !menuAuthorizedStates.Contains(main.state)) return;
        if (state == MainMenuState.none) {
            prevTimeScale = Time.timeScale;
            Time.timeScale = 0;
            action.HandleMessageBoxIsPrintingFrozen(true);
            prevInventoryAuth = inventory.canAccessInventory;
            inventory.canAccessInventory = false;
            main.SetMovement(Vector2.zero);
            prevState = main.state;
            main.state = PlayerState.inMenu;
            StartCoroutine(HandleMainMenuAnimation(true));
            ChangeSet(MainMenuState.defaultSet);
        } else {
            ChangeSet(MainMenuState.none);
            StartCoroutine(HandleMainMenuAnimation(false));
            action.HandleMessageBoxIsPrintingFrozen(false);
            main.state = prevState;
            inventory.canAccessInventory = prevInventoryAuth;
            Time.timeScale = prevTimeScale;
        }
    }

    private IEnumerator HandleMainMenuAnimation(bool open) {
        isMenuLoading = true;
        float cycleDuration = 0.35f;
        float timer = Time.unscaledTime + cycleDuration;
        float timeElapsed = 0;
        RectTransform rt = mainMenuWindow.GetComponent<RectTransform>();
        Vector2 closeVector = new Vector2(0, -Screen.height);
        Image img = mainMenuBackground.GetComponent<Image>();
        Color openColor = new Color(0, 0, 0, 0.8f);
        if (open) {
            while (Time.unscaledTime < timer) {
                rt.anchoredPosition = Vector2.Lerp(closeVector, Vector2.zero, timeElapsed / cycleDuration);
                img.color = Color.Lerp(Color.clear, openColor, timeElapsed / cycleDuration);
                timeElapsed += Time.unscaledDeltaTime;
                yield return null;
            }
        } else {
            while (Time.unscaledTime < timer) {
                rt.anchoredPosition = Vector2.Lerp(Vector2.zero, closeVector, timeElapsed / cycleDuration);
                img.color = Color.Lerp(openColor, Color.clear, timeElapsed / cycleDuration);
                timeElapsed += Time.unscaledDeltaTime;
                yield return null;
            }
        }
        isMenuLoading = false;
    }

    public void HandleAction() {
        if (selectedElement == null) return;
        pMenuAction.HandleAction(selectedElement.GetComponent<MenuElementController>().menuActionType);
    }

    public void HandleMenuSelector(Vector2 movement) {
        if (selectedElement == null) return;
        if (movement.y >= 0.5f) {
            HandleSelection((Int16)(selectionIndex - 1));
        } else if (movement.y <= -0.5f) {
            HandleSelection((Int16)(selectionIndex + 1));
        }
    }

    private void HandleSelection(Int16 i) {
        if (i >= selectedElements.Count) {
            selectionIndex = 0;
        } else if (i < 0) {
            selectionIndex = (Int16)(selectedElements.Count - 1);
        } else {
            selectionIndex = i;
        }
        UpdateSelectedElement(state == MainMenuState.none ? null : selectedElements[selectionIndex]);
    }

    private void UpdateSelectedElement(GameObject o = null) {
        if (selectedElement != null)
            selectedElement.GetComponent<MenuElementController>().HandleSelection(false);
        selectedElement = o;
        if (selectedElement != null)
            selectedElement.GetComponent<MenuElementController>().HandleSelection(true);
    }

    public void UpdateSelection() {
        HandleSelection(selectionIndex);
    }

    public void ChangeSet(MainMenuState s) {
        UpdateSetActivation(false);
        state = s;
        UpdateSetActivation(true);
        HandleSelection(0);
    }

    public void HandleReturn() {
        switch(state) {
            case MainMenuState.defaultSet:
                pMenuAction.HandleAction(PMenuActionType.resume);
                break;
            case MainMenuState.saveSet or MainMenuState.loadSet or MainMenuState.confirmLeaveSet:
                pMenuAction.HandleAction(PMenuActionType.accessDefaultSection);
                break;
        }
    }

    private void UpdateSetActivation(bool active) {
        switch (state) {
            case MainMenuState.none:
                if (active) SetSelectableElements(new List<GameObject>());
                break;
            case MainMenuState.defaultSet:
                defaultSet.SetActive(active);
                if (active) SetSelectableElements(defaultSet.GetComponent<MenuSetController>().selectableElements);
                break;
            case MainMenuState.saveSet:
                saveSet.SetActive(active);
                if (active) {
                    SetSelectableElements(saveSet.GetComponent<MenuSetController>().selectableElements);
                    UpdateAllSaveSlots();
                }
                break;
            case MainMenuState.loadSet:
                loadSet.SetActive(active);
                if (active) {
                    SetSelectableElements(loadSet.GetComponent<MenuSetController>().selectableElements);
                    UpdateAllSaveSlots();
                }
                break;
            case MainMenuState.confirmLeaveSet:
                confirmLeaveSet.SetActive(active);
                if (active) SetSelectableElements(confirmLeaveSet.GetComponent<MenuSetController>().selectableElements);
                break;
        }
    }

    private void SetSelectableElements(List<GameObject> e) {
        selectedElements = e;
    }

    private void UpdateAllSaveSlots() {
        List<PlayerData> saves = SaveSystem.CheckExistingSaves();
        Int16 i = 0;
        if (state == MainMenuState.saveSet) {
            foreach (PlayerData s in saves) {
                i++;
                saveSet.transform.GetChild(i).gameObject.GetComponent<SaveSlotShort>().SetData(s);
            }
        } else if (state == MainMenuState.loadSet) {
            foreach (PlayerData s in saves) {
                i++;
                loadSet.transform.GetChild(i).gameObject.GetComponent<SaveSlotShort>().SetData(s);
            }
        }
    }
}
