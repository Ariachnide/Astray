using System.Collections.Generic;
using UnityEngine;

public enum InteractionType {
    sign,
    closedChest,
    openedChest,
    pickUp
}

public enum ActionType {
    none,
    read,
    openChest,
    examineChest,
    pickUp
}

public enum PauseActionType {
    none,
    skipReading,
    proceedReading,
    finishReading,
    equip,
    unequip,
    displayWarning,
    hideWarning,
    sleeping
}

public enum FinishReadingExtraAction {
    none,
    destroyStoredElement
}

public class PlayerAction : MonoBehaviour {
    public ActionType currentActionType;
    public PauseActionType currentPauseActionType;
    [SerializeField]
    private GameObject
        // actionUITxt,
        // actionButton,
        messageBox,
        inventoryWindow,
        prologue;
    private GameObject extElement, storedElement;
    private PlayerMain main;
    private PlayerMenu pMenu;
    private List<PlayerState> actionAuthorizedStates;
    private List<PauseActionType> updatablePauseActionTypes;
    private FinishReadingExtraAction finishReadingExtraAction;

    private void Awake() {
        main = gameObject.GetComponent<PlayerMain>();
        pMenu = gameObject.GetComponent<PlayerMenu>();
        currentActionType = ActionType.none;
        currentPauseActionType = PauseActionType.none;
        finishReadingExtraAction = FinishReadingExtraAction.none;
        actionAuthorizedStates = new List<PlayerState>() {
            PlayerState.idle,
            PlayerState.walk
        };
        updatablePauseActionTypes = new List<PauseActionType>() {
            PauseActionType.none,
            PauseActionType.equip,
            PauseActionType.unequip,
            PauseActionType.displayWarning,
            PauseActionType.sleeping
        };
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag == "PlayerInteraction") {
            main.SetSleepingMode(RigidbodySleepMode2D.NeverSleep);
            UpdateAction(collision.gameObject);
            extElement = collision.gameObject;
        }
    }

    private void OnCollisionStay2D(Collision2D collision) {
        if (collision.gameObject.tag == "PlayerInteraction") {
            UpdateAction(collision.gameObject);
            extElement = collision.gameObject;
        }
    }

    private void OnCollisionExit2D(Collision2D collision) {
        if (collision.gameObject.tag == "PlayerInteraction") {
            main.SetSleepingMode(RigidbodySleepMode2D.StartAwake);
            UpdateActionStatus(
                // "",
                ActionType.none
            );
            extElement = null;
        }
    }

    private void UpdateAction(GameObject interactGO) {
        switch (interactGO.GetComponent<IInteractionType>().GetInteractionType()) {
            case InteractionType.sign:
                DetectSign(interactGO);
                break;
            case InteractionType.closedChest:
                DetectChest(interactGO);
                break;
            case InteractionType.openedChest:
                DetectOpenedChest(interactGO);
                break;
            case InteractionType.pickUp:
                DetectPickUp(interactGO);
                break;
        }
    }

    private void UpdateActionStatus(
        // string statusText = "",
        ActionType type = ActionType.none
    ) {
        currentActionType = type;
        // actionButton.GetComponent<ButtonManager>().HandleActivation((statusText == "") ? false : true);
        // actionUITxt.GetComponent<ActionText>().UpdateText(statusText);
    }

    public void SuspendActionStatus() {
        UpdateActionStatus();
    }

    private void DetectSign(GameObject interactGO) {
        Vector2 difference = transform.position - interactGO.transform.position;
        difference = difference.normalized;
        if (
            (difference.x < -0.85f && main.moveDirection == "right") ||
            (difference.x > 0.85f && main.moveDirection == "left") ||
            (difference.y < -0.75f && main.moveDirection == "up") ||
            (difference.y > 0.75f && main.moveDirection == "down")
        ) {
            UpdateActionStatus(
                // "Lire",
                ActionType.read
            );
        } else {
            UpdateActionStatus();
        }
    }

    private void DetectChest(GameObject interactGO) {
        Vector2 difference = transform.position - interactGO.transform.position;
        if (difference.normalized.y < -0.8f && main.moveDirection == "up") {
            UpdateActionStatus(
                // "Ouvrir",
                ActionType.openChest
            );
        } else {
            UpdateActionStatus();
        }
    }

    private void DetectOpenedChest(GameObject interactGO) {
        Vector2 difference = transform.position - interactGO.transform.position;
        difference = difference.normalized;
        if (
            (difference.x < -0.9f && main.moveDirection == "right") ||
            (difference.x > 0.9f && main.moveDirection == "left") ||
            (difference.y < -0.8f && main.moveDirection == "up") ||
            (difference.y > 0.8f && main.moveDirection == "down")
        ) {
            UpdateActionStatus(
                // "Voir",
                ActionType.examineChest
            );
        } else {
            UpdateActionStatus();
        }
    }

    private void DetectPickUp(GameObject interactGO) {
        Vector2 difference = transform.position - interactGO.transform.position;
        difference = difference.normalized;
        if (
            (difference.x < -0.9f && main.moveDirection == "right") ||
            (difference.x > 0.9f && main.moveDirection == "left") ||
            (difference.y < -0.8f && main.moveDirection == "up") ||
            (difference.y > 0.8f && main.moveDirection == "down")
        ) {
            UpdateActionStatus(
                // "Ramasser",
                ActionType.pickUp
            );
        } else {
            UpdateActionStatus();
        }
    }

    public void SetPauseStatus(
        PauseActionType type //, string statusText = ""
    ) {
        currentPauseActionType = type;
        // actionButton.GetComponent<ButtonManager>().HandleActivation((statusText == "") ? false : true);
        // if (updatablePauseActionTypes.Contains(type))
            // actionUITxt.GetComponent<ActionText>().UpdateText(statusText);
    }

    public void HandleMessageBoxIsPrintingFrozen(bool isPrintingFrozen) {
        messageBox.GetComponent<MessageBox>().isPrintingFrozen = isPrintingFrozen;
    }

    public void HandleAction() {
        if (actionAuthorizedStates.Contains(main.state)) {
            switch (currentActionType) {
                case ActionType.none:
                    break;
                case ActionType.read:
                    UpdateActionStatus();
                    extElement.GetComponent<IReadElement>().ReadElement();
                    main.state = PlayerState.busy;
                    Time.timeScale = 0;
                    break;
                case ActionType.openChest:
                    UpdateActionStatus();
                    extElement.GetComponent<IOpenChest>().Open(gameObject);
                    main.state = PlayerState.busy;
                    Time.timeScale = 0;
                    break;
                case ActionType.examineChest:
                    UpdateActionStatus();
                    extElement.GetComponent<IOpenChest>().Examine();
                    main.state = PlayerState.busy;
                    Time.timeScale = 0;
                    break;
                case ActionType.pickUp:
                    UpdateActionStatus();
                    extElement.GetComponent<IReadElement>().ReadElement();
                    storedElement = extElement;
                    storedElement.GetComponent<ICollectElement>().Collect(gameObject);
                    main.state = PlayerState.busy;
                    finishReadingExtraAction = storedElement.GetComponent<ItemToPickUp>().endAction;
                    Time.timeScale = 0;
                    break;
            }
        } else if (main.state == PlayerState.busy || main.state == PlayerState.inInventory) {
            switch (currentPauseActionType) {
                case PauseActionType.none:
                    break;
                case PauseActionType.skipReading:
                    messageBox.GetComponent<MessageBox>().PrintAll();
                    break;
                case PauseActionType.proceedReading:
                    messageBox.GetComponent<MessageBox>().ProceedReading();
                    break;
                case PauseActionType.finishReading:
                    messageBox.GetComponent<MessageBox>().ProceedReading();
                    main.state = PlayerState.idle;
                    HandleFinishReadingExtraAction();
                    Time.timeScale = 1;
                    break;
                // Inventory cases
                case PauseActionType.equip:
                    inventoryWindow.GetComponent<InventoryManager>().EquipSelectedElement();
                    break;
                case PauseActionType.unequip:
                    inventoryWindow.GetComponent<InventoryManager>().UnequipSelectedElement();
                    break;
                case PauseActionType.displayWarning:
                    inventoryWindow.GetComponent<InventoryManager>().DisplayWarnMessage();
                    break;
                case PauseActionType.hideWarning:
                    messageBox.GetComponent<MessageBox>().ProceedReading();
                    SetPauseStatus(PauseActionType.displayWarning);
                    break;
                // Special behaviors -- replaced by first part of prologue
                case PauseActionType.sleeping:
                    // GetComponent<PlayerHouseGrottoBed>().HandleWakeAttempt();
                    prologue.GetComponent<PrologueHandler>().WakeUp();
                    break;
            }
        } else if (main.state == PlayerState.inMenu) {
            pMenu.HandleAction();
        }
    }

    private void HandleFinishReadingExtraAction() {
        switch (finishReadingExtraAction) {
            case FinishReadingExtraAction.none:
                return;
            case FinishReadingExtraAction.destroyStoredElement:
                Destroy(storedElement);
                storedElement = null;
                break;
        }
        finishReadingExtraAction = FinishReadingExtraAction.none;
    }
}
