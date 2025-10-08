using System;
using UnityEngine;

public class ItemToPickUp : MonoBehaviour, ICollectElement, IRegisteredElement {
    public Int16 indexInRegistry = -1;
    public string elementName;
    public FinishReadingExtraAction endAction;
    [SerializeField]
    private GameObject globalMapCtrl, mapCtrl;

    public void Collect(GameObject go) {
        go.GetComponent<PlayerInventory>().SetItemInInventory(elementName);
        HandleStateUpdate(RegElmState.ended);
    }

    public void HandleStateUpdate(RegElmState s) {
        CheckIndexInRegistry();
        switch (s) {
            case RegElmState.ended:
                globalMapCtrl
                    .GetComponent<GlobalRegisteredElementsController>()
                    .GetOrCreateElmInRegistry(
                        indexInRegistry,
                        mapCtrl.name,
                        RegElmState.ended
                    );
                gameObject.SetActive(false);
                break;
        }
    }

    private void CheckIndexInRegistry() {
        if (indexInRegistry != -1) return;
        indexInRegistry = (Int16)mapCtrl
            .GetComponent<MapController>()
            .registeredElements
            .FindIndex(e => gameObject.GetInstanceID() == e.GetInstanceID());
    }
}
