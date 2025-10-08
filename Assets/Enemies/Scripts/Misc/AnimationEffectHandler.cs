using UnityEngine;

public class AnimationEffectHandler : MonoBehaviour {
    public void End() {
        Destroy(gameObject);
    }

    private void OnDisable() {
        Destroy(gameObject);
    }
}
