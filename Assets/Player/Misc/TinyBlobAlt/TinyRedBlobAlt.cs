using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TinyRedBlobAlt : MonoBehaviour, IControlOverride {
    private GameObject source, affectedPlayer;
    private SpriteRenderer srd;
    public Int16 persistance;
    private Vector3 destination = Vector3.zero;
    public float speed = 1.75f;
    private float previousXValue = 0f, nextXValue = 0f;
    [SerializeField] private GameObject splashEffect;

    private Int16 randomBlocker, randomState;
    [SerializeField] private readonly Int16 maxRandom = 3;

    private readonly List<PlayerControlType> overridenControls = new List<PlayerControlType>() {
        PlayerControlType.Action,
        PlayerControlType.Weapon,
        PlayerControlType.Spell,
        PlayerControlType.OtherObject
    };
    private readonly Int16 ctrlOverridePriority = 15;
    private readonly CtrlOverriderType ctrlOverriderType = CtrlOverriderType.TinyBlob;

    private void Awake() {
        if (ctrlOverridePriority == 0) Debug.LogError("WARNING: NO PRIORITY DEFINED FOR ctrlOverridePriority");
    }

    public void InitialSetup(
        GameObject sourceArg,
        GameObject affectedPlayerArg,
        Int16 persistanceArg
    )   {
        source = sourceArg;
        affectedPlayer = affectedPlayerArg;
        persistance = persistanceArg;

        randomBlocker = (Int16)UnityEngine.Random.Range(0, maxRandom);
        randomState = 0;

        srd = GetComponent<SpriteRenderer>();
        srd.sortingOrder = 6;
        transform.position = new Vector2(
            affectedPlayer.transform.position.x,
            affectedPlayer.transform.position.y - 0.2f
        );
    }

    public void Purge() {
        affectedPlayer.GetComponent<PlayerMain>().GetPlayerControl().DeleteOverridenControls(GetOverriderType());
        Destroy(source);
        Destroy(gameObject);
    }

    private void FixedUpdate() {
        if (persistance <= 0) {
            RestoreSource();
            return;
        }

        source.GetComponent<Rigidbody2D>().MovePosition(affectedPlayer.transform.position);
        nextXValue = Mathf.PingPong(Time.time * speed, 1.4f) - 0.7f;

        if (srd.sortingOrder == 6) {
            if (nextXValue < previousXValue) srd.sortingOrder = 4;
        } else {
            if (nextXValue > previousXValue) srd.sortingOrder = 6;
        }

        destination.x = nextXValue;
        destination.y = Mathf.PingPong(Time.time * (speed / 4f), 0.5f) - 0.25f;
        transform.position = transform.position + speed * Time.deltaTime * destination;

        previousXValue = nextXValue;
    }

    private void RestoreSource() {
        affectedPlayer.GetComponent<PlayerAlteration>().RemoveTinyBlob(gameObject.GetInstanceID());
        source.SetActive(true);
        source.GetComponent<TinyBlobCollision>().ReactivateSelfAfterPlayerAlt(affectedPlayer, transform.position);
        Destroy(gameObject);
    }

    public void SplashReaction() {
        GameObject splashEffectInstance = Instantiate(splashEffect, transform.position, Quaternion.identity);
        splashEffectInstance.transform.SetParent(source.transform.parent);
    }

    private bool CheckRandomChance() {
        if (randomState == randomBlocker) {
            randomBlocker = (Int16)UnityEngine.Random.Range(0, maxRandom);
            randomState = 0;
            return true;
        } else {
            randomState++;
            return false;
        }
    }

    // IControlOverride methods
    public NextOrderType CallOverrideInterceptor(PlayerControlType controlType, InputAction.CallbackContext context) {
        if (!context.started) return NextOrderType.Continue;

        switch (controlType) {
            case PlayerControlType.Action:
                persistance -= 15;
                return NextOrderType.Stop;
            case PlayerControlType.Weapon:
                if (affectedPlayer.GetComponent<PlayerInventory>().GetEquippedWeapon().elementName == "empty") {
                    persistance -= 3;
                    return NextOrderType.Continue;
                }
                if (CheckRandomChance()) {
                    persistance -= 100;
                    return NextOrderType.AllowAction;
                } else {
                    GameObject splashEffectGO = Instantiate(splashEffect, transform.position, Quaternion.identity);
                    splashEffectGO.transform.SetParent(source.transform.parent);
                    persistance -= 30;
                }
                break;
            case PlayerControlType.Spell:
                if (
                    affectedPlayer.GetComponent<PlayerInventory>().GetEquippedItem().elementName == "empty"
                    || !affectedPlayer.GetComponent<PlayerSpell>().IsUsable()
                ) {
                    persistance -= 3;
                    return NextOrderType.Continue;
                }
                if (CheckRandomChance()) {
                    persistance -= 60;
                    return NextOrderType.AllowAction;
                } else {
                    GameObject splashEffectGO = Instantiate(splashEffect, transform.position, Quaternion.identity);
                    splashEffectGO.transform.SetParent(source.transform.parent);
                    persistance -= 20;
                }
                break;
            case PlayerControlType.OtherObject:
                if (
                    affectedPlayer.GetComponent<PlayerInventory>().GetEquippedItem().elementName == "empty"
                    || !affectedPlayer.GetComponent<PlayerItem>().IsUsable()
                ) {
                    persistance -= 3;
                    return NextOrderType.Continue;
                }
                if (CheckRandomChance()) {
                    persistance -= 30;
                    return NextOrderType.AllowAction;
                } else {
                    GameObject splashEffectGO = Instantiate(splashEffect, transform.position, Quaternion.identity);
                    splashEffectGO.transform.SetParent(source.transform.parent);
                    persistance -= 10;
                }
                break;
        }

        return NextOrderType.Continue;
    }

    public List<PlayerControlType> GetOverridenControls() {
        return overridenControls;
    }

    public Int16 GetOverridePriority() {
        return ctrlOverridePriority;
    }

    public CtrlOverriderType GetOverriderType() {
        return ctrlOverriderType;
    }

    public GameObject GetRelatedGameObject() {
        return gameObject;
    }
    // end IControlOverride methods
}
