using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerItemState {
    none,
    bottle
}

public class PlayerItemData {
    public Int16 AnimatorValue { get; set; }
    public string Name { get; set; }
    public List<PlayerState> AuthStates { get; set; }
}

public class PlayerItem : MonoBehaviour {
    public PlayerItemState state;
    private PlayerMain main;
    private PlayerInventory inventory;
    public bool isItemEnabled, canUseBombs;
    public PlayerItemData usedItem;
    private List<PlayerItemData> itemDataList;

    [SerializeField]
    private List<GameObject> itemGOList;
    [SerializeField]
    private GameObject itemUiButton, itemUiIcon, itemUiCounter;

    private void Awake() {
        state = PlayerItemState.none;
        main = GetComponent<PlayerMain>();
        inventory = GetComponent<PlayerInventory>();
        isItemEnabled = canUseBombs = true;
        itemDataList = new List<PlayerItemData> {
            new() {
                AnimatorValue = 0,
                Name = "",
                AuthStates = new List<PlayerState>()
            },
            new() {
                AnimatorValue = 0,
                Name = "bomb",
                AuthStates = new List<PlayerState> {
                    PlayerState.idle,
                    PlayerState.walk
                }
            },
            new() {
                AnimatorValue = 0,
                Name = "bottle",
                AuthStates = new List<PlayerState> {
                    PlayerState.idle,
                    PlayerState.walk
                }
            }
        };
        usedItem = itemDataList[0];
    }

    public void HandleChange(string itemName, Sprite itemSprite = null) {
        string prevItem = usedItem.Name;
        usedItem = itemDataList.Find(d => d.Name == itemName);
        itemUiButton.GetComponent<ButtonManager>().HandleActivation(
            (itemName != "")
                ? CheckActivation(usedItem.Name)
                : false
        );
        itemUiIcon.GetComponent<ButtonIconManager>().HandleVisibility(CheckActivation(usedItem.Name));
        itemUiIcon.GetComponent<ButtonIconManager>().HandleSprite(itemSprite);
        usedItem = itemDataList.Find(d => d.Name == itemName);
        if (usedItem.Name == "bomb") {
            itemUiCounter.GetComponent<ItemValueController>().SetMaxValue(inventory.maxBomb);
            itemUiCounter.GetComponent<ItemValueController>().UpdateText(inventory.bombs);
        } else if (prevItem == "bomb") {
            itemUiCounter.GetComponent<ItemValueController>().Hide();
        }
    }

    public void SetMaxBomb(Int16 v) {
        if (v < inventory.maxBomb) return;
        if (inventory.maxBomb <= 0) inventory.SetItemInInventory("bomb");
        inventory.maxBomb = v;
        if (usedItem.Name == "bomb") {
            itemUiCounter.GetComponent<ItemValueController>().SetMaxValue(v);
        }
    }

    public void UpdateBombs(Int16 v) {
        inventory.bombs += v;
        if (v > 0) {
            if (inventory.bombs > inventory.maxBomb)
                inventory.bombs = inventory.maxBomb;
            if (usedItem.Name == "bomb") {
                CheckButtonUIVisibility();
                itemUiCounter.GetComponent<ItemValueController>().UpdateText(inventory.bombs);
            }
        } else if (v < 0) {
            if (inventory.bombs < 0)
                inventory.bombs = 0;
            if (usedItem.Name == "bomb") {
                CheckButtonUIVisibility();
                itemUiCounter.GetComponent<ItemValueController>().UpdateText(inventory.bombs);
            }
        }
    }

    private bool CheckActivation(string name) {
        if (name == "bomb") {
            return inventory.bombs > 0;
        } else {
            return true;
        }
    }

    public bool IsUsable() {
        if (usedItem.Name == "bomb")
            return inventory.bombs > 0;
        return false;
    }

    public void CheckButtonUIVisibility() {
        itemUiButton.GetComponent<ButtonManager>().HandleActivation(IsUsable());
        itemUiIcon.GetComponent<ButtonIconManager>().HandleVisibility(IsUsable());
    }

    public void Use() {
        if (usedItem.AuthStates.Contains(main.state)) {
            switch (usedItem.Name) {
                case "":
                    break;
                case "bomb":
                    if (!canUseBombs || inventory.bombs < 1) break;
                    GameObject bombGO = Instantiate(
                        itemGOList.Find(d => d.GetComponent<PItemController>().pItemName == usedItem.Name),
                        SetStartingPoint(),
                        Quaternion.identity
                    );
                    bombGO.GetComponent<BombController>().StartCountDown();
                    UpdateBombs(-1);
                    StartCoroutine(HandleBombCD());
                    break;
                case "bottle":
                    break;
            }
        }
    }

    private IEnumerator HandleBombCD() {
        canUseBombs = false;
        yield return new WaitForSeconds(1f);
        canUseBombs = true;
    }

    private Vector2 SetStartingPoint() {
        return main.moveDirection switch {
            "right" => new Vector2(transform.position.x + 0.45f, transform.position.y),
            "left" => new Vector2(transform.position.x - 0.45f, transform.position.y),
            "up" => new Vector2(transform.position.x, transform.position.y + 0.45f),
            "down" => new Vector2(transform.position.x, transform.position.y - 0.45f),
            _ => new Vector2(transform.position.x, transform.position.y - 0.45f),
        };
    }

    public void InterruptItemAnim(PlayerState stateArg = PlayerState.idle) {
        FinishItemAnim(stateArg);
    }

    private void FinishItemAnim(PlayerState stateArg = PlayerState.idle) {
        main.state = stateArg;
        state = PlayerItemState.none;
        main.canChangeMoveDirection = true;
        // animator.SetBool("", false);
    }
}
