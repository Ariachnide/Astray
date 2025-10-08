using UnityEngine;

public abstract class VisionEnemyAbstractClass : MonoBehaviour {
    protected abstract void SetHorizontalCapsule();
    protected abstract void SetVerticalCapsule();
    public abstract void SetVisionRight();
    public abstract void SetVisionLeft();
    public abstract void SetVisionUp();
    public abstract void SetVisionDown();
}

public class VisionEnemyFactory : VisionEnemyAbstractClass {
    protected override void SetHorizontalCapsule() { }
    protected override void SetVerticalCapsule() { }
    public override void SetVisionRight() { }
    public override void SetVisionLeft() { }
    public override void SetVisionUp() { }
    public override void SetVisionDown() { }
}
