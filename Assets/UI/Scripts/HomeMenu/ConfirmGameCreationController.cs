using UnityEngine;

public class ConfirmGameCreationController : MonoBehaviour {
    [SerializeField]
    private GameObject waitingContent, positiveResponseContent, continueAfterCreation;
    private GameObject activeContent;

    private void Awake() {
        activeContent = null;
    }

    public void ActivateWindow() {
        gameObject.SetActive(true);
        SetActiveContent(waitingContent);
    }

    private void SetActiveContent(GameObject go = null) {
        if (activeContent) activeContent.SetActive(false);
        activeContent = go;
        if (activeContent) activeContent.SetActive(true);
    }

    public GameObject GoToPositiveResponse() {
        SetActiveContent(positiveResponseContent);
        return continueAfterCreation;
    }

    public void CloseWindow() {
        SetActiveContent();
        gameObject.SetActive(false);
    }
}
