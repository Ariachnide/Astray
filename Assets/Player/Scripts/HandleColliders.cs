using System.Collections.Generic;
using UnityEngine;

public class HandleColliders : MonoBehaviour {
    public List<GameObject> GetCollidersObjects() {
        List<GameObject> elements = new();
        List<string> usedTags = new() {
            "Enemy",
            "Player",
            "HostileProjectile",
            "AllyProjectile",
            "HostileSpell",
            "AllySpell"
        };
        foreach (GameObject e in FindObjectsOfType<GameObject>())
            if (usedTags.Contains(e.tag)) elements.Add(e);
        elements.Remove(gameObject);
        return elements;
    }
}
