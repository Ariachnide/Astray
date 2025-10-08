using System.Collections.Generic;
using UnityEngine;

public class Shadow : MonoBehaviour {
    private SpriteRenderer srd;
    private Renderer rd;
    [SerializeField]
    private List<Sprite> spriteList;

    private void Awake() {
        srd = GetComponent<SpriteRenderer>();
        rd = GetComponent<Renderer>();
    }

    public void SetSize(float v) {
        string s = EstimateSize(v);
        switch (s) {
            case "xs":
                srd.sprite = spriteList[0];
                break;
            case "s":
                srd.sprite = spriteList[1];
                break;
            case "m":
                srd.sprite = spriteList[2];
                break;
            case "l":
                srd.sprite = spriteList[3];
                break;
            case "xl":
                srd.sprite = spriteList[4];
                break;
            default:
                Debug.LogError($"UNRECOGNIZED SHADOW SIZE: {s}");
                break;
        }
    }

    public void ToggleVisibility(bool v) {
        rd.enabled = v;
    }

    private string EstimateSize(float v) {
        return v switch {
            (< 0.625f) => "xs",
            (< 0.75f) => "s",
            (< 0.875f) => "m",
            (< 1f) => "l",
            _ => "xl"
        };
    }

    public void End() {
        Destroy(gameObject);
    }

    public void UpdateRigidBody(float mass, float linearDrag) {
        GetComponent<Rigidbody2D>().mass = mass;
        GetComponent<Rigidbody2D>().drag = linearDrag;
    }
}
