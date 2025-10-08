using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedBlobMain : DefaultEnemyMain {
    [SerializeField] private GameObject divisionAnimation;
    [SerializeField] private GameObject tinyBlobGO;

    protected override IEnumerator Knockback(Vector3 argPosition, float duration, float thrust) {
        yield return base.Knockback(argPosition, duration / 2, thrust / 2);
    }

    public override void CheckHP(bool animate = true) {
        if (hitPoints <= 0) {
            if (animate) {
                if (hitPoints <= -8) {
                    GameObject destructionAnimatonGO = Instantiate(destructionAnimation, transform.position, Quaternion.identity);
                    destructionAnimatonGO.transform.SetParent(transform.parent);
                    destructionAnimatonGO.GetComponent<EnemyDestructionEffect>().loot = lootController.HandleLoot();
                    Destroy(gameObject);
                } else {
                    GameObject tbGO1 = Instantiate(
                        tinyBlobGO,
                        new Vector2(
                            transform.position.x - 0.25f,
                            transform.position.y - 0.25f
                        ),
                        Quaternion.identity
                    );
                    tbGO1.GetComponent<MainEnemyFactory>().HandleSummoning(
                        transform,
                        GetComponent<IHandleActivationPermission>().GetPermissions()
                    );

                    GameObject tbGO2 = Instantiate(
                        tinyBlobGO,
                        new Vector2(
                            transform.position.x + 0.25f,
                            transform.position.y - 0.25f
                        ),
                        Quaternion.identity
                    );
                    tbGO2.GetComponent<MainEnemyFactory>().HandleSummoning(
                        transform,
                        GetComponent<IHandleActivationPermission>().GetPermissions()
                    );

                    if (lastPlayerAttackData.wpnAttackId == lastPlayerAttackData.player.GetComponent<PlayerWeapon>().GetAttackId())
                        lastPlayerAttackData.player.GetComponent<PlayerWeapon>().AddElementsToHitEnemiesList(
                            new List<int>() { tbGO1.GetInstanceID(), tbGO2.GetInstanceID() }
                        );

                    Destroy(gameObject);
                }
            } else {
                Destroy(gameObject);
            }
        }
    }
}
