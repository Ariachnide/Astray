using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using TMPro;

public enum ChestState {
    closed,
    open
}

public class Chest : MonoBehaviour, IInteractionType, IOpenChest, IRegisteredElement {
    public Int16 indexInRegistry = -1;
    public InteractionType interactionType;
    [SerializeField]
    private Sprite openedChest;
    public GameObject loot;
    [SerializeField]
    private GameObject messageBox, globalMapCtrl, mapCtrl;
    public ChestState state;

    private void Awake() {
        interactionType = InteractionType.closedChest;
        state = ChestState.closed;
    }

    public InteractionType GetInteractionType() {
        return interactionType;
    }

    public void Open(GameObject player) {
        StartCoroutine(ShowLoot(player));
        HandleStateUpdate(RegElmState.empty);
    }

    public void HandleStateUpdate(RegElmState s) {
        CheckIndexInRegistry();
        switch (s) {
            case RegElmState.empty:
                globalMapCtrl
                    .GetComponent<GlobalRegisteredElementsController>()
                    .GetOrCreateElmInRegistry(
                        indexInRegistry,
                        mapCtrl.name,
                        RegElmState.empty
                    );
                GetComponent<SpriteRenderer>().sprite = openedChest;
                state = ChestState.open;
                interactionType = InteractionType.openedChest;
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

    private IEnumerator ShowLoot(GameObject player) {
        yield return new WaitForSecondsRealtime(0.15f);
        GameObject lootGO = Instantiate(
            loot,
            new Vector2(transform.position.x, transform.position.y + 0.35f),
            Quaternion.identity
        );
        float halfDelay = 0.75f;
        lootGO.GetComponent<ItemToCollect>().ChestAnim(halfDelay);
        yield return new WaitForSecondsRealtime(halfDelay * 2);

        lootGO.GetComponent<ItemToCollect>().HandleCollect(player, shouldDestroy: false);

        IGetItemData itemData = lootGO.GetComponent<IGetItemData>();
        string txtStart = "Vous avez trouvé ", itemName = loot.GetComponent<IGetItemData>().GetName();

        messageBox.GetComponent<MessageBox>().ActivateMessageBox(
            new List<MessageItem> {
                new MessageItem {
                    TxtContent = $"{txtStart}<b>{itemName}</b>! {itemData.GetComment()}",
                    BoxType = MsgBoxType.blue,
                    Alignment = TextAlignmentOptions.Center,
                    PrintingSpeed = MsgPrintingSpeed.fast,
                    MsgEscapePrintingSpeedElements = new List<MsgBlock> {
                        new MsgBlock {
                            SelectedSpeed = MsgPrintingSpeed.instant,
                            OpeningIndex = (Int16)Regex.Replace(txtStart, "<.*?>", String.Empty).Length,
                            ClosingIndex = (Int16)(Regex.Replace(txtStart, "<.*?>", String.Empty).Length + Regex.Replace(itemName, "<.*?>", String.Empty).Length)
                        }
                    }
                }
            }
        );

        while (Time.timeScale < 1) {
            yield return null;
        }
        Destroy(lootGO);
    }

    public void Examine() {
        string txtStart = "Ce coffre contenait ", itemName = loot.GetComponent<IGetItemData>().GetName();
        messageBox.GetComponent<MessageBox>().ActivateMessageBox(
            new List<MessageItem> {
                new MessageItem {
                    TxtContent = $"{txtStart}{itemName}.",
                    BoxType = MsgBoxType.blue,
                    Alignment = TextAlignmentOptions.Center,
                    PrintingSpeed = MsgPrintingSpeed.instant
                }
            }
        );
    }
}
