using UnityEngine;

public class Projectile : MonoBehaviour {
    void Update() {
        // destroy it when it goes too far enough to be inconsequential
        if (transform.position.magnitude > 250f)
            Destroy(gameObject);
    }

    void OnTriggerEnter2D (Collider2D collider) {
        if (collider.gameObject.CompareTag("Wall"))
            Destroy(gameObject);
        // projectile calls the enemy's Hit function
        else if (collider.gameObject.CompareTag("Enemy")) {
            collider.gameObject.GetComponent<Enemy>().Hit(collider.transform.position - transform.position);
            Destroy(gameObject);
        }
    }
}
