using UnityEngine;

public class EnemyDestructionEffect : MonoBehaviour {
    public GameObject loot, shadow;

    public void End() {
        if (loot != null) {
            GameObject lootGO = Instantiate(loot, transform.position, Quaternion.identity);
            GameObject shadowGO = Instantiate(
                shadow,
                new Vector2(transform.position.x, transform.position.y - lootGO.GetComponent<CapsuleCollider2D>().size.y / 2),
                Quaternion.identity
            );
            shadowGO.GetComponent<Shadow>().SetSize(lootGO.GetComponent<CapsuleCollider2D>().size.y);
            shadowGO.GetComponent<Shadow>().ToggleVisibility(true);
            lootGO.GetComponent<ItemToCollect>().Fade();
            lootGO.GetComponent<ItemToCollect>().SetAuth(true);
            lootGO.GetComponent<ItemToCollect>().SpawnAnim(shadowGO);
        }
        Destroy(gameObject);
    }

    private void OnDisable() {
        Destroy(gameObject);
    }
}
