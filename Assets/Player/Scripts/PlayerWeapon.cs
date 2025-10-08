using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponData {
    public Int16 AnimatorValue { get; set; }
    public string Name { get; set; }
    public List<PlayerState> AuthStates { get; set; }
}

public enum AttackState {
    none,
    staff,
    sword
}

public class PlayerWeapon : MonoBehaviour {
    public AttackState state;
    private PlayerMain main;
    public string comboDirection;
    public Int16 weaponAttackDamage;
    public bool isWeaponEnabled, isComboAllowed, isComboSuccessful, isComboFailed;
    private bool isComboTimerCoRunning;
    private readonly List<int> hitEnemies = new();
    private List<WeaponData> weaponDataList;
    private WeaponData usedWeapon;
    private Animator animator;
    private Rigidbody2D rb;

    [SerializeField]
    private Transform attackHitbox;
    [SerializeField]
    private GameObject hit, weaponUiButton, weaponUiIcon;

    private void Awake() {
        state = AttackState.none;
        main = GetComponent<PlayerMain>();
        isWeaponEnabled = true;
        weaponDataList = new() {
            new() {
                AnimatorValue = 0,
                Name = "",
                AuthStates = new List<PlayerState>()
            },
            new() {
                AnimatorValue = 1,
                Name = "staff",
                AuthStates = new List<PlayerState> {
                    PlayerState.idle,
                    PlayerState.walk,
                    PlayerState.usingWeapon
                }
            },
            new() {
                AnimatorValue = 2,
                Name = "sword",
                AuthStates = new List<PlayerState> {
                    PlayerState.idle,
                    PlayerState.walk,
                    PlayerState.usingWeapon
                }
            }
        };
        usedWeapon = weaponDataList[0];
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        animator.SetBool("isWeaponEnabled", isWeaponEnabled);
        animator.SetBool("isUsingWeapon", false);
        animator.SetInteger("weaponComboStatus", 1);
    }

    public void HandleChange(string weaponName, Sprite wpnSprite = null) {
        ResetCombo();
        if (isComboAllowed)
            isComboAllowed = false;
        weaponUiButton.GetComponent<ButtonManager>().HandleActivation((weaponName != "") ? true : false);
        weaponUiIcon.GetComponent<ButtonIconManager>().HandleSprite(wpnSprite);
        // weaponUiIcon.GetComponent<ButtonIconManager>().HandleVisibility(true);
        usedWeapon = weaponDataList.Find(d => d.Name == weaponName);
        animator.SetInteger("weapon", usedWeapon.AnimatorValue);
    }

    public void HandleAttack() {
        if (usedWeapon.AuthStates.Contains(main.state)) {
            switch (usedWeapon.Name) {
                case "":
                    break;
                case "staff":
                    if (main.state != PlayerState.usingWeapon) {
                        Int16 acs = (Int16)animator.GetInteger("weaponComboStatus");
                        isComboAllowed = acs != 2;
                        main.state = PlayerState.usingWeapon;
                        state = AttackState.staff;
                        main.canChangeMoveDirection = false;
                        if (acs == 1)
                            comboDirection = main.moveDirection;
                        animator.SetBool("isUsingWeapon", true);
                        SetAttackDamage(usedWeapon.Name);
                        if (isComboTimerCoRunning)
                            isComboSuccessful = true;
                    } else {
                        isComboAllowed = false;
                    }
                    break;
                case "sword":
                    if (main.state != PlayerState.usingWeapon) {
                        Int16 acs = (Int16)animator.GetInteger("weaponComboStatus");
                        isComboAllowed = acs != 3;
                        main.state = PlayerState.usingWeapon;
                        state = AttackState.sword;
                        main.canChangeMoveDirection = false;
                        if (acs == 1)
                            comboDirection = main.moveDirection;
                        animator.SetBool("isUsingWeapon", true);
                        SetAttackDamage(usedWeapon.Name);
                        if (isComboTimerCoRunning)
                            isComboSuccessful = true;
                    } else {
                        isComboAllowed = false;
                    }
                    break;
                default:
                    Debug.LogError($"NO BEHAVIOUR FOUND FOR WEAPON NAMED: {usedWeapon.Name}");
                    break;
            }
        }
    }

    private void SetAttackDamage(string weaponName) {
        weaponAttackDamage = 0;
        switch (weaponName) {
            case "staff":
                weaponAttackDamage = 2;
                break;
            case "sword":
                switch (animator.GetInteger("weaponComboStatus")) {
                    case 1:
                        weaponAttackDamage = 4;
                        break;
                    case 2:
                        weaponAttackDamage = 4;
                        break;
                    case 3:
                        weaponAttackDamage = 10;
                        break;
                    default:
                        Debug.LogError($"UNABLE TO HANDLE ATTACKCOMBOSTATUS: {animator.GetInteger("weaponComboStatus")}");
                        break;
                }
                break;
            default:
                Debug.LogError($"UNABLE TO SET ATTACK DAMAGE FOR WEAPON NAMED: {weaponName}");
                break;
        }
    }

    public void EndAttack(AnimationEvent e) {
        if (e.animatorClipInfo.clip.name.ToLower().Contains(main.moveDirection))
            FinishAttack();
    }

    public void InterruptAttack(PlayerState stateArg = PlayerState.idle) {
        FinishAttack(stateArg);
    }

    private void FinishAttack(PlayerState stateArg = PlayerState.idle) {
        main.state = stateArg;
        state = AttackState.none;
        main.canChangeMoveDirection = true;
        animator.SetBool("isUsingWeapon", false);
        hitEnemies.Clear();
        if (isComboAllowed) {
            animator.SetInteger("weaponComboStatus", animator.GetInteger("weaponComboStatus") + 1);
            StartCoroutine(ComboTimer());
            isComboAllowed = false;
        } else {
            ResetCombo();
        }
        weaponAttackDamage = 0;
        attackHitbox.gameObject.SetActive(false);
        attackHitbox.position = Vector2.zero;
    }

    private IEnumerator ComboTimer() {
        isComboTimerCoRunning = true;
        isComboFailed = isComboSuccessful = false;
        float timer = Time.time + 1f;
        while (Time.time < timer) {
            if (isComboFailed || isComboSuccessful)
                break;
            if (comboDirection != main.moveDirection)
                isComboFailed = true;
            yield return null;
        }
        if (!isComboSuccessful || isComboFailed)
            ResetCombo();
        isComboTimerCoRunning = false;
    }

    public void ResetCombo() {
        animator.SetInteger("weaponComboStatus", 1);
        comboDirection = "";
    }

    private void SetAttackHitbox(Vector2 position, Vector2 size, CapsuleDirection2D dir) {
        CapsuleCollider2D capsule = attackHitbox.GetComponent<CapsuleCollider2D>();
        attackHitbox.position = rb.position + position;
        capsule.size = size;
        capsule.direction = dir;
    }

    public Transform GetAttackHitbox() {
        return attackHitbox;
    }

    public void Strike(Collider2D collider) {
        if (collider.CompareTag("CollectiblesCommon") && collider.GetComponent<ItemToCollect>().GetAuth()) {
            collider.GetComponent<ItemToCollect>().HandleCollect(gameObject);
        } else if (collider.CompareTag("BreakableElm")) {
            if (collider.GetComponent<IBreakableElm>().GetMeleeWeaponBreakableAuth(weaponName: usedWeapon.Name))
                collider.GetComponent<IBreakableElm>().BreakElm(weaponName: usedWeapon.Name);
        } else if (!hitEnemies.Contains(collider.gameObject.GetInstanceID())) {
            hitEnemies.Add(collider.gameObject.GetInstanceID());
            collider
                .GetComponent<IGetHit>()
                .GetHit(weaponAttackDamage, gameObject, transform.position, kbThrust: weaponAttackDamage > 4 ? 75f : 40f, weaponName: usedWeapon.Name);
            Instantiate(hit, collider.transform.position, Quaternion.identity);
        }
    }

    public void AddElementsToHitEnemiesList(List<int> instanceID) {
        if (state == AttackState.none) return;
        foreach (int iid in instanceID) hitEnemies.Add(iid);
    }

    public List<int> GetRegEnemiesList() {
        return hitEnemies;
    }

    public int GetAttackId() {
        return attackHitbox.GetComponent<PlayerAttackHitbox>().WpnCount;
    }

    private void CheckAttackHitbox(AnimationEvent e) {
        if (!e.animatorClipInfo.clip.name.ToLower().Contains(main.moveDirection))
            return;
        string[] args = e.stringParameter.Split('_');
        switch (args[0]) {
            case "right":
                switch (args[1]) {
                    case "first":
                        SetAttackHitbox(
                            new Vector2(0.9f, -0.3f),
                            new Vector2(0.35f, 0.6f),
                            CapsuleDirection2D.Vertical
                        );
                        break;
                    case "second":
                        SetAttackHitbox(
                            new Vector2(0.9f, 0.3f),
                            new Vector2(0.35f, 0.6f),
                            CapsuleDirection2D.Vertical
                        );
                        break;
                    case "third":
                        switch (args[2]) {
                            case "1":
                                SetAttackHitbox(
                                    new Vector2(-0.3f, -0.9f),
                                    new Vector2(0.6f, 0.35f),
                                    CapsuleDirection2D.Horizontal
                                );
                                break;
                            case "2":
                                SetAttackHitbox(
                                    new Vector2(0.3f, -0.9f),
                                    new Vector2(0.6f, 0.35f),
                                    CapsuleDirection2D.Horizontal
                                );
                                break;
                            case "3":
                                SetAttackHitbox(
                                    new Vector2(0.9f, -0.3f),
                                    new Vector2(0.35f, 0.6f),
                                    CapsuleDirection2D.Vertical
                                );
                                break;
                            case "4":
                                SetAttackHitbox(
                                    new Vector2(0.9f, 0.3f),
                                    new Vector2(0.35f, 0.6f),
                                    CapsuleDirection2D.Vertical
                                );
                                break;
                            default:
                                Debug.LogError($"WITH DIRECTION {args[0]} AND COMBO ORDER {args[1]}, INVALID INTERNAL ORDER PROVIDED: {args[2]}");
                                break;
                        }
                        break;
                    default:
                        Debug.LogError($"WITH DIRECTION {args[0]}, INVALID COMBO ORDER PROVIDED: {args[1]}");
                        break;
                }
                break;
            case "left":
                switch (args[1]) {
                    case "first":
                        SetAttackHitbox(
                            new Vector2(-0.9f, 0.3f),
                            new Vector2(0.35f, 0.6f),
                            CapsuleDirection2D.Vertical
                        );
                        break;
                    case "second":
                        SetAttackHitbox(
                            new Vector2(-0.9f, -0.3f),
                            new Vector2(0.35f, 0.6f),
                            CapsuleDirection2D.Vertical
                        );
                        break;
                    case "third":
                        switch (args[2]) {
                            case "1":
                                SetAttackHitbox(
                                    new Vector2(0.3f, 0.9f),
                                    new Vector2(0.6f, 0.35f),
                                    CapsuleDirection2D.Horizontal
                                );
                                break;
                            case "2":
                                SetAttackHitbox(
                                    new Vector2(-0.3f, 0.9f),
                                    new Vector2(0.6f, 0.35f),
                                    CapsuleDirection2D.Horizontal
                                );
                                break;
                            case "3":
                                SetAttackHitbox(
                                    new Vector2(-0.9f, 0.3f),
                                    new Vector2(0.35f, 0.6f),
                                    CapsuleDirection2D.Vertical
                                );
                                break;
                            case "4":
                                SetAttackHitbox(
                                    new Vector2(-0.9f, -0.3f),
                                    new Vector2(0.35f, 0.6f),
                                    CapsuleDirection2D.Vertical
                                );
                                break;
                            default:
                                Debug.LogError($"WITH DIRECTION {args[0]} AND COMBO ORDER {args[1]}, INVALID INTERNAL ORDER PROVIDED: {args[2]}");
                                break;
                        }
                        break;
                    default:
                        Debug.LogError($"WITH DIRECTION {args[0]}, INVALID COMBO ORDER PROVIDED: {args[1]}");
                        break;
                }
                break;
            case "up":
                switch (args[1]) {
                    case "first":
                        SetAttackHitbox(
                            new Vector2(0.3f, 0.9f),
                            new Vector2(0.6f, 0.35f),
                            CapsuleDirection2D.Horizontal
                        );
                        break;
                    case "second":
                        SetAttackHitbox(
                            new Vector2(-0.3f, 0.9f),
                            new Vector2(0.6f, 0.35f),
                            CapsuleDirection2D.Horizontal
                        );
                        break;
                    case "third":
                        switch (args[2]) {
                            case "1":
                                SetAttackHitbox(
                                    new Vector2(0.9f, -0.3f),
                                    new Vector2(0.35f, 0.6f),
                                    CapsuleDirection2D.Vertical
                                );
                                break;
                            case "2":
                                SetAttackHitbox(
                                    new Vector2(0.9f, 0.3f),
                                    new Vector2(0.35f, 0.6f),
                                    CapsuleDirection2D.Vertical
                                );
                                break;
                            case "3":
                                SetAttackHitbox(
                                    new Vector2(0.3f, 0.9f),
                                    new Vector2(0.6f, 0.35f),
                                    CapsuleDirection2D.Horizontal
                                );
                                break;
                            case "4":
                                SetAttackHitbox(
                                    new Vector2(-0.3f, 0.9f),
                                    new Vector2(0.6f, 0.35f),
                                    CapsuleDirection2D.Horizontal
                                );
                                break;
                            default:
                                Debug.LogError($"WITH DIRECTION {args[0]} AND COMBO ORDER {args[1]}, INVALID INTERNAL ORDER PROVIDED: {args[2]}");
                                break;
                        }
                        break;
                    default:
                        Debug.LogError($"WITH DIRECTION {args[0]}, INVALID COMBO ORDER PROVIDED: {args[1]}");
                        break;
                }
                break;
            case "down":
                switch (args[1]) {
                    case "first":
                        SetAttackHitbox(
                            new Vector2(-0.3f, -0.9f),
                            new Vector2(0.6f, 0.35f),
                            CapsuleDirection2D.Horizontal
                        );
                        break;
                    case "second":
                        SetAttackHitbox(
                            new Vector2(0.3f, -0.9f),
                            new Vector2(0.6f, 0.35f),
                            CapsuleDirection2D.Horizontal
                        );
                        break;
                    case "third":
                        switch (args[2]) {
                            case "1":
                                SetAttackHitbox(
                                    new Vector2(-0.9f, 0.3f),
                                    new Vector2(0.35f, 0.6f),
                                    CapsuleDirection2D.Vertical
                                );
                                break;
                            case "2":
                                SetAttackHitbox(
                                    new Vector2(-0.9f, -0.3f),
                                    new Vector2(0.35f, 0.6f),
                                    CapsuleDirection2D.Vertical
                                );
                                break;
                            case "3":
                                SetAttackHitbox(
                                    new Vector2(-0.3f, -0.9f),
                                    new Vector2(0.6f, 0.35f),
                                    CapsuleDirection2D.Horizontal
                                );
                                break;
                            case "4":
                                SetAttackHitbox(
                                    new Vector2(0.3f, -0.9f),
                                    new Vector2(0.6f, 0.35f),
                                    CapsuleDirection2D.Horizontal
                                );
                                break;
                            default:
                                Debug.LogError($"WITH DIRECTION {args[0]} AND COMBO ORDER {args[1]}, INVALID INTERNAL ORDER PROVIDED: {args[2]}");
                                break;
                        }
                        break;
                    default:
                        Debug.LogError($"WITH DIRECTION {args[0]}, INVALID COMBO ORDER PROVIDED: {args[1]}");
                        break;
                }
                break;
            default:
                Debug.LogError($"INVALID DIRECTION PROVIDED: {args[0]}");
                break;
        }

        if (!attackHitbox.gameObject.activeSelf)
            attackHitbox.gameObject.SetActive(true);
    }
}
