public class GuardSwordsmanMain : DefaultEnemyMain {
    GuardSwordsmanAggro guardSwordsmanAggro;

    protected override void Awake() {
        guardSwordsmanAggro = GetComponent<GuardSwordsmanAggro>();
        base.Awake();
    }

    public override void Spin(float duration) {
        guardSwordsmanAggro.EndKeepMovingTowardsTarget(false, false, false);
        base.Spin(duration);
    }
}
