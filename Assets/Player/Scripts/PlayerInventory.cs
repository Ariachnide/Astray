using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuitAnimatorValue {
    public Int16 Value { get; set; }
    public string Name { get; set; }
}

public class PlayerInventory : MonoBehaviour {
    public Int16 rupees, maxRupee, bombs, maxBomb;
    public bool canAccessInventory, isWindowOpen;
    private bool isWindowSliding, shouldOpenWindow;
    private PlayerState prevState;
    private List<PlayerState> inventoryAuthorizedStates;
    private Animator animator;
    private PlayerMain main;
    private PlayTime playTime;
    private PlayerAction action;
    private PlayerSpell spell;
    private InventoryManager inventoryManager;
    private List<SuitAnimatorValue> suitAnimatorValues;

    [SerializeField]
    private GameObject
        heartContainerBox,
        manaContainerBox,
        uiRupeeCounter,
        uiRupeeIcon,
        inventoryWindow,
        globalMap,
        storySwitch;

    [SerializeField]
    private SpawnPoint defaultSpawnPoint;

    private void Awake() {
        main = GetComponent<PlayerMain>();
        playTime = GetComponent<PlayTime>();
        action = GetComponent<PlayerAction>();
        spell = GetComponent<PlayerSpell>();
        main.backgroundLayer.SetActive(true);
        inventoryWindow.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -Screen.height);
        inventoryAuthorizedStates = new List<PlayerState>() {
            PlayerState.idle,
            PlayerState.walk,
            PlayerState.usingWeapon,
            PlayerState.usingSpell,
            PlayerState.knockbacked,
            PlayerState.inInventory
        };
        inventoryManager = inventoryWindow.GetComponent<InventoryManager>();
        animator = GetComponent<Animator>();
        suitAnimatorValues = new List<SuitAnimatorValue> {
            new() {
                Value = 1,
                Name = "black_dress"
            },
            new() {
                Value = 2,
                Name = "green_dress"
            }
        };
        canAccessInventory = true;
    }

    private void Start() {
        GetSavedData();
    }

    private void GetSavedData() {
        Int16 staticSlot = SaveSystem.currentSlot;
        PlayerData d = new();
        if (staticSlot != 0) {
            d = SaveSystem.LoadSave(staticSlot);
        } else {
            bool hasLoaded = false;
            for (Int16 i = 1; i <= 3; i++) {
                if (SaveSystem.CheckIfFileExists(i)) {
                    d = SaveSystem.LoadSave(i);
                    SaveSystem.currentSlot = i;
                    hasLoaded = true;
                    break;
                }
            }
            if (!hasLoaded) { // MUST UPDATE THAT
                Debug.Log("LOG DEFAULT FILE");
                d = SaveSystem.GetDefaultFile();
                d.spawnPointName = defaultSpawnPoint.name;
                d.spawnPointLocationName = defaultSpawnPoint.locationName;
                d.orbs = 50;
                d.maxHP = 12;
                d.hitPoints = 12;
                d.maxMana = 12;
                d.currentMana = 12;
                d.mapName = "StartGrotto";
            }
        }

        main.playerName = d.playerName;

        heartContainerBox.GetComponent<HeartsUI>().SetupHealthBar(d.maxHP, d.hitPoints);
        main.maxHP = d.maxHP;
        main.hitPoints = d.hitPoints;
        heartContainerBox.GetComponent<HeartsUI>().HandlePulsing(true);

        manaContainerBox.GetComponent<ManaUI>().SetupManaBar(d.maxMana, d.currentMana);
        spell.maxMana = d.maxMana;
        spell.manaPoints = d.currentMana;
        manaContainerBox.GetComponent<ManaUI>().HandleShowActiveSlot(true);

        SetMaxRupee(d.maxOrb);
        UpdateRupees(d.orbs);

        playTime.StartRecording(d.playTime);

        maxBomb = d.maxBomb;
        bombs = d.bombs;

        List<string> savedElements = d.savedElements;
        inventoryManager.DispatchElementsInSlots(savedElements);

        List<string> equippedElements = d.equippedElements;
        inventoryManager.SetEquippedElements(equippedElements);

        StoryTracker.SetupStoryState(d.storyStatus);
        storySwitch.GetComponent<StorySwitch>().SetupEvents(d.storyMilestones);

        main.currentSpawnPoint = globalMap.GetComponent<GlobalMapHandler>().LoadInitialSetup(d.mapName, d.spawnPointName);
        globalMap.GetComponent<GlobalRegisteredElementsController>().PopulateRegistry(d.regElm);

        main.backgroundLayer.GetComponent<DisplayUIElement>().DisplayScene(0.5f);
    }

    public List<string> GetElementsInSlots() {
        return inventoryManager.GetElementsInSlots();
    }

    public List<string> GetEquippedElements() {
        return inventoryManager.GetEquippedElements();
    }

    public void SetItemInInventory(string itemName) {
        inventoryManager.AddNewElement(itemName);
    }

    public void SetMaxRupee(Int16 v) {
        maxRupee = v;
        uiRupeeCounter.GetComponent<RupeeCounter>().SetMaxRupee(v);
    }

    public void UpdateRupees(Int16 v) {
        rupees += v;
        if (v > 0) {
            if (rupees > maxRupee)
                rupees = maxRupee;
            uiRupeeCounter.GetComponent<RupeeCounter>().UpdateText(rupees);
        } else if (v < 0) {
            if (rupees < 0)
                rupees = 0;
            uiRupeeCounter.GetComponent<RupeeCounter>().UpdateText(rupees);
        }
    }

    public void HandleInventoryWindow() {
        if (canAccessInventory && inventoryAuthorizedStates.Contains(main.state)) {
            if (Time.timeScale != 0) {
                main.SetMovement(Vector2.zero);
                Time.timeScale = 0;
            }
            if (main.state != PlayerState.inInventory) {
                prevState = main.state;
                main.state = PlayerState.inInventory;
            }
            shouldOpenWindow = !shouldOpenWindow;
            if (!isWindowSliding)
                StartCoroutine(WindowSlide());
        }
    }

    private IEnumerator WindowSlide() {
        isWindowSliding = true;
        action.SuspendActionStatus();

        while (true) {
            if (shouldOpenWindow) {
                if (inventoryWindow.transform.position.y < (Screen.height / 2)) {
                    inventoryWindow.transform.Translate(new Vector2(0, 40));
                    yield return new WaitForSecondsRealtime(0.02f);
                } else {
                    isWindowOpen = true;
                    inventoryManager.HandleOpening(true);
                    break;
                }
            } else {
                if (inventoryWindow.transform.position.y > -(Screen.height / 2)) {
                    inventoryWindow.transform.Translate(new Vector2(0, -40));
                    yield return new WaitForSecondsRealtime(0.02f);
                } else {
                    isWindowOpen = false;
                    break;
                }
            }
        }
        if (!isWindowOpen) {
            action.SetPauseStatus(PauseActionType.none);
            inventoryManager.HandleOpening(false);
            main.state = prevState;
            Time.timeScale = 1;
        }
        isWindowSliding = false;
    }

    public void HandleInventorySelector(Vector2 movement) {
        inventoryManager.MoveSelector(movement);
    }

    public bool GetSpellChangeAuth() {
        return !animator.GetBool("isUsingSpell");
    }

    public bool GetWeaponChangeAuth() {
        return !animator.GetBool("isUsingWeapon");
    }

    public bool GetSuitChangeAuth() {
        return !animator.GetBool("isUsingWeapon") && !animator.GetBool("isUsingSpell");
    }

    public void HandleSuitChange(string suitName) {
        suitAnimatorValues.Find(v => {
            if (suitName == v.Name) {
                animator.SetInteger("suit", v.Value);
                return true;
            }
            return false;
        });
    }

    public InventoryElement GetEquippedSuit() {
        return inventoryManager.equippedSuit.GetComponent<InventorySlotManager>().element;
    }

    public InventoryElement GetEquippedWeapon() {
        return inventoryManager.equippedWeapon
            ? inventoryManager.equippedWeapon.GetComponent<InventorySlotManager>().element
            : inventoryManager.emptyElement;
    }

    public InventoryElement GetEquippedSpell() {
        return inventoryManager.equippedSpell
            ? inventoryManager.equippedSpell.GetComponent<InventorySlotManager>().element
            : inventoryManager.emptyElement;
    }

    public InventoryElement GetEquippedItem() {
        return inventoryManager.equippedItem
            ? inventoryManager.equippedItem.GetComponent<InventorySlotManager>().element
            : inventoryManager.emptyElement;
    }
}
