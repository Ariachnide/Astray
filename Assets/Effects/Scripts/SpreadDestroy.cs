using UnityEngine;

public class SpreadDestroy : MonoBehaviour {
    private void OnDestroy() {
        if (transform.parent.gameObject != null) Destroy(transform.parent.gameObject);
    }
}
