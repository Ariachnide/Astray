using UnityEngine;

public interface IHitByShekra {
    public void HandleShekraHit(Vector3 shekraPosition, float duration, float thrust, bool destroyAfterHit);
    public void Blink(float d);
}
