using UnityEngine;

public class ManageActivity : MonoBehaviour {
    private void OnEnable() {
        foreach (Transform t in transform)
            if (t.GetComponent<IHandleActivationPermission>() != null)
                t.gameObject.SetActive(
                    t.GetComponent<IHandleActivationPermission>().CheckPermission()
                );
            else t.gameObject.SetActive(false);
    }

    private void OnDisable() {
        foreach (Transform t in transform)
            t.gameObject.SetActive(false);
    }
}
