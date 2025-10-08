using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartItem {
    public Int16 Value;
    public GameObject HeartGameObject;
}

public class HeartsUI : MonoBehaviour {
    [SerializeField]
    private Sprite
        fullHeartSprite,
        threeQuartersHeartSprite,
        halfHeartSprite,
        oneQuarterHeartSprite,
        emptyHeartSprite;
    private List<HeartItem> heartItemList;
    private Int16
        activeSlotIndex,
        maxFrames = 250,
        frames;
    private Transform activeSlot;
    private Vector3
        minScale = new Vector3(0.8f, 0.8f, 1f),
        maxScale = new Vector3(1.2f, 1.2f, 1f);
    private bool
        canPulse,
        isGrowing;
    public bool isLoaded;

    private void Awake() {
        heartItemList = new List<HeartItem>();
        foreach (Transform child in transform)
            if (child.tag == "HeartsUI")
                heartItemList.Add(new HeartItem { Value = 0, HeartGameObject = child.gameObject });
        activeSlot = heartItemList[activeSlotIndex].HeartGameObject.transform;
    }

    private void Update() {
        if (canPulse) {
            if (isGrowing) {
                if (frames < maxFrames) {
                    activeSlot.localScale = Vector3.Lerp(minScale, maxScale, (float)frames / maxFrames);
                    frames++;
                } else {
                    frames = 0;
                    isGrowing = false;
                }
            } else {
                if (frames < maxFrames) {
                    activeSlot.localScale = Vector3.Lerp(maxScale, minScale, (float)frames / maxFrames);
                    frames++;
                } else {
                    frames = 0;
                    isGrowing = true;
                }
            }
        }
    }

    public void HandlePulsing(bool shouldPulse) {
        canPulse = shouldPulse;
        if (!shouldPulse) {
            activeSlot.localScale = Vector3.one;
        }
    }

    public void SetupHealthBar(Int16 maxHp, Int16 currentHp) {
        Int16 modifier = maxHp;
        foreach (HeartItem item in heartItemList) {
            if (modifier > 0) {
                item.HeartGameObject.SetActive(true);
                item.HeartGameObject.GetComponent<Image>().sprite = emptyHeartSprite;
                modifier -= 4;
            } else {
                item.HeartGameObject.SetActive(false);
            }
        }
        isLoaded = true;
        UpdateHealthBar(currentHp);
    }

    public void Unload() {
        activeSlotIndex = 0;
        activeSlot = heartItemList[activeSlotIndex].HeartGameObject.transform;
        foreach (HeartItem item in heartItemList) {
            item.HeartGameObject.GetComponent<Image>().sprite = emptyHeartSprite;
            item.HeartGameObject.SetActive(false);
            item.Value = 0;
        }
        isLoaded = false;
    }

    public void HealToFull() {
        Int16 tempIndex = -1;
        foreach (HeartItem hi in heartItemList) {
            if (!hi.HeartGameObject.activeSelf) break;
            tempIndex++;
            if (hi.Value != 4) {
                hi.HeartGameObject.GetComponent<Image>().sprite = fullHeartSprite;
                hi.Value = 4;
            }
        }
        if (tempIndex != activeSlotIndex) {
            activeSlot.localScale = Vector3.one;
            activeSlotIndex = tempIndex;
            activeSlot = heartItemList[activeSlotIndex].HeartGameObject.transform;
        }
    }

    public void EmptyHealthBar() {
        foreach (HeartItem hi in heartItemList) {
            if (!hi.HeartGameObject.activeSelf) break;
            if (hi.Value != 0) {
                hi.HeartGameObject.GetComponent<Image>().sprite = emptyHeartSprite;
                hi.Value = 0;
            }
        }
        activeSlot.localScale = Vector3.one;
        activeSlotIndex = 0;
        activeSlot = heartItemList[activeSlotIndex].HeartGameObject.transform;
        if (canPulse) canPulse = false;
    }

    public void UpdateHealthBar(Int16 hp) {
        Int16 modifier = hp;
        Int16 previousIndex = activeSlotIndex;
        List<Int16> itemsToUpdate = new List<Int16>();
        if (modifier > 0) {
            while (modifier > 0) {
                HeartItem item = heartItemList[activeSlotIndex];
                if (item.Value == 4) {
                    activeSlotIndex++;
                    item = heartItemList[activeSlotIndex];
                }
                item.Value++;
                modifier--;
                if (!itemsToUpdate.Contains(activeSlotIndex))
                    itemsToUpdate.Add(activeSlotIndex);
            }
        } else {
            while (modifier < 0) {
                HeartItem item = heartItemList[activeSlotIndex];
                if (!itemsToUpdate.Contains(activeSlotIndex))
                    itemsToUpdate.Add(activeSlotIndex);
                if (item.Value == 1)
                    activeSlotIndex--;
                item.Value--;
                modifier++;
            }
        }
        UpdateHealthBarSprites(itemsToUpdate);
        if (previousIndex != activeSlotIndex) {
            activeSlot.localScale = Vector3.one;
            activeSlot = heartItemList[activeSlotIndex].HeartGameObject.transform;
        }
    }

    private void UpdateHealthBarSprites(List<Int16> itemsToUpdate) {
        foreach (Int16 i in itemsToUpdate) {
            HeartItem item = heartItemList[i];
            switch (item.Value) {
                case 0:
                    item.HeartGameObject.GetComponent<Image>().sprite = emptyHeartSprite;
                    break;
                case 1:
                    item.HeartGameObject.GetComponent<Image>().sprite = oneQuarterHeartSprite;
                    break;
                case 2:
                    item.HeartGameObject.GetComponent<Image>().sprite = halfHeartSprite;
                    break;
                case 3:
                    item.HeartGameObject.GetComponent<Image>().sprite = threeQuartersHeartSprite;
                    break;
                case 4:
                    item.HeartGameObject.GetComponent<Image>().sprite = fullHeartSprite;
                    break;
                default:
                    Debug.LogError($"INVALID HEART VALUE: {item.Value}");
                    break;
            }
        }
    }
}
