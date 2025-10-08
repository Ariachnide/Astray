using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellData {
    public Int16 AnimatorValue { get; set; }
    public string Name { get; set; }
    public Int16 ManaCost { get; set; }
    public List<PlayerState> AuthStates { get; set; }
}

public enum SpellState {
    none,
    fireball,
    whirlwind
}

public class PlayerSpell : MonoBehaviour {
    public SpellState state;
    private PlayerMain main;
    [SerializeField]
    private GameObject manaContainerBox;
    public Int16 manaPoints, maxMana, spellAttackDamage;
    public bool isSpellEnabled;
    private List<SpellData> spellDataList;
    private SpellData usedSpell;
    private Animator animator;

    [SerializeField]
    private GameObject spellUiButton, spellUiIcon;
    [SerializeField]
    private List<GameObject> spellGOList;

    private void Awake() {
        state = SpellState.none;
        main = GetComponent<PlayerMain>();
        animator = GetComponent<Animator>();
        isSpellEnabled = true;
        animator.SetBool("isSpellEnabled", isSpellEnabled);
        animator.SetBool("isUsingSpell", false);
        spellDataList = new List<SpellData> {
            new() {
                AnimatorValue = 0,
                Name = "",
                ManaCost = 0,
                AuthStates = new List<PlayerState>()
            },
            new() {
                AnimatorValue = 1,
                Name = "fireball",
                ManaCost = 2,
                AuthStates = new List<PlayerState> {
                    PlayerState.idle,
                    PlayerState.walk
                }
            },
            new() {
                AnimatorValue = 1,
                Name = "whirlwind",
                ManaCost = 4,
                AuthStates = new List<PlayerState> {
                    PlayerState.idle,
                    PlayerState.walk
                }
            }
        };
        usedSpell = spellDataList[0];
    }

    public bool IsCurrentManaEnough() {
        return manaPoints >= usedSpell.ManaCost;
    } 

    public void HandleChange(string spellName, Sprite splSprite = null) {
        usedSpell = spellDataList.Find(d => d.Name == spellName);
        spellUiButton.GetComponent<ButtonManager>().HandleActivation(
            (spellName != "") && IsCurrentManaEnough()
        );
        spellUiIcon.GetComponent<ButtonIconManager>().HandleVisibility(IsCurrentManaEnough());
        spellUiIcon.GetComponent<ButtonIconManager>().HandleSprite(splSprite);
        animator.SetInteger("spell", usedSpell.AnimatorValue);
    }

    public bool IsUsable() {
        return IsCurrentManaEnough();
    }

    public void CheckButtonUIVisibility() {
        spellUiButton.GetComponent<ButtonManager>().HandleActivation(IsUsable());
        spellUiIcon.GetComponent<ButtonIconManager>().HandleVisibility(IsUsable());
    }

    public void AddMana(Int16 v) {
        if ((manaPoints + v) >= maxMana) {
            manaContainerBox.GetComponent<ManaUI>().ReatoreManaToFull();
            maxMana = manaPoints;
        } else {
            manaContainerBox.GetComponent<ManaUI>().UpdateManaBar(v);
            manaPoints += v;
        }
        CheckButtonUIVisibility();
    }

    public void RemoveMana(Int16 v) {
        if (manaPoints == 0) return;
        if ((manaPoints - v) <= 0) {
            manaContainerBox.GetComponent<ManaUI>().EmptyManaBar();
            manaPoints = 0;
        } else {
            manaContainerBox.GetComponent<ManaUI>().UpdateManaBar((Int16)(-v));
            manaPoints -= v;
        }
        CheckButtonUIVisibility();
    }

    public void Use() {
        if (!IsCurrentManaEnough()) return;
        if (usedSpell.AuthStates.Contains(main.state)) {
            RemoveMana(usedSpell.ManaCost);
            switch (usedSpell.Name) {
                case "":
                    break;
                case "fireball":
                    if (main.state != PlayerState.usingSpell) {
                        main.state = PlayerState.usingSpell;
                        state = SpellState.fireball;
                        main.canChangeMoveDirection = false;
                        animator.SetBool("isUsingSpell", true);
                        if (animator.GetInteger("spell") != usedSpell.AnimatorValue)
                            animator.SetInteger("spell", usedSpell.AnimatorValue);
                        SetAttackDamage();
                        GameObject spellGO = Instantiate(
                            spellGOList.Find(d => d.GetComponent<SpellManager>().spellName == usedSpell.Name),
                            SetCastStartingPoint(),
                            Quaternion.identity
                        );
                        spellGO.GetComponent<SpellManager>().Cast(SetCastDirection(), AnimosityState.Player, gameObject, argDmg: spellAttackDamage);
                        StartCoroutine(SpellAnimTimer());
                    }
                    break;
                case "whirlwind":
                    if (main.state != PlayerState.usingSpell) {
                        main.state = PlayerState.usingSpell;
                        state = SpellState.whirlwind;
                        main.canChangeMoveDirection = false;
                        animator.SetBool("isUsingSpell", true);
                        if (animator.GetInteger("spell") != usedSpell.AnimatorValue)
                            animator.SetInteger("spell", usedSpell.AnimatorValue);
                        GameObject spellGO = Instantiate(
                            spellGOList.Find(d => d.GetComponent<SpellManager>().spellName == usedSpell.Name),
                            SetCastStartingPoint(),
                            Quaternion.identity
                        );
                        spellGO.GetComponent<SpellManager>().Cast(SetCastDirection(), AnimosityState.Player, gameObject);
                        StartCoroutine(SpellAnimTimer());
                    }
                    break;
            }
        }
    }

    private void SetAttackDamage() {
        spellAttackDamage = 0;
        switch (usedSpell.Name) {
            case "fireball":
                spellAttackDamage = 3;
                break;
            default:
                Debug.LogError($"UNABLE TO SET SPELL DAMAGE FOR SPELL NAMED: {usedSpell.Name}");
                break;
        }
    }

    private Vector2 SetCastStartingPoint() {
        switch (main.moveDirection) {
            case "right":
                return new Vector2(transform.position.x + 0.75f, transform.position.y);
            case "left":
                return new Vector2(transform.position.x - 0.75f, transform.position.y);
            case "up":
                return new Vector2(transform.position.x, transform.position.y + 0.75f);
            case "down":
                return new Vector2(transform.position.x, transform.position.y - 0.75f);
            default:
                return transform.position;
        }
    }

    private Vector2 SetCastDirection() {
        switch (main.moveDirection) {
            case "right":
                return Vector2.right;
            case "left":
                return Vector2.left;
            case "up":
                return Vector2.up;
            case "down":
                return Vector2.down;
            default:
                return Vector2.down;
        }
    }

    public void InterruptSpellAnim(PlayerState stateArg = PlayerState.idle) {
        FinishSpellAnim(stateArg);
    }

    private void FinishSpellAnim(PlayerState stateArg = PlayerState.idle) {
        main.state = stateArg;
        state = SpellState.none;
        main.canChangeMoveDirection = true;
        animator.SetBool("isUsingSpell", false);
    }

    private IEnumerator SpellAnimTimer(float castTime = 0.5f) {
        float timer = Time.time + castTime;
        while (Time.time < timer) {
            yield return null;
        }
        FinishSpellAnim();
    }
}
