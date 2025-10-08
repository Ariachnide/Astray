using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Reader : MonoBehaviour, IInteractionType, IReadElement {
    public InteractionType interactionType;
    [SerializeField]
    private GameObject messageBox;
    public List<string> txtBlocks;
    private List<MessageItem> msgItems;

    private void Awake() {
        msgItems = new List<MessageItem>();
        foreach (string tb in txtBlocks) {
            msgItems.Add(new MessageItem {
                TxtContent = tb,
                Alignment = TextAlignmentOptions.TopLeft,
                PrintingSpeed = MsgPrintingSpeed.instant
            });
        }
    }

    public InteractionType GetInteractionType() {
        return interactionType;
    }

    public void ReadElement() {
        messageBox.GetComponent<MessageBox>().ActivateMessageBox(msgItems);
    }
}
