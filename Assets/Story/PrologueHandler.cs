using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum PrologueEventType {
    AwakeningInGrotto,
    FirstGuardMeeting,
    SecondGuardMeeting
}

public struct PrologueStoryMilestone {
    public PrologueEventType EventType;
    public bool HasBeenCompleted;
    public List<string> Details;
}

public class MoveOrder {
    public bool IsDone = false;
    public Vector2 Destination;
    public CharDirection Direction;
}

public class PrologueHandler : MonoBehaviour, IGoToStoryItem {
    private bool
        hasTriggeredGrottoEvent,
        hasTriggeredFirstGuardMeeting,
        hasTriggeredSecondGuardMeeting;

    public bool accelerate;

    [SerializeField]
    private GameObject
        player,
        globalMap,
        upperBackgroundLayer,
        messageBox,
        woodBed,
        shekraGO,
        guardGO;
    [SerializeField]
    private SpawnPoint
        bedSP,
        nearBedSP,
        houseGrottoEntrance;
    [SerializeField]
    private Area
        SW0,
        SW1,
        SW2;
    [SerializeField]
    private List<string>
        msgAwake1,
        msgAwake2,
        msgFirstGuardMeeting1,
        msgFirstGuardMeeting2,
        msgFirstGuardMeeting3, // guard2 yells at Shekra
        msgFirstGuardMeeting4, // Shekra answers and taunts
        msgFirstGuardMeeting5, // guard1 orders attack
        msgFirstGuardMeeting6; // Shekra talks to player
    private WaitForFixedUpdate waitFFU = new WaitForFixedUpdate();

    public bool CheckHasEvent() {
        switch (globalMap.GetComponent<GlobalMapHandler>().GetActiveMapName()) {
            case "StartGrotto":
                if (hasTriggeredGrottoEvent) {
                    return false;
                } else {
                    return true;
                }
            case "SisterWoods":
                if (hasTriggeredFirstGuardMeeting/* && hasTriggeredSecondGuardMeeting*/) {
                    return false;
                } else {
                    return true;
                }
            default:
                return false;
        }
    }

    public void TriggerEvent() {
        switch (globalMap.GetComponent<GlobalMapHandler>().GetActiveMapName()) {
            case "StartGrotto":
                if (!hasTriggeredGrottoEvent) {
                    StartCoroutine(GrottoEvent());
                } else {
                    Debug.LogError("EXPECTED AN EVENT TO OCCUR IN STARTGROTTO BUT NOTHING TO BE HANDLED");
                }
                break;
            case "SisterWoods":
                if (!hasTriggeredFirstGuardMeeting) {
                    FirstGuardMeeting();
                } else if (!hasTriggeredSecondGuardMeeting) {
                    SecondGuardMeeting();
                } else {
                    Debug.LogError("EXPECTED AN EVENT TO OCCUR IN SISTERWOODS BUT NOTHING TO BE HANDLED");
                }
                break;
            default:
                Debug.LogError($"EXPECTED AN EVENT TO OCCUR BUT NO EVENT HANDLED IN MAP {globalMap.GetComponent<GlobalMapHandler>().GetActiveMapName()}");
                break;
        }
    }

    public void EventSetup(List<StoryMilestoneRaw> rawMilestones) {
        foreach (StoryMilestoneRaw rawMilestone in rawMilestones) {
            PrologueEventType type = (PrologueEventType)Enum.Parse(typeof(PrologueEventType), rawMilestone.EventName);
            switch (type) {
                case PrologueEventType.AwakeningInGrotto:
                    hasTriggeredGrottoEvent = rawMilestone.HasBeenCompleted;
                    break;
                case PrologueEventType.FirstGuardMeeting:
                    hasTriggeredFirstGuardMeeting = rawMilestone.HasBeenCompleted;
                    break;
                case PrologueEventType.SecondGuardMeeting:
                    hasTriggeredSecondGuardMeeting = rawMilestone.HasBeenCompleted;
                    break;
            }
        }
    }

    public StoryMilestoneRawList GetMilestones() {
        List<StoryMilestoneRaw> milestones = new List<StoryMilestoneRaw>();

        if (hasTriggeredGrottoEvent)
            milestones.Add(
                new StoryMilestoneRaw() {
                    EventName = PrologueEventType.AwakeningInGrotto.ToString(),
                    HasBeenCompleted = true,
                    Details = new List<string>()
                }
            );

        if (hasTriggeredFirstGuardMeeting)
            milestones.Add(
                new StoryMilestoneRaw() {
                    EventName = PrologueEventType.FirstGuardMeeting.ToString(),
                    HasBeenCompleted = true,
                    Details = new List<string>()
                }
            );

        if (hasTriggeredSecondGuardMeeting)
            milestones.Add(
                new StoryMilestoneRaw() {
                    EventName = PrologueEventType.SecondGuardMeeting.ToString(),
                    HasBeenCompleted = true,
                    Details = new List<string>()
                }
            );

        return new StoryMilestoneRawList() {
            StoryName = StoryState.Prologue.ToString(),
            Milestones = milestones
        };
    }

    private IEnumerator GrottoEvent() {
        Animator playerAnimator = player.GetComponent<Animator>();
        CamPlayerMovement cpm = Camera.main.GetComponent<CamPlayerMovement>();
        MessageBox mb = messageBox.GetComponent<MessageBox>();
        
        upperBackgroundLayer.GetComponent<DisplayUIElement>().SetColor(Color.black);

        cpm.UpdateArea(bedSP.area);
        player.GetComponent<PlayerMain>().state = PlayerState.busy;
        woodBed.GetComponent<BoxCollider2D>().enabled = false;
        player.transform.position = bedSP.position;
        cpm.LockCameraOnTarget(CamTargetType.player);
        playerAnimator.SetInteger("specialBehavior", 1);

        yield return new WaitForSecondsRealtime(accelerate ? 0.2f : 2f);

        List<MessageItem> messageItems = new List<MessageItem>();
        foreach (string m in msgAwake1) {
            messageItems.Add(
                new MessageItem {
                    TxtContent = m,
                    BoxType = MsgBoxType.blue,
                    Alignment = TextAlignmentOptions.Center,
                    PrintingSpeed = MsgPrintingSpeed.fast,
                    CanSkip = accelerate ? true : false,
                    IsPartOfCutscene = true
                }
            );
        }
        mb.ActivateMessageBox(messageItems);

        while (mb.isUsed) yield return null;

        yield return new WaitForSecondsRealtime(accelerate ? 0.2f : 1.5f);

        upperBackgroundLayer.GetComponent<DisplayUIElement>().DisplayScene(accelerate ? 0.2f : 3f);
        yield return new WaitForSecondsRealtime(accelerate ? 0.2f : 3f);

        messageItems.Clear();
        foreach (string m in msgAwake2) {
            messageItems.Add(
                new MessageItem {
                    TxtContent = m,
                    BoxType = MsgBoxType.blue,
                    Alignment = TextAlignmentOptions.Center,
                    PrintingSpeed = MsgPrintingSpeed.fast,
                    CanSkip = accelerate ? true : false,
                    IsPartOfCutscene = true
                }
            );
        }
        mb.ActivateMessageBox(messageItems);
        while (mb.isUsed) yield return null;

        playerAnimator.SetInteger("specialBehavior", 2);

        player.GetComponent<PlayerAction>().SetPauseStatus(PauseActionType.sleeping);
    }

    public void WakeUp() {
        StartCoroutine(HandleWakeUp());
    }

    private IEnumerator HandleWakeUp() {
        PlayerMain playerMain = player.GetComponent<PlayerMain>();
        Animator playerAnimator = player.GetComponent<Animator>();

        player.GetComponent<PlayerAction>().SetPauseStatus(PauseActionType.none);

        playerMain.backgroundLayer.GetComponent<DisplayUIElement>().HideScene(Color.black, 0.5f);
        yield return new WaitForSeconds(0.5f);
        playerAnimator.SetInteger("specialBehavior", 0);

        player.transform.position = nearBedSP.position;
        woodBed.GetComponent<BoxCollider2D>().enabled = true;

        playerMain.moveDirection = "down";
        playerAnimator.SetFloat("moveX", 0);
        playerAnimator.SetFloat("moveY", -1);

        yield return new WaitForSeconds(0.25f);
        playerMain.backgroundLayer.GetComponent<DisplayUIElement>().DisplayScene(0.5f);
        yield return new WaitForSeconds(0.5f);

        playerMain.canInteractWithMapElements = true;
        playerMain.state = PlayerState.idle;
        playerMain.canChangeMoveDirection = playerMain.canMove = true;

        Camera.main.GetComponent<CamPlayerMovement>().UpdateArea(nearBedSP.area);

        hasTriggeredGrottoEvent = true;
    }

    private void FirstGuardMeeting() {
        StartCoroutine(HandleFirstGuardMeeting());
    }

    private IEnumerator HandleFirstGuardMeeting() {
        PlayerMain playerMain = player.GetComponent<PlayerMain>();
        Animator playerAnimator = player.GetComponent<Animator>();
        CamPlayerMovement cpm = Camera.main.GetComponent<CamPlayerMovement>();
        MessageBox mb = messageBox.GetComponent<MessageBox>();
        List<MessageItem> messageItems = new List<MessageItem>();

        playerMain.state = PlayerState.busy;
        player.GetComponent<SpriteRenderer>().enabled = false;
        player.transform.position = new Vector2(houseGrottoEntrance.position.x, houseGrottoEntrance.position.y + 1);
        playerMain.moveDirection = "down";
        playerAnimator.SetFloat("moveX", 0);
        playerAnimator.SetFloat("moveY", -1);

        cpm.UpdateArea(SW0);
        cpm.LockCameraOnTarget(CamTargetType.customPosition, new Vector2(-4.5f, -2.5f));

        GameObject shekraInst = Instantiate(shekraGO, new Vector3(9.5f, -6.5f, 0), Quaternion.identity);
        GameObject guard1Inst = Instantiate(guardGO, new Vector3(-2.5f, -11.5f, 0), Quaternion.identity);
        GameObject guard2Inst = Instantiate(guardGO, new Vector3(0.5f, -14.5f, 0), Quaternion.identity);
        GameObject guard3Inst = Instantiate(guardGO, new Vector3(-9.5f, -13.5f, 0), Quaternion.identity);

        ShekraBehaviour shekraBhvr = shekraInst.GetComponent<ShekraBehaviour>();
        GuardBehaviour guardBhvr1 = guard1Inst.GetComponent<GuardBehaviour>();
        GuardBehaviour guardBhvr2 = guard2Inst.GetComponent<GuardBehaviour>();
        GuardBehaviour guardBhvr3 = guard3Inst.GetComponent<GuardBehaviour>();
        
        yield return new WaitForSeconds(1f);

        List<MoveOrder> g1moves = new List<MoveOrder>() {
            new MoveOrder { Destination = new Vector2(-2.5f, -5.5f), Direction = CharDirection.Up },
            new MoveOrder { Destination = new Vector2(-4.5f, -5.5f), Direction = CharDirection.Left },
            new MoveOrder { Destination = new Vector2(-4.5f, -3.5f), Direction = CharDirection.Up }
        };
        List<MoveOrder> g2moves = new List<MoveOrder>() {
            new MoveOrder { Destination = new Vector2(0.5f, -7.5f), Direction = CharDirection.Up },
            new MoveOrder { Destination = new Vector2(-1.5f, -7.5f), Direction = CharDirection.Left },
            new MoveOrder { Destination = new Vector2(-1.5f, -2.5f), Direction = CharDirection.Up }
        };
        List<MoveOrder> g3moves = new List<MoveOrder>() {
            new MoveOrder { Destination = new Vector2(-9.5f, -7.5f), Direction = CharDirection.Up },
            new MoveOrder { Destination = new Vector2(-7.5f, -7.5f), Direction = CharDirection.Right },
            new MoveOrder { Destination = new Vector2(-7.5f, -2.5f), Direction = CharDirection.Up }
        };

        while (true) {
            if (g1moves.Count == 0 && g2moves.Count == 0 && g3moves.Count == 0) break;
            if (guardBhvr1.handleMoveCharacterCo == null && g1moves.Count > 0) {
                if (guardBhvr1.CheckOrder()) g1moves.RemoveAt(0);
                if (g1moves.Count > 0) guardBhvr1.MoveCharacter(g1moves[0].Destination, g1moves[0].Direction);
            }
            if (guardBhvr2.handleMoveCharacterCo == null && g2moves.Count > 0) {
                if (guardBhvr2.CheckOrder()) g2moves.RemoveAt(0);
                if (g2moves.Count > 0) guardBhvr2.MoveCharacter(g2moves[0].Destination, g2moves[0].Direction);
            }
            if (guardBhvr3.handleMoveCharacterCo == null && g3moves.Count > 0) {
                if (guardBhvr3.CheckOrder()) g3moves.RemoveAt(0);
                if (g3moves.Count > 0) guardBhvr3.MoveCharacter(g3moves[0].Destination, g3moves[0].Direction);
            }
            yield return null;
        }

        guardBhvr1.ResetOrder();
        guardBhvr2.ResetOrder();
        guardBhvr3.ResetOrder();

        guardBhvr2.TurnCharacter(CharDirection.Left);
        guardBhvr3.TurnCharacter(CharDirection.Right);

        yield return new WaitForSeconds(0.5f);

        foreach (string m in msgFirstGuardMeeting1) {
            messageItems.Add(
                new MessageItem {
                    TxtContent = m,
                    BoxType = MsgBoxType.black,
                    Alignment = TextAlignmentOptions.Left,
                    PrintingSpeed = MsgPrintingSpeed.fast,
                    CanSkip = accelerate ? true : false,
                    IsPartOfCutscene = true
                }
            );
        }
        mb.ActivateMessageBox(messageItems);
        while (mb.isUsed) yield return null;
        messageItems.Clear();

        guardBhvr1.TurnCharacter(CharDirection.Up);
        guardBhvr2.TurnCharacter(CharDirection.Up);
        guardBhvr3.TurnCharacter(CharDirection.Up);
        player.GetComponent<SpriteRenderer>().enabled = true;

        playerAnimator.SetBool("moving", true);
        while (player.transform.position.y > 0.5f) {
            player.GetComponent<Rigidbody2D>().MovePosition(player.transform.position + Vector3.down * (playerMain.speed / 1.5f) * Time.deltaTime);
            yield return waitFFU;
        }
        playerAnimator.speed = 0.5f;
        while (player.transform.position.y < 1) {
            player.GetComponent<Rigidbody2D>().MovePosition(player.transform.position + Vector3.up * (playerMain.speed / 3) * Time.deltaTime);
            yield return waitFFU;
        }
        playerAnimator.speed = 1;
        playerAnimator.SetBool("moving", false);

        foreach (string m in msgFirstGuardMeeting2) {
            messageItems.Add(
                new MessageItem {
                    TxtContent = m,
                    BoxType = MsgBoxType.black,
                    Alignment = TextAlignmentOptions.Left,
                    PrintingSpeed = MsgPrintingSpeed.fast,
                    CanSkip = accelerate ? true : false,
                    IsPartOfCutscene = true
                }
            );
        }
        mb.ActivateMessageBox(messageItems);
        while (mb.isUsed) yield return null;
        messageItems.Clear();

        guardBhvr2.GetComponent<Animator>().SetBool("isArmed", true);
        guardBhvr3.GetComponent<Animator>().SetBool("isArmed", true);

        List<MoveOrder> shekraMoves = new List<MoveOrder>() {
            new MoveOrder { Destination = new Vector2(1.5f, -6.5f), Direction = CharDirection.Left },
            new MoveOrder { Destination = new Vector2(1.5f, -0.5f), Direction = CharDirection.Up },
            new MoveOrder { Destination = new Vector2(-2f, -0.5f), Direction = CharDirection.Left }
        };
        while (shekraMoves.Count > 0) {
            if (shekraBhvr.handleMoveCharacterCo == null) {
                if (shekraBhvr.CheckOrder()) shekraMoves.RemoveAt(0);
                if (shekraMoves.Count > 0) shekraBhvr.MoveCharacter(shekraMoves[0].Destination, shekraMoves[0].Direction);
            }
            yield return null;
        }

        shekraBhvr.ResetOrder();
        shekraBhvr.TurnCharacter(CharDirection.Down);
        guardBhvr3.TurnCharacter(CharDirection.Right);

        yield return new WaitForSeconds(1f);

        foreach (string m in msgFirstGuardMeeting3) {
            messageItems.Add(
                new MessageItem {
                    TxtContent = m,
                    BoxType = MsgBoxType.black,
                    Alignment = TextAlignmentOptions.Left,
                    PrintingSpeed = MsgPrintingSpeed.fast,
                    CanSkip = accelerate ? true : false,
                    IsPartOfCutscene = true
                }
            );
        }
        mb.ActivateMessageBox(messageItems);
        while (mb.isUsed) yield return null;
        messageItems.Clear();

        yield return new WaitForSeconds(0.25f);

        foreach (string m in msgFirstGuardMeeting4) {
            messageItems.Add(
                new MessageItem {
                    TxtContent = m,
                    BoxType = MsgBoxType.black,
                    Alignment = TextAlignmentOptions.Left,
                    PrintingSpeed = MsgPrintingSpeed.fast,
                    CanSkip = accelerate ? true : false,
                    IsPartOfCutscene = true
                }
            );
        }
        mb.ActivateMessageBox(messageItems);
        while (mb.isUsed) yield return null;
        messageItems.Clear();

        yield return new WaitForSeconds(0.5f);
        shekraBhvr.SetArm(true);
        yield return new WaitForSeconds(0.25f);
        guardBhvr2.MoveCharacter(new Vector2(-1.5f, -3.5f), CharDirection.Up, false);
        while (!guardBhvr2.CheckOrder()) yield return null;
        guardBhvr1.GetComponent<Animator>().SetBool("isArmed", true);
        yield return new WaitForSeconds(0.25f);

        foreach (string m in msgFirstGuardMeeting5) {
            messageItems.Add(
                new MessageItem {
                    TxtContent = m,
                    BoxType = MsgBoxType.black,
                    Alignment = TextAlignmentOptions.Left,
                    PrintingSpeed = MsgPrintingSpeed.fast,
                    CanSkip = accelerate ? true : false,
                    IsPartOfCutscene = true
                }
            );
        }
        mb.ActivateMessageBox(messageItems);
        while (mb.isUsed) yield return null;
        messageItems.Clear();

        shekraBhvr.ManageThreatAreaActivation(true);
        shekraBhvr.Attack(ShekraAttackMove.Attack1, guard2Inst.transform.position, 0.4f, 0.6f, 0.4f, 15f, true);

        while (!shekraBhvr.CheckOrder()) yield return null;

        guardBhvr1.TurnCharacter(CharDirection.Right);
        shekraBhvr.TurnCharacter(CharDirection.Left);
        yield return new WaitForSeconds(0.65f);
        shekraBhvr.Attack(ShekraAttackMove.ClockwiseAttack, new Vector2(-9.5f, -1.5f), 3f, 0.4f, 0.7f, 25f, true);

        yield return new WaitForSeconds(0.25f);
        guardBhvr1.MoveCharacter(
            new Vector2(guard1Inst.transform.position.x + 3, guard1Inst.transform.position.y),
            CharDirection.Right
        );
        yield return new WaitForSeconds(0.35f);

        guard3Inst.GetComponent<Animator>().speed = 0.7f;
        guardBhvr3.speed = guardBhvr3.speed * 70 / 100;
        guardBhvr3.MoveCharacter(
            new Vector2(guard3Inst.transform.position.x - 1, guard3Inst.transform.position.y),
            CharDirection.Left,
            false
        );

        while (true) {
            if (shekraBhvr.hitTargetList.Contains(guard3Inst.GetInstanceID())) {
                yield return new WaitForSeconds(0.15f);
                shekraBhvr.EndHandleAttackCo(true);
                break;
            } else {
                yield return null;
            }
        }

        while (!shekraBhvr.CheckOrder()) yield return null;

        shekraBhvr.TurnCharacter(CharDirection.Right);
        playerAnimator.SetFloat("moveX", -1f);
        yield return new WaitForSeconds(0.5f);

        foreach (string m in msgFirstGuardMeeting6) {
            messageItems.Add(
                new MessageItem {
                    TxtContent = m,
                    BoxType = MsgBoxType.black,
                    Alignment = TextAlignmentOptions.Left,
                    PrintingSpeed = MsgPrintingSpeed.fast,
                    CanSkip = accelerate ? true : false,
                    IsPartOfCutscene = true
                }
            );
        }
        mb.ActivateMessageBox(messageItems);
        while (mb.isUsed) yield return null;
        messageItems.Clear();

        playerAnimator.SetFloat("moveX", 0);
        shekraBhvr.SetArm(false);
        shekraBhvr.speed = shekraBhvr.speed * 1.5f;
        shekraBhvr.MoveCharacter(new Vector2(shekraInst.transform.position.x + 10, shekraInst.transform.position.y), CharDirection.Right);
        while (!shekraBhvr.CheckOrder()) yield return null;
        shekraBhvr.MoveCharacter(new Vector2(shekraInst.transform.position.x, shekraInst.transform.position.y - 4.5f), CharDirection.Down);
        while (!shekraBhvr.CheckOrder()) yield return null;
        shekraBhvr.MoveCharacter(new Vector2(shekraInst.transform.position.x + 10, shekraInst.transform.position.y), CharDirection.Right);
        while (!shekraBhvr.CheckOrder()) yield return null;
        Destroy(shekraInst);

        cpm.target = player.transform;

        playerMain.canInteractWithMapElements = true;
        playerMain.state = PlayerState.idle;

        hasTriggeredFirstGuardMeeting = true;
    }

    private void SecondGuardMeeting() {
        //
        hasTriggeredSecondGuardMeeting = true;
    }
}
