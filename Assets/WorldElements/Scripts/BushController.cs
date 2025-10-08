using System;
using System.Collections.Generic;
using UnityEngine;

public class BushController : MonoBehaviour, IBreakableElm {
    public List<string> weaponMeleeVulnerability, spellVulnerability;
    private LootController lootController;
    [SerializeField]
    private GameObject leaf;

    private void Awake() {
        weaponMeleeVulnerability = new List<string>() {
            "sword",
            "bomb"
        };
        spellVulnerability = new List<string>() {
            "fireball",
            "whirlwind"
        };
        lootController = GetComponent<LootController>();
    }

    public bool GetMeleeWeaponBreakableAuth(string weaponName, Int16 damage = 0) {
        if (weaponMeleeVulnerability.Contains(weaponName)) return true;
        return false;
    }

    public void BreakElm(string weaponName = "", Int16 damage = 0, bool allowLoot = true) {
        if (allowLoot) {
            GameObject loot = lootController.HandleLoot();
            if (loot != null) {
                GameObject lootGO = Instantiate(loot, transform.position, Quaternion.identity);
                lootGO.GetComponent<ItemToCollect>().Fade();
                lootGO.GetComponent<ItemToCollect>().SetAuth(true);
            }
        }
        GameObject leafTL = Instantiate(
            leaf,
            new Vector2(
                transform.position.x - 0.15f,
                transform.position.y + 0.15f
            ),
            Quaternion.identity
        );
        leafTL.GetComponent<FlyingLeaf>().FlyTopLeft();
        GameObject leafTR = Instantiate(
            leaf,
            new Vector2(
                transform.position.x + 0.15f,
                transform.position.y + 0.15f
            ),
            Quaternion.identity
        );
        leafTR.GetComponent<FlyingLeaf>().FlyTopRight();
        GameObject leafBL = Instantiate(
            leaf,
            new Vector2(
                transform.position.x - 0.15f,
                transform.position.y - 0.15f
            ),
            Quaternion.identity
        );
        leafBL.GetComponent<FlyingLeaf>().FlyBottomLeft();
        GameObject leafBR = Instantiate(
            leaf,
            new Vector2(
                transform.position.x + 0.15f,
                transform.position.y - 0.15f
            ),
            Quaternion.identity
        );
        leafBR.GetComponent<FlyingLeaf>().FlyBottomRight();
        Destroy(gameObject);
    }
}
