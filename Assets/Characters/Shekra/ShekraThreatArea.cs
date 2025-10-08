using UnityEngine;

public class ShekraThreatArea : MonoBehaviour {
    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.name.Contains("GuardDummy")) {
            collision.gameObject.GetComponent<IHitByShekra>().HandleShekraHit(transform.position, 0.25f, 35f, true);
            collision.gameObject.GetComponent<IHitByShekra>().Blink(1f);
            if (!transform.parent.GetComponent<ShekraBehaviour>().hitTargetList.Contains(collision.gameObject.GetInstanceID())) {
                transform.parent.GetComponent<ShekraBehaviour>().hitTargetList.Add(collision.gameObject.GetInstanceID());
            }
        }
    }
}
