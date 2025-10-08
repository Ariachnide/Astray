using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(MenuSetController))]
public class DetailSetController : MonoBehaviour {
    [Serializable]
    private struct InventoryCategory {
        public InventoryElementType IEType;
        public GameObject CategoryGO;
    }

    public Int16 deleteElementIndex = 2;
    [SerializeField]
    private GameObject
        availableItems,
        heartContainerBox,
        manaContainerBox,
        orbBox,
        localizationBox,
        timePlayedBox;
    [SerializeField]
    private List<InventoryCategory> inventoryCategories = new();

    public GameObject GetDeleteElement() {
        return GetComponent<MenuSetController>().selectableElements[deleteElementIndex];
    }

    public void LoadElements(PlayerData d) {
        heartContainerBox.GetComponent<HeartsUI>().SetupHealthBar(d.maxHP, d.hitPoints);

        if (d.maxMana > 0) {
            manaContainerBox.SetActive(true);
            ManaUI manaUI = manaContainerBox.GetComponent<ManaUI>();
            if (manaUI.isLoaded) manaUI.Unload();
            manaUI.SetupManaBar(d.maxMana, d.currentMana);
            manaUI.HandleShowActiveSlot(false);
        }

        InventoryElement elm;

        foreach (string strElm in d.savedElements) {
            elm = availableItems.GetComponent<AvailableElements>().GetElementByName(strElm);
            inventoryCategories
                .Find(catStruct => catStruct.IEType == elm.type)
                .CategoryGO
                .GetComponent<ICHM_Controller>().DispatchElement(elm.icon);
        }

        orbBox.GetComponent<TMP_Text>().text = d.orbs.ToString();

        localizationBox.GetComponent<TMP_Text>().text = d.spawnPointLocationName;

        List<string> splitTime = new(d.playTime.Split(":"));
        timePlayedBox.GetComponent<TMP_Text>().text = $"{Int16.Parse(splitTime[0]).ToString()} h {Int16.Parse(splitTime[1]).ToString()} m";
    }

    public void UnloadElements() {
        heartContainerBox.GetComponent<HeartsUI>().Unload();
        if (manaContainerBox.GetComponent<ManaUI>().isLoaded)
            manaContainerBox.GetComponent<ManaUI>().Unload();
        manaContainerBox.SetActive(false);
        foreach (InventoryCategory catStruct in inventoryCategories)
            catStruct.CategoryGO.GetComponent<ICHM_Controller>().ClearElements();
        orbBox.GetComponent<TMP_Text>().text = "";
        localizationBox.GetComponent<TMP_Text>().text = "";
        timePlayedBox.GetComponent<TMP_Text>().text = "";
    }
}
