using UnityEngine;

public class MarkPointGroupManager : MonoBehaviour {
    [SerializeField] private Transform forbiddenAreas;

    public void CallExpulsions() {
        foreach (Transform child in forbiddenAreas)
            child.GetComponent<ForbiddenAreaMapChanger>().ExpelEnemies();
    }
}
