using UnityEngine;

public class FilntNSteel : Loot {
    new void OnTriggerEnter2D(Collider2D collision) {
        if (!collision.transform.CompareTag("Player")) return;
        
        AudioSource.PlayClipAtPoint(pickupSound, transform.position, 1.75f);
        Player.instance.hasFlintAndSteel = true;
        SceneC1.instance.CollectedFlintNSteel(); // transition teleports to portal
        Destroy(gameObject);
    }
}
