using System;
using UnityEngine;

public class BombToLaunch : MonoBehaviour {
    [SerializeField] private GameObject bomb, shadow;

    public void UpdateSpriteRenderer(Int16 sortingOrder, string sortingLayerName = "") {
        if (sortingLayerName != "") bomb.GetComponent<SpriteRenderer>().sortingLayerName = sortingLayerName;
        bomb.GetComponent<SpriteRenderer>().sortingOrder = sortingOrder;
    }

    public void AddForceToRigidbody(Vector2 direction, float thrust = 1) {
        shadow.GetComponent<Rigidbody2D>().AddForce(direction * thrust, ForceMode2D.Impulse);
    }

    // Spread methods to BombController
    public void SpreadStartCountDown(float delay = 3f) {
        bomb.GetComponent<BombController>().StartCountDown(delay);
    }

    public void SpreadEnd() {
        bomb.GetComponent<BombController>().End();
    }

    public void SpreadHandleBombSmoke() {
        bomb.GetComponent<BombController>().HandleBombSmoke();
    }

    public void SpreadShadowUpdateRigidBody(float mass, float linearDrag) {
        shadow.GetComponent<Shadow>().UpdateRigidBody(mass, linearDrag);
    }
}
