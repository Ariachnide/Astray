using UnityEngine;

public class EnemyProjectileAttack : AttackEnemyFactory {
    [SerializeField] private GameObject projectile;
    public VisionDirection defaultDirection = VisionDirection.None;
    public float
        projectileThrust = 8.5f,
        projectileFlightDuration = 2.5f;

    private VisionDirection GetVisionDirection() {
        if (hasAggroEF) {
            return aggroEF.visionDirection;
        } else {
            return defaultDirection;
        }
    }

    public override void Execute() {
        if (hasMovementEF) movementEF.InterruptMoves();
        Vector2 projectileOriginPoint = Vector2.zero;
        Vector2 shootDirection = Vector2.zero;
        switch (GetVisionDirection()) {
            case VisionDirection.Right:
                projectileOriginPoint = new Vector2(transform.position.x + 0.75f, transform.position.y);
                shootDirection = Vector2.right;
                break;
            case VisionDirection.Left:
                projectileOriginPoint = new Vector2(transform.position.x - 0.75f, transform.position.y);
                shootDirection = Vector2.left;
                break;
            case VisionDirection.Up:
                projectileOriginPoint = new Vector2(transform.position.x, transform.position.y + 0.75f);
                shootDirection = Vector2.up;
                break;
            case VisionDirection.Down:
                projectileOriginPoint = new Vector2(transform.position.x, transform.position.y - 0.75f);
                shootDirection = Vector2.down;
                break;
            default:
                Debug.LogError($"UNABLE FOR ENEMY CALLED \"{gameObject.name}\" TO SET UP PROJECTILE ORIGIN WITH MOVE DIRECTION: {GetVisionDirection()}");
                break;
        }
        GameObject rockGO = Instantiate(projectile, projectileOriginPoint, Quaternion.identity);
        rockGO.transform.SetParent(transform.parent);
        rockGO.GetComponent<ILaunchProjectile>().HandleLaunch(
            shootDirection,
            gameObject,
            projectileThrust,
            projectileFlightDuration,
            attackDmg,
            false
        );
    }

    public override void HandleAttackPermission(bool allow) {
        canAttack = allow;
        if (hasAggroEF && !allow) aggroEF.EndAggressiveBehaviour();
    }
}
