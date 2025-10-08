using System;
using UnityEngine;

public class SelectorPassive : MonoBehaviour {
    private bool isGrowing;
    private Int16 maxFrames, frames;
    private Vector3 minScale, maxScale;

    private void Awake() {
        minScale = new Vector3(1, 1, 1);
        maxScale = new Vector3(2, 2, 1);
        maxFrames = 125;
        isGrowing = true;
    }

    private void Update() {
        if (isGrowing) {
            if (frames < maxFrames) {
                transform.localScale = Vector3.Lerp(minScale, maxScale, (float)frames / maxFrames);
                frames++;
            } else {
                frames = 0;
                isGrowing = false;
            }
        } else {
            if (frames < maxFrames) {
                transform.localScale = Vector3.Lerp(maxScale, minScale, (float)frames / maxFrames);
                frames++;
            } else {
                frames = 0;
                isGrowing = true;
            }
        }
    }
}
