using UnityEngine;

public enum SpecialEffectType {
    Whirl
}

public interface IHandleSpecialEffects {
    public void SetSpecialEffect(SpecialEffectType effect, float duration, GameObject effectGO = null);
    public void EndSpecialEffect(SpecialEffectType effect);
}
