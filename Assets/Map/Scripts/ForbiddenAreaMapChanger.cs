using System.Collections.Generic;
using UnityEngine;

public class ForbiddenAreaMapChanger : MonoBehaviour {
    public List<GameObject> enemiesInArea = new List<GameObject>();

    private void OnTriggerEnter2D(Collider2D collider) {
        if (collider.CompareTag("Enemy")) enemiesInArea.Add(collider.gameObject);
    }

    private void OnTriggerExit2D(Collider2D collider) {
        if (collider.CompareTag("Enemy")) enemiesInArea.Remove(collider.gameObject);
    }

    public void ExpelEnemies() {
        foreach (GameObject e in enemiesInArea)
            e.GetComponent<IHandleDefaultSettings>().ResetPosition();
        enemiesInArea.Clear();
    }
}
