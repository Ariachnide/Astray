using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Collections;

public class CheckHasStaffArea : MonoBehaviour {
    private StaffPUAreaActivityPermission staffPermission;
    [SerializeField]
    private GameObject staffGO, messageBox;
    [SerializeField]
    private List<string> warnMsgNoItem, warnMsgItemNotEquipped;
    private bool isPlayerInArea;

    private void Awake() {
        staffPermission = GetComponent<StaffPUAreaActivityPermission>();
    }

    private void OnEnable() {
        if (staffGO == null) Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        if (collider.CompareTag("Player") && !isPlayerInArea) {
            if (staffPermission.HasPlayerEquippedWeapon()) {
                Destroy(gameObject);
            } else {
                StartCoroutine(HandlePlayerExpulsion(collider.gameObject));
            }
        }
    }

    private void OnTriggerExit2D() {
        isPlayerInArea = false;
    }

    private IEnumerator HandlePlayerExpulsion(GameObject player) {
        isPlayerInArea = true;

        PlayerMain pm = player.GetComponent<PlayerMain>();
        Animator playerAnimator = player.GetComponent<Animator>();
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        MessageBox mb = messageBox.GetComponent<MessageBox>();
        WaitForFixedUpdate wait = new WaitForFixedUpdate();

        Time.timeScale = 0;
        List<MessageItem> messageItems = new List<MessageItem>();
        List<string> chosenMessages = staffPermission.DoesPlayerHasStaff()
            ? warnMsgItemNotEquipped
            : warnMsgNoItem;

        pm.state = PlayerState.busy;

        foreach (string m in chosenMessages) {
            messageItems.Add(
                new MessageItem {
                    TxtContent = m,
                    BoxType = MsgBoxType.blue,
                    Alignment = TextAlignmentOptions.Center,
                    PrintingSpeed = MsgPrintingSpeed.fast,
                    CanSkip = true,
                    IsPartOfCutscene = true
                }
            );
        }
        mb.ActivateMessageBox(messageItems);

        while (mb.isUsed) yield return null;

        Time.timeScale = 1;
        pm.moveDirection = "up";
        playerAnimator.SetFloat("moveX", 0);
        playerAnimator.SetFloat("moveY", 1);
        playerAnimator.SetBool("moving", true);

        float destinationY = player.transform.position.y + 0.5f;

        while (player.transform.position.y < destinationY) {
            rb.MovePosition(player.transform.position + Vector3.up * pm.speed * Time.deltaTime);
            yield return wait;
        }

        playerAnimator.SetBool("moving", false);
        pm.state = PlayerState.idle;
    }
}
