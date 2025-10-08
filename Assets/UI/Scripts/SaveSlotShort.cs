using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MenuElementController))]
public class SaveSlotShort : MonoBehaviour {
    private MenuElementController controller;
    public Int16 slotValue;
    public string emptyTxt, selectedTxt;
    public bool hasData;
    [SerializeField]
    private GameObject
        emptyGO,
        dataPreviewGO,
        statPreviewGO,
        playerNameGO;
    private Image img;
    private Color selectedColor1, selectedColor2;

    private void Awake() {
        controller = GetComponent<MenuElementController>();
        img = GetComponent<Image>();
        selectedColor1 = controller.selectedColor1;
        selectedColor2 = controller.selectedColor2;
    }

    public void SetData(PlayerData d) {
        if (d == null) {
            hasData = false;
            emptyGO.SetActive(true);
            return;
        }

        hasData = true;
        dataPreviewGO.SetActive(true);
        emptyGO.SetActive(false);

        statPreviewGO.GetComponent<StatPreviewLoader>().LoadStats(d.maxHP, d.maxMana);

        playerNameGO.GetComponent<TMP_Text>().text = d.playerName;
    }

    public void HandleSelection() {
        if (controller.isSelected) StartCoroutine(AnimElement());
    }

    private IEnumerator AnimElement() {
        if (hasData) {

            img.enabled = true;

            while (controller.isSelected) {
                img.color = Color.Lerp(selectedColor1, selectedColor2, Mathf.PingPong(Time.unscaledTime, 1));
                yield return null;
            }

            img.enabled = false;

        } else {

            TMP_Text emptyTxtComponent = emptyGO.GetComponent<TMP_Text>();

            if (selectedTxt != "") emptyTxtComponent.text = selectedTxt;
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
