using TMPro;
using UnityEngine;

public class LetterItem : MonoBehaviour {
    private string letter;
    public bool isCaseSensitive = true;

    private void Awake() {
        letter = GetComponent<TMP_Text>().text;
    }

    public char GetLetter() {
        if (!isCaseSensitive) return letter[0];
        if (GetComponent<TMP_Text>().fontStyle != FontStyles.UpperCase) return letter[0];
        else return (char)(letter[0] - 32);
    }
}
