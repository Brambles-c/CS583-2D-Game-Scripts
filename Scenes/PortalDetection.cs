using UnityEngine;

public class PortalDetection : MonoBehaviour {
    public bool lit = true;

    // light the portal when player is near and has flint and steel
    private void OnTriggerEnter2D(Collider2D collision) {
        if (lit || !collision.gameObject.CompareTag("Player") || !Player.instance.hasFlintAndSteel) return;
        lit = true;

        GetComponent<AudioSource>().Play();

        AudioSource portalAmbience = transform.parent.GetComponents<AudioSource>()[0];
        portalAmbience.Play();

        transform.parent.GetComponent<Animator>().enabled = true;
    }
}
