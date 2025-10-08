using UnityEngine;

public interface ICustomRestrainer {
    public bool CanKeepTarget(GameObject otherRestrainer);
    public void CustomEffect(GameObject player);
    public void CustomHit(GameObject player);
    public void End();
    public void RestoreDefaultBehavior();
}
