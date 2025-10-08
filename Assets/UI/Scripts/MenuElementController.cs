using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuElementController : MonoBehaviour {
    private TMP_Text txt;
    private Image img;
    public Color
        defaultColor = Color.black,
        selectedColor1 = new Color(0.5f, 0.5f, 1, 0.5f),
        selectedColor2 = Color.cyan;
    public bool isSelected, isSelectionFrozen;
    private bool hasImage;
    public PMenuActionType menuActionType;
    public HomeMenuActionType homeMenuActionType;

    private void Awake() {
        hasImage = GetComponent<Image>() != null;
        if (!hasImage) {
            txt = GetComponent<TMP_Text>();
        } else {
            img = GetComponent<Image>();
        }
    }

    public void HandleSelection(bool select) {
        if (isSelected == select) return;
        isSelected = select;
        if (menuActionType == PMenuActionType.save || menuActionType == PMenuActionType.load) {
            GetComponent<SaveSlotShort>().HandleSelection();
        } else if (homeMenuActionType == HomeMenuActionType.displayGameDetail) {
            GetComponent<SaveSlotBig>().HandleSelection();
        } else {
            if (isSelected) {
                if (isSelectionFrozen) {
                    isSelectionFrozen = false;
                } else {
                    StartCoroutine(AnimElement());
                }
            } else {
                if (!hasImage) {
                    txt.color = defaultColor;
                } else {
                    img.color = defaultColor;
                }
                if (isSelectionFrozen) isSelectionFrozen = false;
            }
        }
    }

    private IEnumerator AnimElement() {
        if (!hasImage) {
            while (isSelected) {
                if (isSelectionFrozen) {
                    yield return null;
                    continue;
                }
                txt.color = Color.Lerp(selectedColor1, selectedColor2, Mathf.PingPong(Time.unscaledTime, 1));
                yield return null;
            }
        } else {
            while (isSelected) {
                if (isSelectionFrozen) {
                    yield return null;
                    continue;
                }
                img.color = Color.Lerp(selectedColor1, selectedColor2, Mathf.PingPong(Time.unscaledTime, 1));
                yield return null;
            }
        }
    }
}
