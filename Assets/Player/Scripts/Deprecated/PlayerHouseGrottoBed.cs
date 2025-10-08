using System.Collections;
using UnityEngine;

public enum SleepingStatus {
    none,
    deepSleep,
    lightSleep,
    awaken,
    gettingUp
}

public class PlayerHouseGrottoBed : MonoBehaviour {
    private PlayerMain main;
    private PlayerAction action;
    private Animator animator;
    public SleepingStatus status;
    [SerializeField]
    private SpawnPoint nearBedSP;
    private CamPlayerMovement camPlayerMovement;
    [SerializeField]
    private GameObject woodBed;

    private void Awake() {
        main = GetComponent<PlayerMain>();
        action = GetComponent<PlayerAction>();
        animator = GetComponent<Animator>();
        status = SleepingStatus.none;
        camPlayerMovement = Camera.main.GetComponent<CamPlayerMovement>();
    }

    public void SetUpBehavior(SpawnPoint sp) {
        camPlayerMovement.UpdateArea(sp.area);
        main.state = PlayerState.busy;
        woodBed.GetComponent<BoxCollider2D>().enabled = false;
        transform.position = sp.position;
        camPlayerMovement.LockCameraOnTarget(CamTargetType.player);
        status = SleepingStatus.deepSleep;
        animator.SetInteger("specialBehavior", 1);
        action.SetPauseStatus(PauseActionType.sleeping);
        StartCoroutine(DelayWaking());
    }

    private IEnumerator DelayWaking() {
        yield return new WaitForSeconds(3);
        animator.SetInteger("specialBehavior", 2);
        status = SleepingStatus.awaken;
        action.SetPauseStatus(
            PauseActionType.sleeping//, "Commencer"
        );
        float delay = 0;
        bool shouldPrepareDelay = true;
        while (status != SleepingStatus.gettingUp) {
            if (status == SleepingStatus.awaken) {
                if (shouldPrepareDelay) {
                    delay = Time.unscaledTime + 3f;
                    shouldPrepareDelay = false;
                } else if (Time.unscaledTime >= delay) {
                    animator.SetInteger("specialBehavior", 1);
                    action.SetPauseStatus(PauseActionType.sleeping);
                    status = SleepingStatus.lightSleep;
                    shouldPrepareDelay = true;
                }
            }
            yield return null;
        }
    }

    public void HandleWakeAttempt() {
        if (status == SleepingStatus.awaken) {
            status = SleepingStatus.gettingUp;
            action.SetPauseStatus(PauseActionType.sleeping);
            StartCoroutine(HandleGetUp());
        } else if (status == SleepingStatus.lightSleep) {
            action.SetPauseStatus(
                PauseActionType.sleeping//, "Commencer"
            );
            animator.SetInteger("specialBehavior", 2);
            status = SleepingStatus.awaken;
        }
    }

    private IEnumerator HandleGetUp() {
        main.backgroundLayer.GetComponent<DisplayUIElement>().HideScene(Color.black, 0.5f);
        yield return new WaitForSeconds(0.5f);
        animator.SetInteger("specialBehavior", 0);
        main.currentSpawnPoint = nearBedSP;
        transform.position = nearBedSP.position;
        woodBed.GetComponent<BoxCollider2D>().enabled = true;
        main.moveDirection = "down";
        animator.SetFloat("moveX", 0);
        animator.SetFloat("moveY", -1);
        yield return new WaitForSeconds(0.25f);
        main.backgroundLayer.GetComponent<DisplayUIElement>().DisplayScene(0.5f);
        yield return new WaitForSeconds(0.5f);
        status = SleepingStatus.none;
        main.canInteractWithMapElements = true;
        main.state = PlayerState.idle;
        main.canChangeMoveDirection = main.canMove = true;
        camPlayerMovement.UpdateArea(nearBedSP.area);
        action.SetPauseStatus(PauseActionType.none);
    }
}
