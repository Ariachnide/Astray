using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour {
    public InventoryElement emptyElement;
    [SerializeField]
    private List<GameObject> spellSlots, weaponSlots, suitSlots, itemSlots;
    [SerializeField]
    private GameObject selector, player, fullPlayerView, messageBox, availableItemsGo;
    private AvailableElements availableElementsScr;
    private GameObject selectedElement;
    public GameObject equippedSpell, equippedWeapon, equippedSuit, equippedItem;
    [SerializeField]
    private List<Sprite> fullPlayerViewSprites;
    private List<string> warnMessages;

    private void Awake() {
        warnMessages = new List<string>();
        availableElementsScr = availableItemsGo.GetComponent<AvailableElements>();
        equippedSuit = suitSlots.Find(s => s.GetComponent<InventorySlotManager>().element.elementName == "black_dress");
    }

    private void SetSelectorDefaultPosition() {
        UpdateSelectedElement(GetFirstElementAvailable());
        selector.GetComponent<SelectorInventory>().selectedElementIndex = 0;
        selector.GetComponent<SelectorInventory>().selectedElementType = selectedElement.GetComponent<InventorySlotManager>().element.type;
        selector.transform.position = selectedElement.transform.position;
    }

    public List<string> GetElementsInSlots() {
        List<string> e = new List<string>();
        List<List<GameObject>> allSlots = new List<List<GameObject>> {
            spellSlots,
            weaponSlots,
            suitSlots,
            itemSlots
        };
        foreach (List<GameObject> slots in allSlots) {
            foreach (GameObject go in slots) {
                if (go.GetComponent<InventorySlotManager>().isUsed) {
                    e.Add(go.GetComponent<InventorySlotManager>().element.elementName);
                }
            }
        }
        return e;
    }

    public List<string> GetEquippedElements() {
        List<string> e = new();
        List<List<GameObject>> allSlots = new() {
            spellSlots,
            weaponSlots,
            suitSlots,
            itemSlots
        };
        foreach (List<GameObject> slots in allSlots) {
            GameObject equippedGo = null;
            foreach (GameObject go in slots) {
                equippedGo = slots.Find(s => s.GetComponent<InventorySlotManager>().isEquipped);
            }
            if (equippedGo != null) {
                e.Add(equippedGo.GetComponent<InventorySlotManager>().element.elementName);
            }
        }
        return e;
    }

    public void DispatchElementsInSlots(List<string> elementNames) {
        foreach (string s in elementNames) {
            if (s == "black_dress")
                continue;
            InventoryElement ie = availableElementsScr.GetElementByName(s);
            switch (ie.type) {
                case InventoryElementType.spell:
                    GetFirstSpellSlotAvailable(ie);
                    break;
                case InventoryElementType.weapon:
                    GetFirstWeaponSlotAvailable(ie);
                    break;
                case InventoryElementType.suit:
                    GetFirstSuitSlotAvailable(ie);
                    break;
                case InventoryElementType.item:
                    GetFirstItemSlotAvailable(ie);
                    break;
            }
        }
    }

    public void AddNewElement(string elementName) {
        InventoryElement ie = availableElementsScr.GetElementByName(elementName);
        switch (ie.type) {
            case InventoryElementType.spell:
                GetFirstSpellSlotAvailable(ie);
                break;
            case InventoryElementType.weapon:
                GetFirstWeaponSlotAvailable(ie);
                break;
            case InventoryElementType.suit:
                GetFirstSuitSlotAvailable(ie);
                break;
            case InventoryElementType.item:
                GetFirstItemSlotAvailable(ie);
                break;
        }
    }

    public void SetEquippedElements(List<string> elementsNames) {
        foreach (string s in elementsNames) {
            InventoryElement ie = availableElementsScr.GetElementByName(s);
            switch (ie.type) {
                case InventoryElementType.spell:
                    HandleSpellChange(spellSlots.Find(s => s.GetComponent<InventorySlotManager>().element.elementName == ie.elementName), true);
                    break;
                case InventoryElementType.weapon:
                    HandleWeaponChange(weaponSlots.Find(s => s.GetComponent<InventorySlotManager>().element.elementName == ie.elementName), true);
                    break;
                case InventoryElementType.suit:
                    HandleSuitChange(suitSlots.Find(s => s.GetComponent<InventorySlotManager>().element.elementName == ie.elementName), true);
                    break;
                case InventoryElementType.item:
                    HandleItemChange(itemSlots.Find(s => s.GetComponent<InventorySlotManager>().element.elementName == ie.elementName), true);
                    break;
            }
        }
    }

    private void UpdateSelectedElement(GameObject element) {
        selectedElement = element;
        UpdateAction(element.GetComponent<InventorySlotManager>());
    }

    private void UpdateAction(InventorySlotManager manager) {
        if (manager.isUsed) {
            bool canInteract = false;
            warnMessages = new List<string>();
            bool shouldWarn = false;
            switch (manager.element.type) {
                case InventoryElementType.spell:
                    if (player.GetComponent<PlayerInventory>().GetSpellChangeAuth()) {
                        canInteract = true;
                    } else {
                        shouldWarn = true;
                        warnMessages = new List<string> { "Impossible de changer le sortilège pendant une incantation." };
                    }
                    break;
                case InventoryElementType.weapon:
                    if (player.GetComponent<PlayerInventory>().GetWeaponChangeAuth()) {
                        canInteract = true;
                    } else {
                        shouldWarn = true;
                        warnMessages = new List<string> { "Impossible de changer l'arme pendant une attaque." };
                    }
                    break;
                case InventoryElementType.suit:
                    if (equippedSuit.GetComponent<InventorySlotManager>().element.elementName == manager.element.elementName)
                        break;
                    if (player.GetComponent<PlayerInventory>().GetSuitChangeAuth()) {
                        canInteract = true;
                    } else {
                        shouldWarn = true;
                        warnMessages = new List<string> {"Impossible de changer de tenue pendant une attaque ou un sortilège."};
                    }
                    break;
                case InventoryElementType.item:
                    // if (player.GetComponent<PlayerItem>().GetItemChangeAuth())
                    canInteract = true;
                    break;
            }
            if (!manager.isEquipped) {
                if (canInteract) {
                    player.GetComponent<PlayerAction>().SetPauseStatus(
                        PauseActionType.equip//, "Equiper"
                    );
                } else if (shouldWarn) {
                    player.GetComponent<PlayerAction>().SetPauseStatus(PauseActionType.displayWarning);
                } else {
                    player.GetComponent<PlayerAction>().SetPauseStatus(PauseActionType.none);
                }
            } else {
                if (canInteract) {
                    if (manager.element.type == InventoryElementType.suit) {
                        player.GetComponent<PlayerAction>().SetPauseStatus(PauseActionType.none);
                    } else {
                        player.GetComponent<PlayerAction>().SetPauseStatus(
                            PauseActionType.unequip//, "Retirer"
                        );
                    }
                } else {
                    if (shouldWarn) {
                        player.GetComponent<PlayerAction>().SetPauseStatus(PauseActionType.displayWarning);
                    } else {
                        player.GetComponent<PlayerAction>().SetPauseStatus(PauseActionType.none);
                    }
                }
            }
        } else {
            player.GetComponent<PlayerAction>().SetPauseStatus(PauseActionType.none);
        }
    }

    public void DisplayWarnMessage() {
        List<MessageItem> messageItems = new List<MessageItem>();
        foreach (string m in warnMessages) {
            messageItems.Add(
                new MessageItem {
                    TxtContent = m,
                    BoxType = MsgBoxType.blue,
                    Alignment = TextAlignmentOptions.Center,
                    PrintingSpeed = MsgPrintingSpeed.instant,
                }
            );
        }
        messageBox.GetComponent<MessageBox>().ActivateMessageBox(messageItems);
        player.GetComponent<PlayerAction>().SetPauseStatus(PauseActionType.none);
    }

    public void EquipSelectedElement() {
        switch (selectedElement.GetComponent<InventorySlotManager>().element.type) {
            case InventoryElementType.spell:
                HandleSpellChange(selectedElement, true);
                break;
            case InventoryElementType.weapon:
                if (player.GetComponent<PlayerInventory>().GetWeaponChangeAuth())
                    HandleWeaponChange(selectedElement, true);
                break;
            case InventoryElementType.suit:
                if (player.GetComponent<PlayerInventory>().GetSuitChangeAuth())
                    HandleSuitChange(selectedElement, true);
                break;
            case InventoryElementType.item:
                HandleItemChange(selectedElement, true);
                break;
        }
        UpdateAction(selectedElement.GetComponent<InventorySlotManager>());
    }

    public void UnequipSelectedElement() {
        switch (selectedElement.GetComponent<InventorySlotManager>().element.type) {
            case InventoryElementType.spell:
                HandleSpellChange(selectedElement, false);
                break;
            case InventoryElementType.weapon:
                HandleWeaponChange(selectedElement, false);
                break;
            case InventoryElementType.suit:
                HandleSuitChange(selectedElement, false);
                break;
            case InventoryElementType.item:
                HandleItemChange(selectedElement, false);
                break;
        }
        UpdateAction(selectedElement.GetComponent<InventorySlotManager>());
    }

    private void HandleSpellChange(GameObject spell, bool shouldEquip) {
        if (shouldEquip) {
            if (equippedSpell)
                equippedSpell.GetComponent<InventorySlotManager>().HandleIsEquipped(false);
            equippedSpell = spell;
            InventorySlotManager manager = spell.GetComponent<InventorySlotManager>();
            manager.HandleIsEquipped(true);
            player.GetComponent<PlayerSpell>().HandleChange(manager.element.elementName, manager.element.icon);
        } else {
            if (spell == null && equippedSpell) {
                equippedSpell.GetComponent<InventorySlotManager>().HandleIsEquipped(false);
                equippedSpell = null;
                player.GetComponent<PlayerSpell>().HandleChange("");
            } else if (
                equippedSpell &&
                equippedSpell.GetComponent<InventorySlotManager>().element.elementName ==
                spell.GetComponent<InventorySlotManager>().element.elementName
            ) {
                equippedSpell.GetComponent<InventorySlotManager>().HandleIsEquipped(false);
                equippedSpell = null;
                player.GetComponent<PlayerSpell>().HandleChange("");
            }
        }
    }

    private void HandleWeaponChange(GameObject weapon, bool shouldEquip) {
        if (shouldEquip) {
            if (equippedWeapon)
                equippedWeapon.GetComponent<InventorySlotManager>().HandleIsEquipped(false);
            equippedWeapon = weapon;
            InventorySlotManager manager = weapon.GetComponent<InventorySlotManager>();
            manager.HandleIsEquipped(true);
            player.GetComponent<PlayerWeapon>().HandleChange(manager.element.elementName, manager.element.icon);
            UpdateFullPlayerView();
        } else {
            if (weapon == null && equippedWeapon) {
                equippedWeapon.GetComponent<InventorySlotManager>().HandleIsEquipped(false);
                equippedWeapon = null;
                player.GetComponent<PlayerWeapon>().HandleChange("");
                UpdateFullPlayerView();
            } else if (
                equippedWeapon &&
                equippedWeapon.GetComponent<InventorySlotManager>().element.elementName ==
                weapon.GetComponent<InventorySlotManager>().element.elementName
            ) {
                equippedWeapon.GetComponent<InventorySlotManager>().HandleIsEquipped(false);
                equippedWeapon = null;
                player.GetComponent<PlayerWeapon>().HandleChange("");
                UpdateFullPlayerView();
            }
        }
    }

    private void HandleSuitChange(GameObject suit, bool shouldEquip) {
        InventorySlotManager prevManager = equippedSuit.GetComponent<InventorySlotManager>();
        if (shouldEquip) {
            prevManager.HandleIsEquipped(false);
            equippedSuit = suit;
            InventorySlotManager manager = suit.GetComponent<InventorySlotManager>();
            manager.HandleIsEquipped(true);
            player.GetComponent<PlayerInventory>().HandleSuitChange(manager.element.elementName);
            UpdateFullPlayerView();
        } else {
            if (
                (suit == null && prevManager.element.elementName == "black_dress") ||
                (suit != null && (suit.GetComponent<InventorySlotManager>().element.elementName == "black_dress"))
            ) {
                Debug.LogError("CANNOT UNEQUIP BLACK DRESS WHICH IS DEFAULT SUIT");
                return;
            }
            if (suit == null) {
                prevManager.HandleIsEquipped(false);
                equippedSuit = suitSlots.Find(s => s.GetComponent<InventorySlotManager>().element.elementName == "black_dress");
                InventorySlotManager manager = equippedSuit.GetComponent<InventorySlotManager>();
                manager.HandleIsEquipped(true);
                player.GetComponent<PlayerInventory>().HandleSuitChange(manager.element.elementName);
                UpdateFullPlayerView();
            } else if (prevManager.element.elementName == suit.GetComponent<InventorySlotManager>().element.elementName) {
                prevManager.HandleIsEquipped(false);
                equippedSuit = suit;
                InventorySlotManager manager = suit.GetComponent<InventorySlotManager>();
                manager.HandleIsEquipped(true);
                player.GetComponent<PlayerInventory>().HandleSuitChange(manager.element.elementName);
                UpdateFullPlayerView();
            }
        }
    }

    private void HandleItemChange(GameObject item, bool shouldEquip) {
        if (shouldEquip) {
            if (equippedItem)
                equippedItem.GetComponent<InventorySlotManager>().HandleIsEquipped(false);
            equippedItem = item;
            InventorySlotManager manager = item.GetComponent<InventorySlotManager>();
            manager.HandleIsEquipped(true);
            player.GetComponent<PlayerItem>().HandleChange(manager.element.elementName, manager.element.icon);
        } else {
            if (item == null && equippedItem) {
                equippedItem.GetComponent<InventorySlotManager>().HandleIsEquipped(false);
                equippedItem = null;
                player.GetComponent<PlayerItem>().HandleChange("");
            } else if (
                equippedItem &&
                equippedItem.GetComponent<InventorySlotManager>().element.elementName ==
                item.GetComponent<InventorySlotManager>().element.elementName
            ) {
                equippedItem.GetComponent<InventorySlotManager>().HandleIsEquipped(false);
                equippedItem = null;
                player.GetComponent<PlayerItem>().HandleChange("");
            }
        }
    }

    private void UpdateFullPlayerView() {
        string imgName = "fullplayer-";
        switch (equippedSuit.GetComponent<InventorySlotManager>().element.elementName) {
            case "black_dress":
                imgName += "blackdress";
                break;
            case "green_dress":
                imgName += "greendress";
                break;
            case "":
                Debug.LogError("NO SUIT FOUND FOR FULL PLAYER VIEW - DEFAULT IS BLACK DRESS");
                break;
            default:
                Debug.LogError($"UNKNOW SUIT NAME: {equippedSuit.GetComponent<InventorySlotManager>().element.elementName}");
                break;
        }
        if (equippedWeapon)
            switch (equippedWeapon.GetComponent<InventorySlotManager>().element.elementName) {
                case "staff":
                    imgName += "-staff";
                    break;
                case "sword":
                    imgName += "-sword";
                    break;
            }
        fullPlayerView.GetComponent<Image>().sprite = fullPlayerViewSprites.Find(sprite => sprite.name == imgName);
    }

    private void GetFirstSpellSlotAvailable(InventoryElement element) {
        foreach (GameObject go in spellSlots) {
            if (!go.GetComponent<InventorySlotManager>().isUsed) {
                go.GetComponent<InventorySlotManager>().SetElement(element);
                return;
            }
        }
        Debug.LogError($"NO SPELL SLOT AVAILABLE - ELEMENT COULD NOT BE ADDED TO INVENTORY: {element.elementName}");
    }

    private void GetFirstWeaponSlotAvailable(InventoryElement element) {
        foreach (GameObject go in weaponSlots) {
            if (!go.GetComponent<InventorySlotManager>().isUsed) {
                go.GetComponent<InventorySlotManager>().SetElement(element);
                return;
            }
        }
        Debug.LogError($"NO WEAPON SLOT AVAILABLE - ELEMENT COULD NOT BE ADDED TO INVENTORY: {element.elementName}");
    }

    private void GetFirstSuitSlotAvailable(InventoryElement element) {
        foreach (GameObject go in suitSlots) {
            if (!go.GetComponent<InventorySlotManager>().isUsed) {
                go.GetComponent<InventorySlotManager>().SetElement(element);
                return;
            }
        }
        Debug.LogError($"NO SUIT SLOT AVAILABLE - ELEMENT COULD NOT BE ADDED TO INVENTORY: {element.elementName}");
    }

    private void GetFirstItemSlotAvailable(InventoryElement element) {
        foreach (GameObject go in itemSlots) {
            if (!go.GetComponent<InventorySlotManager>().isUsed) {
                go.GetComponent<InventorySlotManager>().SetElement(element);
                return;
            }
        }
        Debug.LogError($"NO ITEM SLOT AVAILABLE - ELEMENT COULD NOT BE ADDED TO INVENTORY: {element.elementName}");
    }

    private GameObject GetFirstElementAvailable() {
        if (spellSlots[0].GetComponent<InventorySlotManager>().isUsed) {
            return spellSlots[0];
        } else if (weaponSlots[0].GetComponent<InventorySlotManager>().isUsed) {
            return weaponSlots[0];
        } else if (suitSlots[0].GetComponent<InventorySlotManager>().isUsed) {
            return suitSlots[0];
        } else if (itemSlots[0].GetComponent<InventorySlotManager>().isUsed) {
            return itemSlots[0];
        } else {
            Debug.LogError("NO ELEMENT FOUND IN INVENTORY");
            return null;
        }
    }

    public void HandleOpening(bool shouldOpen) {
        if (shouldOpen) {
            selector.SetActive(true);
            if (selectedElement == null) {
                SetSelectorDefaultPosition();
            } else {
                UpdateAction(selectedElement.GetComponent<InventorySlotManager>());
            }
        } else {
            selector.SetActive(false);
        }
    }

    public void MoveSelector(Vector2 movement) {
        if (movement.x >= 0.5f) {
            MoveRight();
        } else if (movement.x <= -0.5f) {
            MoveLeft();
        }
        if (movement.y >= 0.5f) {
            MoveUp();
        } else if (movement.y <= -0.5f) {
            MoveDown();
        }
    }

    private void MoveRight() {
        SelectorInventory selectorManager = selector.GetComponent<SelectorInventory>();
        Int16 newIndex = 0;
        switch (selectorManager.selectedElementType) {
            case InventoryElementType.spell:
                newIndex = (Int16)((selectorManager.selectedElementIndex + 1) == spellSlots.Count ? 0 : selectorManager.selectedElementIndex + 1);
                UpdateSelectedElement(spellSlots[newIndex]);
                break;
            case InventoryElementType.weapon:
                newIndex = (Int16)((selectorManager.selectedElementIndex + 1) == weaponSlots.Count ? 0 : selectorManager.selectedElementIndex + 1);
                UpdateSelectedElement(weaponSlots[newIndex]);
                break;
            case InventoryElementType.suit:
                newIndex = (Int16)((selectorManager.selectedElementIndex + 1) == suitSlots.Count ? 0 : selectorManager.selectedElementIndex + 1);
                UpdateSelectedElement(suitSlots[newIndex]);
                break;
            case InventoryElementType.item:
                newIndex = (Int16)(selectorManager.selectedElementIndex == 5 || selectorManager.selectedElementIndex == 11 ? selectorManager.selectedElementIndex - 5 : selectorManager.selectedElementIndex + 1);
                UpdateSelectedElement(itemSlots[newIndex]);
                break;
        }
        selectorManager.selectedElementIndex = newIndex;
        selector.transform.position = selectedElement.transform.position;
    }

    private void MoveLeft() {
        SelectorInventory selectorManager = selector.GetComponent<SelectorInventory>();
        Int16 newIndex = 0;
        switch (selectorManager.selectedElementType) {
            case InventoryElementType.spell:
                newIndex = (Int16)(selectorManager.selectedElementIndex == 0 ? spellSlots.Count - 1 : selectorManager.selectedElementIndex - 1);
                UpdateSelectedElement(spellSlots[newIndex]);
                break;
            case InventoryElementType.weapon:
                newIndex = (Int16)(selectorManager.selectedElementIndex == 0 ? weaponSlots.Count - 1 : selectorManager.selectedElementIndex - 1);
                UpdateSelectedElement(weaponSlots[newIndex]);
                break;
            case InventoryElementType.suit:
                newIndex = (Int16)(selectorManager.selectedElementIndex == 0 ? suitSlots.Count - 1 : selectorManager.selectedElementIndex - 1);
                UpdateSelectedElement(suitSlots[newIndex]);
                break;
            case InventoryElementType.item:
                newIndex = (Int16)(selectorManager.selectedElementIndex == 0 || selectorManager.selectedElementIndex == 6 ? selectorManager.selectedElementIndex + 5 : selectorManager.selectedElementIndex - 1);
                UpdateSelectedElement(itemSlots[newIndex]);
                break;
        }
        selectorManager.selectedElementIndex = newIndex;
        selector.transform.position = selectedElement.transform.position;
    }

    private void MoveUp() {
        SelectorInventory selectorManager = selector.GetComponent<SelectorInventory>();
        switch (selectorManager.selectedElementType) {
            case InventoryElementType.spell:
                selectorManager.selectedElementIndex = (Int16)(selectorManager.selectedElementIndex + 6);
                selectorManager.selectedElementType = InventoryElementType.item;
                UpdateSelectedElement(itemSlots[selectorManager.selectedElementIndex]);
                break;
            case InventoryElementType.weapon:
                selectorManager.selectedElementType = InventoryElementType.spell;
                UpdateSelectedElement(spellSlots[selectorManager.selectedElementIndex]);
                break;
            case InventoryElementType.suit:
                selectorManager.selectedElementType = InventoryElementType.weapon;
                UpdateSelectedElement(weaponSlots[selectorManager.selectedElementIndex]);
                break;
            case InventoryElementType.item:
                if (selectorManager.selectedElementIndex >= 6) {
                    selectorManager.selectedElementIndex = (Int16)(selectorManager.selectedElementIndex - 6);
                    UpdateSelectedElement(itemSlots[selectorManager.selectedElementIndex]);
                } else {
                    selectorManager.selectedElementType = InventoryElementType.suit;
                    UpdateSelectedElement(suitSlots[selectorManager.selectedElementIndex]);
                }
                break;
        }
        selector.transform.position = selectedElement.transform.position;
    }

    private void MoveDown() {
        SelectorInventory selectorManager = selector.GetComponent<SelectorInventory>();
        switch (selectorManager.selectedElementType) {
            case InventoryElementType.spell:
                selectorManager.selectedElementType = InventoryElementType.weapon;
                UpdateSelectedElement(weaponSlots[selectorManager.selectedElementIndex]);
                break;
            case InventoryElementType.weapon:
                selectorManager.selectedElementType = InventoryElementType.suit;
                UpdateSelectedElement(suitSlots[selectorManager.selectedElementIndex]);
                break;
            case InventoryElementType.suit:
                selectorManager.selectedElementType = InventoryElementType.item;
                UpdateSelectedElement(itemSlots[selectorManager.selectedElementIndex]);
                break;
            case InventoryElementType.item:
                if (selectorManager.selectedElementIndex >= 6) {
                    selectorManager.selectedElementIndex = (Int16)(selectorManager.selectedElementIndex - 6);
                    selectorManager.selectedElementType = InventoryElementType.spell;
                    UpdateSelectedElement(spellSlots[selectorManager.selectedElementIndex]);
                } else {
                    selectorManager.selectedElementIndex = (Int16)(selectorManager.selectedElementIndex + 6);
                    UpdateSelectedElement(itemSlots[selectorManager.selectedElementIndex]);
                }
                break;
        }
        selector.transform.position = selectedElement.transform.position;
    }
}
