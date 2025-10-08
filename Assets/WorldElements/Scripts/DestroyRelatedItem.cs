using System.Collections.Generic;
using UnityEngine;

public class DestroyRelatedItem : MonoBehaviour {
    [SerializeField]
    List<GameObject> relatedObject;

    private void OnDestroy() {
        foreach (GameObject go in relatedObject) Destroy(go);
    }
}
