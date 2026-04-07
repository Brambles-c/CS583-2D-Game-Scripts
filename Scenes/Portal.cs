using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour {
    public bool destinationPortal = true;

    AudioSource primary;
    AudioSource secondary;

    private void Start() {
        AudioSource[] audioSources = GetComponents<AudioSource>();        
        if (destinationPortal) {
            audioSources[0].Play(); // ambient
            audioSources[1].Play(); // teleported
        }
    }

    // go to the next level when player enters lit portal
    private void OnTriggerEnter2D(Collider2D collision) {
        if (!destinationPortal && collision.gameObject.CompareTag("Player") && Player.instance.hasFlintAndSteel)
            SceneManager.LoadScene("2 E");
    }
}
