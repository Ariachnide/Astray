using System;
using UnityEngine;

public class ManaBarContainerController : MonoBehaviour {
    [SerializeField]
    private GameObject manaBar;

    public void AdjustManaBarSize(Int16 v) {
        RectTransform manaContainerRT = GetComponent<RectTransform>();
        RectTransform manaBarRT = manaBar.GetComponent<RectTransform>();
        switch (v) {
            case 0:
                if (gameObject.activeSelf)
                    gameObject.SetActive(false);
                break;
            case 20:
                if (!gameObject.activeSelf)
                    gameObject.SetActive(true);
                manaContainerRT.anchoredPosition = new Vector2(
                    80, manaContainerRT.anchoredPosition.y
                );
                manaContainerRT.sizeDelta = new Vector2(150, 30);
                manaBarRT.sizeDelta = new Vector2(142, 22);
                break;
            case 40:
                if (!gameObject.activeSelf)
                    gameObject.SetActive(true);
                manaContainerRT.anchoredPosition = new Vector2(
                    155, manaContainerRT.anchoredPosition.y
                );
                manaContainerRT.sizeDelta = new Vector2(300, 30);
                manaBarRT.sizeDelta = new Vector2(292, 22);
                break;
            default:
                Debug.LogError($"UNKNOWN MANA MAX VALUE: {v}");
                if (gameObject.activeSelf)
                    gameObject.SetActive(false);
                break;
        }
    }
}
