using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class SceneC1 : MonoBehaviour {
    public GameObject laser;
    public float targetY;
    public List<AudioClip> music;
    public static SceneC1 instance;
    public GameObject transition;

    bool laserRetreat = false;
    bool laserDestroyed = false;
    bool started = false;

    void Start() {
        Player.instance.gameObject.SetActive(false);
        StartCoroutine(DelayStart());
        instance = this;
    }

    void Update() {
        if (laserDestroyed || !started) return;

        // brings the laser down to the surface and then back up
        Vector3 pos = laser.transform.position;
        pos.y = Mathf.Lerp(pos.y, laserRetreat ? 80f : targetY, Time.deltaTime * 8f);
        laser.transform.position = pos;
    }

    IEnumerator DelayStart() {
        // start the scene by showing the player being summoned in through the laser beam
        yield return new WaitForSeconds(1f);

        started = true;
        AudioSource laserSound = laser.GetComponent<AudioSource>();
        laserSound.time = 8f;
        laserSound.volume = 0.75f;
        laserSound.Play();
        yield return new WaitForSeconds(2f);

        laserRetreat = true;
        Player.instance.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.3f);

        laserSound.Stop();
        laserDestroyed = true;
        Destroy(laser);
        yield return new WaitForSeconds(2.5f);

        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.Play();
    }

    public void CollectedFlintNSteel() {
        StartCoroutine(Transition());
    }

    IEnumerator Transition() {
        CanvasGroup cg = transition.GetComponent<CanvasGroup>();

        float time = 0f;
        while (time < 1f) {
            time += Time.deltaTime;
            cg.alpha = Mathf.Lerp(0f, 1f, time);
            yield return null;
        }

        cg.alpha = 1f;
        Player.instance.transform.position = new Vector3(31f, 40f, 1f);
        yield return new WaitForSeconds(1.5f);


        time = 0f;
        while (time < 1f) {
            time += Time.deltaTime;
            cg.alpha = Mathf.Lerp(1f, 0f, time);
            yield return null;
        }

        cg.alpha = 0f;
    }
}
