using System;
using System.Collections.Generic;
using UnityEngine;

public class ConfirmDeleteController : MonoBehaviour {
    private Int16 i;
    [SerializeField]
    private GameObject askConfirmContent, waitResponseContent, showPositiveResponseContent, continueAfterDeleteOption;
    private List<GameObject> askConfirmOptions;
    private GameObject activeContent;

    private void Awake() {
        activeContent = null;
        i = 0;
        askConfirmOptions = askConfirmContent.GetComponent<MenuSetController>().selectableElements;
    }

    public GameObject ActivateWindow() {
        i = 0;
        gameObject.SetActive(true);
        SetActiveContent(askConfirmContent);
        return askConfirmOptions[i];
    }

    private void SetActiveContent(GameObject go = null) {
        if (activeContent) activeContent.SetActive(false);
        activeContent = go;
        if (activeContent) activeContent.SetActive(true);
    }

    public void GoToWait() {
        i = 0;
        SetActiveContent(waitResponseContent);
    }

    public GameObject GoToPositiveResponse() {
        SetActiveContent(showPositiveResponseContent);
        return continueAfterDeleteOption;
    }

    public GameObject SwitchResponse() {
        i = (Int16)(i == 1 ? 0 : 1);
        return askConfirmOptions[i];
    }

    public void CloseWindow() {
        i = 0;
        SetActiveContent();
        gameObject.SetActive(false);
    }
}
