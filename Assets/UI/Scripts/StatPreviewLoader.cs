using System;
using TMPro;
using UnityEngine;

public class StatPreviewLoader : MonoBehaviour {
    [SerializeField]
    private GameObject
        healthTxtGO,
        manaPreviewGO,
        manaTxtGO;

    public void LoadStats(Int16 maxHP, Int16 maxMana) {
        healthTxtGO.GetComponent<TMP_Text>().text = maxHP.ToString();
        if (maxMana > 0) {
            manaPreviewGO.SetActive(true);
            manaTxtGO.GetComponent<TMP_Text>().text = maxMana.ToString();
        }
    }

    /* public void UnloadStats() {
        healthTxtGO.GetComponent<TMP_Text>().text = "";
        manaPreviewGO.SetActive(false);
        manaTxtGO.GetComponent<TMP_Text>().text = "";
    } */
}
