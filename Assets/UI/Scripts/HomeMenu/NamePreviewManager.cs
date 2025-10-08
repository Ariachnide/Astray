using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NamePreviewManager : MonoBehaviour {
    [SerializeField]
    private List<GameObject> letters;
    [SerializeField]
    private GameObject selector;
    public string value;
    public Int16 index;

    private void Awake() {
        value = "";
        index = 0;
    }

    public void SetDefault() {
        selector.GetComponent<RectTransform>().anchoredPosition = letters[0].GetComponent<RectTransform>().anchoredPosition;
        value = "";
        index = 0;
        foreach (GameObject l in letters) {
            l.GetComponent<TMP_Text>().text = "";
        }
    }

    public void WriteLetter(char letter) {
        letters[index].GetComponent<TMP_Text>().text = Char.ToString(letter);
        UpdateValue();
        if (index == letters.Count - 1) return;
        index++;
        selector.GetComponent<RectTransform>().anchoredPosition = letters[index].GetComponent<RectTransform>().anchoredPosition;
    }

    public void RemoveLetter() {
        if (index != 0 && (index < letters.Count - 1 || letters[index].GetComponent<TMP_Text>().text == "")) {
            index--;
            selector.GetComponent<RectTransform>().anchoredPosition = letters[index].GetComponent<RectTransform>().anchoredPosition;
        }
        letters[index].GetComponent<TMP_Text>().text = "";
        UpdateValue();
    }

    private void UpdateValue() {
        value = "";
        foreach (GameObject l in letters) {
            value += l.GetComponent<TMP_Text>().text;
        }
    }

    public Int16 GetPreviewLength() {
        return (Int16)letters.Count;
    }
}
