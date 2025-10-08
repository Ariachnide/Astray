using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MenuElementController))]
public class SaveSlotBig : MonoBehaviour {
    private MenuElementController controller;
    public Int16 slotValue;
    public string emptyTxt, selectedTxt;
    public bool hasData;
    [SerializeField]
    private GameObject
        emptyGO,
        dataPreviewGO,
        heartContainerBoxGO,
        manaContainerBoxGO,
        playerNameGO,
        playerPreviewGO,
        availableItemsGO;
    private Image img;
    private Color selectedColor1, selectedColor2;
    private AvailableElements availableElementsScr;

    private void Awake() {
        availableElementsScr = availableItemsGO.GetComponent<AvailableElements>();
        controller = GetComponent<MenuElementController>();
        img = GetComponent<Image>();
        selectedColor1 = controller.selectedColor1;
        selectedColor2 = controller.selectedColor2;
    }

    public void SetData(PlayerData d) {
        if (d == null) {
            hasData = false;
            dataPreviewGO.SetActive(false);
            emptyGO.SetActive(true);
            return;
        }

        hasData = true;
        dataPreviewGO.SetActive(true);
        emptyGO.SetActive(false);

        HeartsUI heartsUI = heartContainerBoxGO.GetComponent<HeartsUI>();
        if (heartsUI.isLoaded) heartsUI.Unload();
        heartsUI.SetupHealthBar(d.maxHP, d.hitPoints);

        if (d.maxMana > 0) {
            manaContainerBoxGO.SetActive(true);
            ManaUI manaUI = manaContainerBoxGO.GetComponent<ManaUI>();
            if (manaUI.isLoaded) manaUI.Unload();
            manaUI.SetupManaBar(d.maxMana, d.currentMana);
        }

        playerNameGO.GetComponent<TMP_Text>().text = d.playerName;

        playerPreviewGO.GetComponent<PlayerPreviewSaveSlot>().SetSuitValue(
            d.equippedElements.Find(e => availableElementsScr.GetElementByName(e).type == InventoryElementType.suit)
        );
    }

    public void HandleSelection() {
        if (controller.isSelected) StartCoroutine(AnimElement());
    }

    private IEnumerator AnimElement() {
        if (hasData) {

            playerPreviewGO.GetComponent<PlayerPreviewSaveSlot>().HandleSelection(true);
            img.enabled = true;

            while (controller.isSelected) {
                img.color = Color.Lerp(selectedColor1, selectedColor2, Mathf.PingPong(Time.unscaledTime, 1));
                yield return null;
            }

            playerPreviewGO.GetComponent<PlayerPreviewSaveSlot>().HandleSelection(false);
            img.enabled = false;

        } else {

            TMP_Text emptyTxtComponent = emptyGO.GetComponent<TMP_Text>();

            emptyTxtComponent.text = selectedTxt;
            emptyTxtComponent.color = Color.black;
            img.enabled = true;

            while (controller.isSelected) {
                img.color = Color.Lerp(selectedColor1, selectedColor2, Mathf.PingPong(Time.unscaledTime, 1));
                yield return null;
            }

            emptyTxtComponent.text = emptyTxt;
            emptyTxtComponent.color = Color.gray;
            img.enabled = false;
        }
    }
}
