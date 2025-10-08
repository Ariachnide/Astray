using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum HomeMenuState {
    none,
    loadScreen,
    gameDetailScreen,
    deleteScreen,
    newGameScreen
}

public enum HomeMenuActionType {
    none,
    displayGameDetail,
    backToLoadingScreen,
    displayDeleteWindow,
    cancelDelete,
    confirmDelete,
    continueAfterDelete,
    newGameSpecialCommand,
    letters,
    continueAfterCreation,
    load
}

public class HomeMenuUI : MonoBehaviour {
    public HomeMenuState state;
    [SerializeField]
    private GameObject
        backgroundLayer,
        loadSet,
        detailSet,
        headline,
        alphabetSet,
        newGameSpecialCommandsSet,
        confirmDeleteWindow,
        namePreview,
        confirmGameCreationWindow;

    private Int16
        verticalSelectionIndex,
        horizontalSelectionIndex,
        letterFontSizeDefault = 26,
        letterFontSizeSelected = 42,
        tempIndex,
        gameIndex = 1;
    public GameObject selectedElement;
    private List<List<GameObject>> newGameScreenElements;
    private List<GameObject> loadingScreenElements, gameDetailScreenElements;
    List<PlayerData> saves;
    [SerializeField]
    private SpawnPoint defaultSpawnPoint;

    private void Awake() {
        confirmDeleteWindow.SetActive(false);
        confirmGameCreationWindow.SetActive(false);
        UpdateContent(HomeMenuState.none);
        backgroundLayer.SetActive(true);
        loadingScreenElements = loadSet.GetComponent<MenuSetController>().selectableElements;
        gameDetailScreenElements = detailSet.GetComponent<MenuSetController>().selectableElements;
        newGameScreenElements = new List<List<GameObject>>() {
            alphabetSet.GetComponent<MenuSetController>().selectableElements,
            newGameSpecialCommandsSet.GetComponent<MenuSetController>().selectableElements
        };
        saves = SaveSystem.CheckExistingSaves();
    }

    private void Start() {
        backgroundLayer.GetComponent<DisplayUIElement>().DisplayScene(2f);
        UpdateContent(
            SaveSystem.CheckIfAnyExistingSaves()
                ? HomeMenuState.loadScreen
                : HomeMenuState.newGameScreen
        );
    }

    private void UpdateContent(HomeMenuState s) {
        state = s;
        selectedElement = null;
        horizontalSelectionIndex = verticalSelectionIndex = 0;
        switch (s) {
            case HomeMenuState.loadScreen:
                UpdateActiveSet(loadSet);
                headline.GetComponent<TMP_Text>().text = "Selectionnez une partie :";
                UpdateSaveSlots();
                verticalSelectionIndex = (Int16)(gameIndex - 1);
                HandleLoadSelection(verticalSelectionIndex);
                selectedElement = loadingScreenElements[verticalSelectionIndex];
                selectedElement.GetComponent<MenuElementController>().HandleSelection(true);
                break;
            case HomeMenuState.newGameScreen:
                UpdateActiveSet(alphabetSet);
                headline.GetComponent<TMP_Text>().text = "Choisissez un nom :";
                selectedElement = newGameScreenElements[verticalSelectionIndex][horizontalSelectionIndex];
                namePreview.GetComponent<NamePreviewManager>().SetDefault();
                selectedElement.GetComponent<RectTransform>().GetComponent<TMP_Text>().fontSize = letterFontSizeSelected;
                break;
            case HomeMenuState.gameDetailScreen:
                UpdateActiveSet(detailSet);
                headline.GetComponent<TMP_Text>().text = saves[gameIndex - 1].playerName;
                HandleGameDetailSelection(horizontalSelectionIndex);
                selectedElement = gameDetailScreenElements[horizontalSelectionIndex];
                selectedElement.GetComponent<MenuElementController>().HandleSelection(true);
                detailSet.GetComponent<DetailSetController>().LoadElements(saves[gameIndex - 1]);
                break;
            case HomeMenuState.deleteScreen:
                selectedElement = confirmDeleteWindow.GetComponent<ConfirmDeleteController>().ActivateWindow();
                selectedElement.GetComponent<MenuElementController>().HandleSelection(true);
                break;
            case HomeMenuState.none:
                headline.GetComponent<TMP_Text>().text = "";
                break;
        }
    }

    private void UpdateActiveSet(GameObject activeSet, List<GameObject> ignoredSets = null) {
        ignoredSets = ignoredSets ?? new List<GameObject>();
        activeSet.SetActive(true);
        if (loadSet.name != activeSet.name && !ignoredSets.Contains(loadSet)) loadSet.SetActive(false);
        if (alphabetSet.name != activeSet.name && !ignoredSets.Contains(alphabetSet)) alphabetSet.SetActive(false);
        if (detailSet.name != activeSet.name && !ignoredSets.Contains(detailSet)) detailSet.SetActive(false);
    }

    private IEnumerator LeavePage() {
        backgroundLayer.GetComponent<DisplayUIElement>().HideScene(Color.black);
        yield return new WaitForSecondsRealtime(1);
        SaveSystem.currentSlot = gameIndex;
        SceneManager.LoadScene("Main");
    }
    
    public void HandleAction() {
        if (state == HomeMenuState.none || selectedElement == null) return;
        switch (selectedElement.GetComponent<MenuElementController>().homeMenuActionType) {
            case HomeMenuActionType.displayGameDetail:
                if (selectedElement.GetComponent<SaveSlotBig>().hasData && SaveSystem.CheckIfFileExists(selectedElement.GetComponent<SaveSlotBig>().slotValue)) {
                    selectedElement.GetComponent<MenuElementController>().HandleSelection(false);
                    UpdateContent(HomeMenuState.gameDetailScreen);
                } else {
                    gameIndex = selectedElement.GetComponent<SaveSlotBig>().slotValue;
                    selectedElement.GetComponent<MenuElementController>().HandleSelection(false);
                    UpdateContent(HomeMenuState.newGameScreen);
                }
                break;
            case HomeMenuActionType.load:
                state = HomeMenuState.none;
                StartCoroutine(LeavePage());
                break;
            case HomeMenuActionType.backToLoadingScreen:
                detailSet.GetComponent<DetailSetController>().UnloadElements();
                selectedElement.GetComponent<MenuElementController>().HandleSelection(false);
                UpdateContent(HomeMenuState.loadScreen);
                break;
            case HomeMenuActionType.displayDeleteWindow:
                confirmDeleteWindow.GetComponent<ConfirmDeleteController>().ActivateWindow();
                selectedElement.GetComponent<MenuElementController>().isSelectionFrozen = true;
                tempIndex = horizontalSelectionIndex;
                UpdateContent(HomeMenuState.deleteScreen);
                break;
            case HomeMenuActionType.cancelDelete:
                confirmDeleteWindow.GetComponent<ConfirmDeleteController>().CloseWindow();
                selectedElement.GetComponent<MenuElementController>().HandleSelection(false);
                selectedElement = detailSet.GetComponent<DetailSetController>().GetDeleteElement();
                selectedElement.GetComponent<MenuElementController>().isSelectionFrozen = false;
                horizontalSelectionIndex = tempIndex;
                tempIndex = 0;
                state = HomeMenuState.gameDetailScreen;
                break;
            case HomeMenuActionType.confirmDelete:
                confirmDeleteWindow.GetComponent<ConfirmDeleteController>().GoToWait();
                UpdateSelectedElement();
                StartCoroutine(HandleFileDelete(gameIndex));
                break;
            case HomeMenuActionType.continueAfterDelete:
                confirmDeleteWindow.GetComponent<ConfirmDeleteController>().CloseWindow();
                selectedElement.GetComponent<MenuElementController>().HandleSelection(false);
                detailSet.GetComponent<DetailSetController>().UnloadElements();
                detailSet
                    .GetComponent<DetailSetController>()
                    .GetDeleteElement()
                    .GetComponent<MenuElementController>()
                    .HandleSelection(false);
                tempIndex = 0;
                UpdateContent(HomeMenuState.loadScreen);
                break;
            case HomeMenuActionType.newGameSpecialCommand:
                switch (selectedElement.GetComponent<AlphabetSpecCommand>().type) {
                    case AlphabetSpecCommandType.back:
                        if (namePreview.GetComponent<NamePreviewManager>().index == 0) {
                            LeaveNewGameScreen();
                            UpdateContent(HomeMenuState.loadScreen);
                        } else {
                            namePreview.GetComponent<NamePreviewManager>().RemoveLetter();
                        }
                        break;
                    case AlphabetSpecCommandType.maj:
                        HandleMajSpecCommand();
                        break;
                    case AlphabetSpecCommandType.end:
                        if (namePreview.GetComponent<NamePreviewManager>().value.Length == 0) break;
                        confirmGameCreationWindow.GetComponent<ConfirmGameCreationController>().ActivateWindow();
                        UpdateSelectedElement();
                        StartCoroutine(HandleFileCreation(gameIndex));
                        break;
                }
                break;
            case HomeMenuActionType.letters:
                NamePreviewManager npm = namePreview.GetComponent<NamePreviewManager>();
                npm.WriteLetter(selectedElement.GetComponent<LetterItem>().GetLetter());
                if (npm.value.Length == npm.GetPreviewLength()) {
                    verticalSelectionIndex = 1;
                    horizontalSelectionIndex = 2;
                    selectedElement.GetComponent<RectTransform>().GetComponent<TMP_Text>().fontSize = letterFontSizeDefault;
                    selectedElement = newGameSpecialCommandsSet
                        .GetComponent<MenuSetController>()
                        .selectableElements.Find(
                            e => e.GetComponent<AlphabetSpecCommand>().type == AlphabetSpecCommandType.end
                        );
                    selectedElement.GetComponent<MenuElementController>().HandleSelection(true);
                }
                break;
            case HomeMenuActionType.continueAfterCreation:
                LeaveNewGameScreen();
                confirmGameCreationWindow.GetComponent<ConfirmGameCreationController>().CloseWindow();
                UpdateContent(HomeMenuState.loadScreen);
                break;
        }
    }

    private IEnumerator HandleFileDelete(Int16 v) {
        SaveSystem.DeleteSave(v);
        yield return new WaitForSecondsRealtime(0.5f);
        while (true) {
            if (SaveSystem.CheckIfFileExists(v)) {
                yield return new WaitForSecondsRealtime(0.5f);
            } else {
                UpdateSelectedElement(confirmDeleteWindow.GetComponent<ConfirmDeleteController>().GoToPositiveResponse());
                break;
            }
        }
    }

    private IEnumerator HandleFileCreation(Int16 v) {
        SaveSystem.SavePlayer(
            new PlayerData() {
                playerName = namePreview.GetComponent<NamePreviewManager>().value,
                saveTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm"),
                playTime = TimeSpan.Zero.ToString(@"hh\:mm\:ss"),
                mapName = defaultSpawnPoint.area.mapName,
                savedElements = new List<string>() { "black_dress" },
                equippedElements = new List<string>() { "black_dress" },
                regElm = new List<string>(),
                maxHP = 4,
                hitPoints = 4,
                maxMana = 0,
                currentMana = 0,
                maxOrb = 100,
                orbs = 0,
                maxBomb = 0,
                bombs = 0,
                spawnPointName = defaultSpawnPoint.name,
                spawnPointLocationName = defaultSpawnPoint.locationName,
                storyStatus = StoryState.Prologue.ToString(),
                storyMilestones = new List<StoryMilestoneRawList>(),
            },
            v
        );
        while(true) {
            if (!SaveSystem.CheckIfFileExists(v)) {
                yield return 0;
            } else {
                UpdateSelectedElement(confirmGameCreationWindow.GetComponent<ConfirmGameCreationController>().GoToPositiveResponse());
                break;
            }
        }
    }

    private void LeaveNewGameScreen() {
        List<GameObject> specCommands = newGameSpecialCommandsSet.GetComponent<MenuSetController>().selectableElements;
        foreach (GameObject go in specCommands)
            go.GetComponent<MenuElementController>().HandleSelection(false);
        Transform majButtonElement = specCommands
            .Find(e => e.GetComponent<AlphabetSpecCommand>().type == AlphabetSpecCommandType.maj)
            .transform.GetChild(0);
        if (majButtonElement.GetComponent<TMP_Text>().fontStyle == FontStyles.UpperCase) {
            foreach (GameObject l in alphabetSet.GetComponent<MenuSetController>().selectableElements)
                l.GetComponent<TMP_Text>().fontStyle = FontStyles.UpperCase;
            majButtonElement.GetComponent<TMP_Text>().fontStyle = FontStyles.LowerCase;
        }
    }

    private void HandleMajSpecCommand() {
        Transform buttonElement = newGameSpecialCommandsSet
            .GetComponent<MenuSetController>()
            .selectableElements.Find(
                e => e.GetComponent<AlphabetSpecCommand>().type == AlphabetSpecCommandType.maj
            ).transform.GetChild(0);
        if (buttonElement.GetComponent<TMP_Text>().fontStyle == FontStyles.UpperCase) {
            foreach (GameObject l in alphabetSet.GetComponent<MenuSetController>().selectableElements)
                l.GetComponent<TMP_Text>().fontStyle = FontStyles.UpperCase;
            buttonElement.GetComponent<TMP_Text>().fontStyle = FontStyles.LowerCase;
        } else {
            foreach (GameObject l in alphabetSet.GetComponent<MenuSetController>().selectableElements)
                l.GetComponent<TMP_Text>().fontStyle = FontStyles.LowerCase;
            buttonElement.GetComponent<TMP_Text>().fontStyle = FontStyles.UpperCase;
        }
    }

    public void HandleMenuSelector(Vector2 movement) {
        switch (state) {
            case HomeMenuState.loadScreen:
                if (movement.y >= 0.5f) {
                    HandleLoadSelection((Int16)(verticalSelectionIndex - 1));
                    UpdateSelectedElement(loadingScreenElements[verticalSelectionIndex]);
                } else if (movement.y <= -0.5f) {
                    HandleLoadSelection((Int16)(verticalSelectionIndex + 1));
                    UpdateSelectedElement(loadingScreenElements[verticalSelectionIndex]);
                }
                break;
            case HomeMenuState.deleteScreen:
                if (selectedElement == null || selectedElement.GetComponent<MenuElementController>().homeMenuActionType == HomeMenuActionType.continueAfterDelete) break;
                if (movement.x >= 0.5f || movement.x <= -0.5f) UpdateSelectedElement(confirmDeleteWindow.GetComponent<ConfirmDeleteController>().SwitchResponse());
                break;
            case HomeMenuState.newGameScreen:
                if (selectedElement.GetComponent<MenuElementController>().homeMenuActionType == HomeMenuActionType.continueAfterCreation) break;
                if (verticalSelectionIndex == 0) selectedElement.GetComponent<RectTransform>().GetComponent<TMP_Text>().fontSize = letterFontSizeDefault;
                if (movement.y >= 0.5f) {
                    MoveUpCursorNewGame();
                } else if (movement.y <= -0.5f) {
                    MoveDownCursorNewGame();
                }
                if (movement.x >= 0.5f) {
                    MoveRightCursorNewGame();
                } else if (movement.x <= -0.5f) {
                    MoveLeftCursorNewGame();
                }
                UpdateNewGameSelectionElement(newGameScreenElements[verticalSelectionIndex][horizontalSelectionIndex]);
                if (verticalSelectionIndex == 0) selectedElement.GetComponent<RectTransform>().GetComponent<TMP_Text>().fontSize = letterFontSizeSelected;
                break;
            case HomeMenuState.gameDetailScreen:
                if (movement.x >= 0.5f) {
                    HandleGameDetailSelection((Int16)(horizontalSelectionIndex + 1));
                    UpdateSelectedElement(gameDetailScreenElements[horizontalSelectionIndex]);
                } else if (movement.x <= -0.5f) {
                    HandleGameDetailSelection((Int16)(horizontalSelectionIndex - 1));
                    UpdateSelectedElement(gameDetailScreenElements[horizontalSelectionIndex]);
                }
                break;
            case HomeMenuState.none:
                break;
        }
    }

    private void HandleLoadSelection(Int16 i) {
        if (i >= loadingScreenElements.Count) {
            verticalSelectionIndex = 0;
        } else if (i < 0) {
            verticalSelectionIndex = (Int16)(loadingScreenElements.Count - 1);
        } else {
            verticalSelectionIndex = i;
        }
        gameIndex = (Int16)(verticalSelectionIndex + 1);
    }

    private void HandleGameDetailSelection(Int16 i) {
        if (i >= gameDetailScreenElements.Count) {
            horizontalSelectionIndex = 0;
        } else if (i < 0) {
            horizontalSelectionIndex = (Int16)(gameDetailScreenElements.Count - 1);
        } else {
            horizontalSelectionIndex = i;
        }
    }

    private void MoveUpCursorNewGame() {
        if (verticalSelectionIndex == 0) {
            if (horizontalSelectionIndex < 9) {
                verticalSelectionIndex = 1;
                horizontalSelectionIndex /= 3;
            } else {
                horizontalSelectionIndex -= 9;
            }
        } else {
            selectedElement.GetComponent<MenuElementController>().HandleSelection(false);
            verticalSelectionIndex = 0;
            horizontalSelectionIndex = (Int16)(19 + horizontalSelectionIndex * 3);
        }
    }

    private void MoveDownCursorNewGame() {
        if (verticalSelectionIndex == 0) {
            if (horizontalSelectionIndex > 17) {
                verticalSelectionIndex = 1;
                horizontalSelectionIndex = (Int16)((horizontalSelectionIndex - 18) /  3);
            } else {
                horizontalSelectionIndex += 9;
            }
        } else {
            selectedElement.GetComponent<MenuElementController>().HandleSelection(false);
            verticalSelectionIndex = 0;
            horizontalSelectionIndex = (Int16)(horizontalSelectionIndex * 3 + 1);
        }
    }

    private void MoveLeftCursorNewGame() {
        if (verticalSelectionIndex == 0) {
            if (horizontalSelectionIndex == 0 || horizontalSelectionIndex == 9 || horizontalSelectionIndex == 18) {
                horizontalSelectionIndex += 8;
            } else {
                horizontalSelectionIndex--;
            }
        } else {
            if (horizontalSelectionIndex == 0) {
                horizontalSelectionIndex = 2;
            } else {
                horizontalSelectionIndex--;
            }
        }
    }

    private void MoveRightCursorNewGame() {
        if (verticalSelectionIndex == 0) {
            if (horizontalSelectionIndex == 8 || horizontalSelectionIndex == 17 || horizontalSelectionIndex == 26) {
                horizontalSelectionIndex -= 8;
            } else {
                horizontalSelectionIndex++;
            }
        } else {
            if (horizontalSelectionIndex == 2) {
                horizontalSelectionIndex = 0;
            } else {
                horizontalSelectionIndex++;
            }
        }
    }

    private void UpdateNewGameSelectionElement(GameObject o) {
        if (verticalSelectionIndex == 0) {
            selectedElement = o;
        } else {
            UpdateSelectedElement(o);
        }
    }

    private void UpdateSelectedElement(GameObject o = null) {
        if (selectedElement) selectedElement.GetComponent<MenuElementController>().HandleSelection(false);
        selectedElement = o;
        if (selectedElement) selectedElement.GetComponent<MenuElementController>().HandleSelection(true);
    }

    public void UpdateSelection() {
        HandleLoadSelection(verticalSelectionIndex);
    }

    private void UpdateSaveSlots() {
        saves = SaveSystem.CheckExistingSaves();
        Int16 i = -1;
        foreach (PlayerData s in saves) {
            i++;
            loadSet.transform.GetChild(i).gameObject.GetComponent<SaveSlotBig>().SetData(s);
        }
    }
}
