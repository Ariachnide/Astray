using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState {
    idle,
    walk,
    usingWeapon,
    usingSpell,
    usingItem,
    knockbacked,
    busy,
    inInventory,
    inMenu
}

public enum CharDirection {
    Left,
    Right,
    Down,
    Up
}

public class PlayerMain : MonoBehaviour, IColliderRef {
    public PlayerState state;
    private PlayTime playTime;
    private PlayerInventory inventory;
    private PlayerSpell spell;
    private PlayerAlteration alteration;
    public ColliderRef SelfColliderRef { get; private set; } = ColliderRef.Player;
    public float speed;
    public string moveDirection, playerName;
    public bool canChangeMoveDirection, canMove, canInteractWithMapElements;
    public Int16 hitPoints, maxHP;
    public SpawnPoint currentSpawnPoint;
    private Rigidbody2D rb;
    private Collider2D playerCollider;
    private Vector3 movement;
    private Animator animator;
    private SpriteRenderer srd;
    [SerializeField]
    private GameObject
        heartContainerBox,
        globalMap,
        playerControl,
        globalUIGO,
        storySwitch;
    public GameObject backgroundLayer;
    private CamPlayerMovement camPlayerMovement;
    private List<PlayerState> blockWalkControlStates;
    private WaitForFixedUpdate waitForFixedUpdate;

    private void Awake() {
        state = PlayerState.idle;

        playTime = GetComponent<PlayTime>();
        inventory = GetComponent<PlayerInventory>();
        spell = GetComponent<PlayerSpell>();
        alteration = GetComponent<PlayerAlteration>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();
        srd = GetComponent<SpriteRenderer>();

        globalUIGO.SetActive(true);

        camPlayerMovement = Camera.main.GetComponent<CamPlayerMovement>();

        canInteractWithMapElements = true;

        blockWalkControlStates = new List<PlayerState>() {
            PlayerState.usingWeapon,
            PlayerState.usingSpell,
            PlayerState.usingItem,
            PlayerState.knockbacked,
            PlayerState.busy,
        };

        waitForFixedUpdate = new WaitForFixedUpdate();

        Time.timeScale = 1;
    }

    private void FixedUpdate() {
        if (state != PlayerState.busy)
            HandleWalk();
    }

    private void LateUpdate() {
        if (
            state != PlayerState.usingWeapon
            && state != PlayerState.usingSpell
            && state != PlayerState.usingItem
            && state != PlayerState.knockbacked
        ) {
            if (movement != Vector3.zero && canChangeMoveDirection) {
                string currentSprite = GetMoveDirection();
                if (moveDirection != currentSprite)
                    moveDirection = currentSprite;
            }
        }
    }

    public void SetMovement(Vector2 v) {
        movement = v;
    }

    public Vector3 GetMovement() {
        return movement;
    }

    public void HandleWalk() {
        if (!blockWalkControlStates.Contains(state)) {
            if (movement != Vector3.zero) {
                if (canMove) {
                    state = PlayerState.walk;
                    rb.MovePosition(transform.position + movement * alteration.GetCurrentSpeed() * Time.deltaTime);
                    animator.SetBool("moving", true);
                } else {
                    state = PlayerState.idle;
                    animator.SetBool("moving", false);
                }
                if (canChangeMoveDirection) {
                    animator.SetFloat("moveX", movement.x);
                    animator.SetFloat("moveY", movement.y);
                }
            } else {
                state = PlayerState.idle;
                animator.SetBool("moving", false);
            }
        }
    }

    public void HandleSpawnWalk(SpawnPoint sp) {
        StartCoroutine(HandleAutomaticSpawnWalk(sp));
    }

    private IEnumerator HandleAutomaticSpawnWalk(SpawnPoint sp) {
        camPlayerMovement.UpdateArea(sp.area);
        // sp.area.HandleActorsActivation(true);
        playerCollider.enabled = false;
        state = PlayerState.busy;
        transform.position = sp.position;
        float destination, distance;
        switch (sp.behavior) {
            case BehaviorAtSpawning.shortWalk:
                distance = 1f;
                break;
            case BehaviorAtSpawning.longWalk:
                distance = 3f;
                break;
            default:
                distance = 1f;
                Debug.LogError($"UNABLE TO SET DISTANCE FOR THAT BEHAVIOR: {sp.behavior} - DEFAULT WILL BE 1");
                break;
        }
        animator.SetBool("moving", true);
        switch (sp.walkDirection) {
            case WalkAtSpawningDirection.right:
                animator.SetFloat("moveX", 1);
                animator.SetFloat("moveY", 0);
                moveDirection = "right";
                destination = transform.position.x + distance;
                camPlayerMovement.LockCameraOnTarget(CamTargetType.customPosition, new Vector2(transform.position.x + 1, transform.position.y));
                while (transform.position.x < destination) {
                    rb.MovePosition(transform.position + Vector3.right * speed * Time.deltaTime);
                    yield return waitForFixedUpdate;
                }
                break;
            case WalkAtSpawningDirection.left:
                animator.SetFloat("moveX", -1);
                animator.SetFloat("moveY", 0);
                moveDirection = "left";
                destination = transform.position.x - distance;
                camPlayerMovement.LockCameraOnTarget(CamTargetType.customPosition, new Vector2(transform.position.x - 1, transform.position.y));
                while (transform.position.x > destination) {
                    rb.MovePosition(transform.position + Vector3.left * speed * Time.deltaTime);
                    yield return waitForFixedUpdate;
                }
                break;
            case WalkAtSpawningDirection.up:
                animator.SetFloat("moveY", 1);
                animator.SetFloat("moveX", 0);
                moveDirection = "up";
                destination = transform.position.y + distance;
                camPlayerMovement.LockCameraOnTarget(CamTargetType.customPosition, new Vector2(transform.position.x, transform.position.y + 1));
                while (transform.position.y < destination) {
                    rb.MovePosition(transform.position + Vector3.up * speed * Time.deltaTime);
                    yield return waitForFixedUpdate;
                }
                break;
            case WalkAtSpawningDirection.down:
                animator.SetFloat("moveY", -1);
                animator.SetFloat("moveX", 0);
                moveDirection = "down";
                destination = transform.position.y - distance;
                camPlayerMovement.LockCameraOnTarget(CamTargetType.customPosition, new Vector2(transform.position.x, transform.position.y - 1));
                while (transform.position.y > destination) {
                    rb.MovePosition(transform.position + Vector3.down * speed * Time.deltaTime);
                    yield return waitForFixedUpdate;
                }
                break;
            default:
                Debug.LogError($"INVALID SPAWN WALK DIRECTION - {sp.walkDirection}");
                yield return null;
                break;
        }
        animator.SetBool("moving", false);
        playerCollider.enabled = true;
        canChangeMoveDirection = canMove = canInteractWithMapElements = true;
        camPlayerMovement.LockCameraOnTarget(CamTargetType.player);
        state = PlayerState.idle;
    }

    public void HandleSpawningPosition(SpawnPoint sp) {
        switch (sp.walkDirection) {
            case WalkAtSpawningDirection.right:
                animator.SetFloat("moveX", 1);
                animator.SetFloat("moveY", 0);
                moveDirection = "right";
                break;
            case WalkAtSpawningDirection.left:
                animator.SetFloat("moveX", -1);
                animator.SetFloat("moveY", 0);
                moveDirection = "left";
                break;
            case WalkAtSpawningDirection.up:
                animator.SetFloat("moveY", 1);
                animator.SetFloat("moveX", 0);
                moveDirection = "up";
                break;
            case WalkAtSpawningDirection.down:
                animator.SetFloat("moveY", -1);
                animator.SetFloat("moveX", 0);
                moveDirection = "down";
                break;
            default:
                animator.SetFloat("moveY", -1f);
                animator.SetFloat("moveX", 0f);
                moveDirection = "down";
                break;
        }
        transform.position = sp.position;
        camPlayerMovement.UpdateArea(sp.area);
        // sp.area.HandleActorsActivation(true);
        camPlayerMovement.LockCameraOnTarget(CamTargetType.player);
        animator.SetBool("moving", false);
        canChangeMoveDirection = canMove = canInteractWithMapElements = true;
        state = PlayerState.idle;
    }
    
    private string GetMoveDirection() {
        foreach (string d in new List<string> { "right", "left", "up", "down" })
            if (srd.sprite.name.Contains(d)) return d;
        return "none";
    }

    public void GetHeal(Int16 v) {
        if ((hitPoints + v) >= maxHP) {
            heartContainerBox.GetComponent<HeartsUI>().HealToFull();
            hitPoints = maxHP;
        } else {
            heartContainerBox.GetComponent<HeartsUI>().UpdateHealthBar(v);
            hitPoints += v;
        }
    }

    public void GetDamage(Int16 dmg) {
        if ((hitPoints - dmg) <= 0) {
            heartContainerBox.GetComponent<HeartsUI>().EmptyHealthBar();
            hitPoints = 0;
            Defeat();
        } else {
            heartContainerBox.GetComponent<HeartsUI>().UpdateHealthBar((Int16)(-dmg));
            hitPoints -= dmg;
        }
    }

    private void Defeat() {
        Debug.Log("--DEFEAT--");
    }

    public void SetSleepingMode(RigidbodySleepMode2D mode) {
        rb.sleepMode = mode;
    }

    public PlayerData SavePlayer(Int16 saveSlotNumber) {
        PlayerData d = new PlayerData() {
            playerName = playerName,
            mapName = currentSpawnPoint.area.mapName,
            spawnPointName = currentSpawnPoint.name,
            spawnPointLocationName = currentSpawnPoint.locationName,
            storyStatus = StoryTracker.currentStoryState.ToString(),
            savedElements = inventory.GetElementsInSlots(),
            equippedElements = inventory.GetEquippedElements(),
            maxHP = maxHP,
            hitPoints = hitPoints,
            maxMana = spell.maxMana,
            currentMana = spell.manaPoints,
            maxOrb = inventory.maxRupee,
            orbs = inventory.rupees,
            maxBomb = inventory.maxBomb,
            bombs = inventory.bombs,
            saveTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm"),
            playTime = playTime.GetTime(),
            regElm = globalMap.GetComponent<GlobalRegisteredElementsController>().GetRawRegisteredElements(),
            storyMilestones = storySwitch.GetComponent<StorySwitch>().GetAllMilestones(),
        };
        SaveSystem.SavePlayer(d, saveSlotNumber);
        return d;
    }

    public PlayerControl GetPlayerControl() {
        return playerControl.GetComponent<PlayerControl>();
    }
}
