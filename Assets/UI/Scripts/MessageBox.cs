using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MessageItem {
    public string TxtContent;
    public MsgBoxType BoxType = MsgBoxType.black;
    public TextAlignmentOptions Alignment = TextAlignmentOptions.TopLeft;
    public MsgPrintingSpeed PrintingSpeed = MsgPrintingSpeed.fast;
    public List<MsgBlock> MsgEscapePrintingSpeedElements = new List<MsgBlock>();
    public bool CanSkip = true;
    public bool IsPartOfCutscene = false;
}

public enum MsgBoxType {
    black,
    blue
}

public enum MsgPrintingSpeed {
    instant,
    fast
}

public class MsgBlock {
    public MsgPrintingSpeed SelectedSpeed;
    public Int16 OpeningIndex;
    public Int16 ClosingIndex;
}

public class MessageBox : MonoBehaviour {
    [SerializeField]
    private GameObject txtGO, messageSign, player;
    [SerializeField]
    private Sprite boxImg, blurryBoxImg;
    private PlayerMain playerMain;
    private PlayerAction playerAction;
    private Image bgImg;
    private TMP_Text txtField;
    private List<MessageItem> messageItems;
    private WaitForSecondsRealtime wait;
    private bool shouldProceedReading, isPrinting, isHandlingMsgBlock, isSkipping;
    public bool isPrintingFrozen, isBoxDisplayed, isUsed;
    private Color blackBoxColor, blueBoxColor;
    private MsgBoxType currentBoxType;
    private Coroutine fastPrintingCo;

    private void Awake() {
        txtField = txtGO.GetComponent<TMP_Text>();
        bgImg = GetComponent<Image>();
        wait = new WaitForSecondsRealtime(0.02f);
        blueBoxColor = new Color(0f, 0f, 1f, 0.6f);
        blackBoxColor = new Color(0f, 0f, 0f, 0.75f);
        messageItems = new List<MessageItem>();
        playerMain = player.GetComponent<PlayerMain>();
        playerAction = player.GetComponent<PlayerAction>();
    }

    private IEnumerator DisplayMessageBox(MsgBoxType boxType) {
        float timePassed = 0f;
        float transTime = 0.3f;
        Color startColor = new Color(0f, 0f, 0f, 0f);
        Color chosenColor;

        switch (boxType) {
            case MsgBoxType.blue:
                bgImg.sprite = blurryBoxImg;
                chosenColor = blueBoxColor;
                currentBoxType = MsgBoxType.blue;
                break;
            case MsgBoxType.black:
            default:
                bgImg.sprite = boxImg;
                chosenColor = blackBoxColor;
                currentBoxType = MsgBoxType.black;
                break;
        }

        while (timePassed < transTime) {
            timePassed += Time.fixedUnscaledDeltaTime;
            bgImg.color = Color.Lerp(
                startColor,
                chosenColor,
                timePassed / transTime
            );
            yield return wait;
        }
        isBoxDisplayed = true;
    }

    private IEnumerator PrintMessages() {
        isUsed = true;
        for (Int16 i = 0; i < messageItems.Count; i++) {
            if (!isBoxDisplayed) StartCoroutine(DisplayMessageBox(messageItems[i].BoxType));
            if (currentBoxType != messageItems[i].BoxType) {
                ClearMessageBox();
                if (!isBoxDisplayed) isBoxDisplayed = false;
                yield return new WaitForSecondsRealtime(0.5f);
                StartCoroutine(DisplayMessageBox(messageItems[i].BoxType));
            }
            while (!isBoxDisplayed) yield return null;

            if (txtField.alignment != messageItems[i].Alignment)
                txtField.alignment = messageItems[i].Alignment;
            
            Int16 charCount = (Int16)txtField.GetTextInfo(messageItems[i].TxtContent).characterCount;
            List<MsgBlock> blocks = SetMsgBlocks(messageItems[i], charCount);
            txtField.maxVisibleCharacters = 0;
            txtField.text = messageItems[i].TxtContent;

            isPrinting = true;
            if (messageItems[i].CanSkip) {
                if (!(playerMain.state == PlayerState.inInventory))
                    playerAction.SetPauseStatus(PauseActionType.skipReading);
            }
            foreach (MsgBlock b in blocks) {
                if (isSkipping) break;
                isHandlingMsgBlock = true;
                switch (b.SelectedSpeed) {
                    case MsgPrintingSpeed.instant:
                        InstantPrinting(b.ClosingIndex, charCount);
                        break;
                    case MsgPrintingSpeed.fast:
                        fastPrintingCo = StartCoroutine(FastPrinting(b.ClosingIndex, charCount));
                        break;
                }
                while (isHandlingMsgBlock) yield return null;
            }
            while (isPrinting) yield return null;
            if (isSkipping) isSkipping = false;

            txtField.maxVisibleCharacters = 99999;

            if ((i + 1) == messageItems.Count) {
                if (playerMain.state == PlayerState.inInventory) {
                    messageSign.GetComponent<MessageSign>().DisplaySign(MsgBoxSign.square);
                    playerAction.SetPauseStatus(PauseActionType.hideWarning);
                } else if (messageItems[i].IsPartOfCutscene) {
                    messageSign.GetComponent<MessageSign>().DisplaySign(MsgBoxSign.square);
                    playerAction.SetPauseStatus(PauseActionType.proceedReading);
                } else {
                    messageSign.GetComponent<MessageSign>().DisplaySign(MsgBoxSign.square);
                    playerAction.SetPauseStatus(PauseActionType.finishReading);
                }
            } else {
                messageSign.GetComponent<MessageSign>().DisplaySign(MsgBoxSign.arrow);
                playerAction.SetPauseStatus(PauseActionType.proceedReading);
            }
            while (!shouldProceedReading) yield return null;

            ClearMessageBox();
            shouldProceedReading = false;
        }
        messageItems.Clear();
        DisableMessageBox();
        isUsed = false;
    }

    private List<MsgBlock> SetMsgBlocks(MessageItem msgItem, Int16 charCount) {
        if (msgItem.MsgEscapePrintingSpeedElements.Count == 0)
            return new List<MsgBlock> {
                new MsgBlock {
                    SelectedSpeed = msgItem.PrintingSpeed,
                    OpeningIndex = 0,
                    ClosingIndex = charCount
                }
            };
        Int16 i = 0;
        List<MsgBlock> response = new List<MsgBlock>();
        foreach (MsgBlock block in msgItem.MsgEscapePrintingSpeedElements) {
            if (i < block.OpeningIndex) {
                response.Add(new MsgBlock {
                    SelectedSpeed = msgItem.PrintingSpeed,
                    OpeningIndex = i,
                    ClosingIndex = block.OpeningIndex
                });
            }
            response.Add(block);
            i = block.ClosingIndex;
        }
        if (i < charCount)
            response.Add(new MsgBlock {
                SelectedSpeed = msgItem.PrintingSpeed,
                OpeningIndex = i,
                ClosingIndex = charCount
            });
        return response;
    }

    private void InstantPrinting(Int16 i, Int16 charCount) {
        txtField.maxVisibleCharacters = i;
        isHandlingMsgBlock = false;
        if (i >= charCount)
            isPrinting = false;
    }

    private IEnumerator FastPrinting(Int16 i, Int16 charCount) {
        WaitForSecondsRealtime wait = new WaitForSecondsRealtime(0.05f);
        while (txtField.maxVisibleCharacters < i) {
            if (isPrintingFrozen) {
                yield return null;
                continue;
            }
            txtField.maxVisibleCharacters++;
            yield return wait;
        }
        isHandlingMsgBlock = false;
        if (i >= charCount)
            isPrinting = false;
    }

    public void PrintAll() {
        StopCoroutine(fastPrintingCo);
        fastPrintingCo = null;
        isHandlingMsgBlock = false;
        isPrinting = false;
        isSkipping = true;
    }

    private void ClearMessageBox() {
        txtField.text = "";
        txtField.maxVisibleCharacters = 0;
        messageSign.GetComponent<MessageSign>().HideSign();
    }

    public void ActivateMessageBox(List<MessageItem> items) {
        foreach (MessageItem i in items) messageItems.Add(i);
        StartCoroutine(PrintMessages());
    }

    private void DisableMessageBox() {
        bgImg.color = new Color(0f, 0f, 0f, 0f);
        isBoxDisplayed = false;
    }

    public void ProceedReading() {
        if (!isBoxDisplayed || !isUsed) return;
        shouldProceedReading = true;
        playerAction.SetPauseStatus(PauseActionType.none);
    }
}
