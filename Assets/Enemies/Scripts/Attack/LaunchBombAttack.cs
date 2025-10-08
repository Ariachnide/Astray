using UnityEngine;

public class LaunchBombAttack : AttackEnemyFactory {
    [SerializeField] private GameObject bomb;
    private GameObject instantiatedBombGO;
    public float bombCountDown;
    [SerializeField] private bool hasDefaultSpawnPosition;
    [SerializeField] private Vector3 spawnPosition;
    [SerializeField] private float thrust;

    public override void Execute() {
        isAttacking = true;
        if (hasAnimatorEF) animatorEF.Attack(true);
    }

    public override void Interrupt() {
        EndAttackMove();
    }

    public override void HandleAttackPermission(bool allow) {
        canAttack = allow;
        if (hasAggroEF && !allow) aggroEF.EndAggressiveBehaviour();
    }

    private void EndAttackMove() {
        if (hasAnimatorEF) animatorEF.Attack(false);
        isAttacking = false;
        if (instantiatedBombGO != null) {
            Destroy(instantiatedBombGO);
            instantiatedBombGO = null;
        }
    }

    private void PrepareBomb() {
        if (instantiatedBombGO != null) return; // Animator will play this fx two times
        instantiatedBombGO = Instantiate(bomb, transform.position + spawnPosition, Quaternion.identity);
        instantiatedBombGO.transform.SetParent(transform.parent);
        instantiatedBombGO.GetComponent<BombToLaunch>().UpdateSpriteRenderer(6, "Mid-Level");
    }

    private void SendBomb() {
        if (instantiatedBombGO == null) return; // Animator will play this fx two times
        instantiatedBombGO.GetComponent<BombController>().StartCountDown(bombCountDown);
        instantiatedBombGO.GetComponent<BombToLaunch>().UpdateSpriteRenderer(3, "Sub-Level");

        instantiatedBombGO.GetComponent<BombToLaunch>().AddForceToRigidbody(
            GetComponent<ILauchDirection>().GetLauchDirection(),
            thrust
        );
        instantiatedBombGO = null;
        EndAttackMove();
    }
}
