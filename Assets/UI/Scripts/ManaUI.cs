using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManaItem {
    public Int16 Value;
    public GameObject ManaGameObject;
}

public class ManaUI : MonoBehaviour {
    [SerializeField]
    private Sprite
        fullCrystalSprite,
        threeQuartersCrystalSprite,
        halfCrystalSprite,
        oneQuarterCrystalSprite,
        emptyCrystalSprite;
    private List<ManaItem> manaItemList;
    private Int16 activeSlotIndex;
    private Vector3 maxScale = new Vector3(1.2f, 1.2f, 1f);
    private Transform activeSlot;
    public bool isLoaded;
    private bool showActiveSlot;

    public void SetupManaBar(Int16 maxMana, Int16 currentMana) {
        Int16 modifier = maxMana;

        manaItemList = new List<ManaItem>();
        foreach (Transform child in transform) {
            if (child.tag == "ManaUI")
                manaItemList.Add(new ManaItem { Value = 0, ManaGameObject = child.gameObject });
        }
        activeSlot = manaItemList[activeSlotIndex].ManaGameObject.transform;

        foreach (ManaItem item in manaItemList) {
            if (modifier > 0) {
                item.ManaGameObject.SetActive(true);
                item.ManaGameObject.GetComponent<Image>().sprite = emptyCrystalSprite;
                modifier -= 4;
            } else {
                item.ManaGameObject.SetActive(false);
            }
        }
        isLoaded = true;
        UpdateManaBar(currentMana);
    }

    public void HandleShowActiveSlot(bool shouldShow) {
        showActiveSlot = shouldShow;
        if (shouldShow) {
            activeSlot.localScale = maxScale;
        } else {
            activeSlot.localScale = Vector3.one;
        }
    }

    public void Unload() {
        activeSlotIndex = 0;
        activeSlot = manaItemList[activeSlotIndex].ManaGameObject.transform;
        foreach (ManaItem item in manaItemList) {
            item.ManaGameObject.GetComponent<Image>().sprite = emptyCrystalSprite;
            item.ManaGameObject.SetActive(false);
            item.Value = 0;
        }
        isLoaded = false;
    }

    public void ReatoreManaToFull() {
        Int16 tempIndex = -1;
        foreach (ManaItem mi in manaItemList) {
            if (!mi.ManaGameObject.activeSelf) break;
            tempIndex++;
            if (mi.Value != 4) {
                mi.ManaGameObject.GetComponent<Image>().sprite = fullCrystalSprite;
                mi.Value = 4;
            }
        }
        if (tempIndex != activeSlotIndex) {
            activeSlot.localScale = Vector3.one;
            activeSlotIndex = tempIndex;
            activeSlot = manaItemList[activeSlotIndex].ManaGameObject.transform;
        }
    }

    public void EmptyManaBar() {
        foreach (ManaItem mi in manaItemList) {
            if (!mi.ManaGameObject.activeSelf) break;
            if (mi.Value != 0) {
                mi.ManaGameObject.GetComponent<Image>().sprite = emptyCrystalSprite;
                mi.Value = 0;
            }
        }
        activeSlot.localScale = Vector3.one;
        activeSlotIndex = 0;
        activeSlot = manaItemList[activeSlotIndex].ManaGameObject.transform;
    }

    public void UpdateManaBar(Int16 mana) {
        Int16 modifier = mana;
        Int16 previousIndex = activeSlotIndex;
        List<Int16> itemsToUpdate = new List<Int16>();
        if (modifier > 0) {
            while (modifier > 0) {
                ManaItem item = manaItemList[activeSlotIndex];
                if (item.Value == 4) {
                    activeSlotIndex++;
                    item = manaItemList[activeSlotIndex];
                }
                item.Value++;
                modifier--;
                if (!itemsToUpdate.Contains(activeSlotIndex))
                    itemsToUpdate.Add(activeSlotIndex);
            }
        } else {
            while (modifier < 0) {
                ManaItem item = manaItemList[activeSlotIndex];
                if (!itemsToUpdate.Contains(activeSlotIndex))
                    itemsToUpdate.Add(activeSlotIndex);
                if (item.Value == 1)
                    activeSlotIndex--;
                item.Value--;
                modifier++;
            }
        }
        UpdateManaBarSprites(itemsToUpdate);
        if (previousIndex != activeSlotIndex) {
            activeSlot.localScale = Vector3.one;
            activeSlot = manaItemList[activeSlotIndex].ManaGameObject.transform;
            if (showActiveSlot) activeSlot.localScale = maxScale;
        }
    }

    private void UpdateManaBarSprites(List<Int16> itemsToUpdate) {
        foreach (Int16 i in itemsToUpdate) {
            ManaItem item = manaItemList[i];
            switch (item.Value) {
                case 0:
                    item.ManaGameObject.GetComponent<Image>().sprite = emptyCrystalSprite;
                    break;
                case 1:
                    item.ManaGameObject.GetComponent<Image>().sprite = oneQuarterCrystalSprite;
                    break;
                case 2:
                    item.ManaGameObject.GetComponent<Image>().sprite = halfCrystalSprite;
                    break;
                case 3:
                    item.ManaGameObject.GetComponent<Image>().sprite = threeQuartersCrystalSprite;
                    break;
                case 4:
                    item.ManaGameObject.GetComponent<Image>().sprite = fullCrystalSprite;
                    break;
                default:
                    Debug.LogError($"INVALID MANA VALUE: {item.Value}");
                    break;
            }
        }
    }
}
