using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TinyBlobAlterationType {
    Red
}

public class PlayerAlteration : MonoBehaviour {
    private PlayerMain main;
    private PlayerControl pControl;
    [SerializeField]
    private GameObject alterationContainer;
    private float
        moveSpeedNegativeModifier,
        moveSpeedPositiveModifier,
        tempSpeed;

    [SerializeField]
    private GameObject tinyRedBlob;
    private readonly List<GameObject> tinyRedBlobList = new();
    private Coroutine handleTinyRedBlobCo = null;

    private void Awake() {
        main = GetComponent<PlayerMain>();
    }

    private void Start() {
        pControl = main.GetPlayerControl();
    }

    public float GetCurrentSpeed() {
        tempSpeed = main.speed - moveSpeedNegativeModifier + moveSpeedPositiveModifier;
        if (tempSpeed < (main.speed - 4)) {
            return main.speed - 4;
        } else if (tempSpeed > (main.speed + 6)) {
            return main.speed + 6;
        } else {
            return tempSpeed;
        }
    }

    public void PurgeMinorAlterations() {
        foreach (GameObject tb in tinyRedBlobList) {
            tb.GetComponent<TinyRedBlobAlt>().Purge();
            moveSpeedNegativeModifier -= 2;
        }
        tinyRedBlobList.Clear();
    }

    public void AffectTinyBlob(GameObject sourceBlob, Int16 persistance, TinyBlobAlterationType blobType) {
        switch (blobType) {
            case TinyBlobAlterationType.Red:
                GameObject tinyRedBlobGO = AddTinyRedBlob(sourceBlob, persistance);
                handleTinyRedBlobCo ??= StartCoroutine(HandleTinyRedBlob());
                pControl.OverrideControls(tinyRedBlobGO.GetComponent<IControlOverride>());
                break;
        }
    }

    private IEnumerator HandleTinyRedBlob() {
        Int16 delay = 0;

        while (tinyRedBlobList.Count > 0) {
            delay++;

            if (Math.Abs(main.GetMovement().x) > 0.4f || Math.Abs(main.GetMovement().y) > 0.4f)
                delay += 2;

            if (delay >= 25) {
                foreach (GameObject tb in tinyRedBlobList)
                    tb.GetComponent<TinyRedBlobAlt>().persistance -= delay;
                delay = 0;
            }
            yield return null;
        }
        EndHandleTinyRedBlob();
    }

    private GameObject AddTinyRedBlob(GameObject sourceBlob, Int16 persistance) {
        GameObject newTinyBlob = Instantiate(tinyRedBlob, transform.position, Quaternion.identity);
        newTinyBlob.transform.SetParent(alterationContainer.transform);
        newTinyBlob.GetComponent<TinyRedBlobAlt>().InitialSetup(sourceBlob, gameObject, persistance);
        tinyRedBlobList.Add(newTinyBlob);

        moveSpeedNegativeModifier += 2;

        return newTinyBlob;
    }

    public void RemoveTinyBlob(int id) {
        int blobToRemoveListId = tinyRedBlobList.FindIndex(e => e.GetInstanceID() == id);
        tinyRedBlobList.RemoveAt(blobToRemoveListId);
        if (tinyRedBlobList.Count > 0) pControl.UpdateOverridenControls(tinyRedBlobList[0]);

        moveSpeedNegativeModifier -= 2;
    }

    private void EndHandleTinyRedBlob() {
        if (handleTinyRedBlobCo != null) {
            StopCoroutine(handleTinyRedBlobCo);
            handleTinyRedBlobCo = null;
        }
        pControl.DeleteOverridenControls(CtrlOverriderType.TinyBlob);
    }
}
