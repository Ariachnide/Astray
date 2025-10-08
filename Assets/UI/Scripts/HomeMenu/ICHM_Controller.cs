using UnityEngine;

public class ICHM_Controller : MonoBehaviour {
    public void DispatchElement(Sprite s) {
        foreach (Transform child in transform)
            if (!child.GetComponent<IBHM_Controller>().isUsed) {
                child.GetComponent<IBHM_Controller>().SetIcon(s);
                return;
            }
    }

    public void ClearElements() {
        foreach (Transform child in transform)
            if (child.GetComponent<IBHM_Controller>().isUsed)
                child.GetComponent<IBHM_Controller>().RemoveIcon();
    }
}
