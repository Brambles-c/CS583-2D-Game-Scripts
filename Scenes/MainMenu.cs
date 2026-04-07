using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MainMenu : MonoBehaviour {
    public List<GameObject> menuItems = new();
    public float fadeTime = 1f;
    public Transform title;
    public RectTransform laser1;
    public GameObject laser2;
    public Vector2 laserTargetPos;
    public GameObject moon;
    public RectTransform background;
    public RectTransform mare;
    public Vector2 mareTarget;

    Vector2 titleStartPos;
    bool showLaser = false;
    bool showMare = false;

    private void Start() {
        titleStartPos = title.position;
    }

    private void Update() {
        if (!showLaser)
            // animating title
            title.position = titleStartPos + new Vector2(Mathf.Sin(Time.time) * 10f, Mathf.Cos(Time.time) * 5f);
        else {
            if (!showMare) // first laser animation
                laser1.anchoredPosition = Vector2.MoveTowards(
                    laser1.anchoredPosition,
                    laserTargetPos,
                    Time.deltaTime * 2000f
                );
            else {
                // mare animation during second laser animation
                mare.anchoredPosition = Vector2.MoveTowards(
                    mare.anchoredPosition,
                    mareTarget,
                    Time.deltaTime * 550f
                );

                mare.Rotate(0f, 0f, Time.deltaTime * 80f);
            }
        }
    }

    void Clicked() {
        // menu items fading out one by one
        for (int i = 0; i < menuItems.Count; i++)
            StartCoroutine(FadeOut(menuItems[i], i));
    }

    IEnumerator FadeOut(GameObject obj, int i) {
        // make the menu items fade out
        CanvasGroup cg = obj.GetComponent<CanvasGroup>();

        yield return new WaitForSeconds(i * 0.35f);
        float startAlpha = cg.alpha;
        float time = 0f;

        while (time < fadeTime) {
            time += Time.deltaTime;
            cg.alpha = Mathf.Lerp(startAlpha, 0f, time / fadeTime);
            yield return null;
        }

        cg.alpha = 0f;
        obj.SetActive(false);

        if (i == menuItems.Count - 1) {
            // animation steps for each part of the mini cutscene after the last item fades

            yield return new WaitForSeconds(2.5f);

            showLaser = true;
            AudioSource laserSound = GetComponent<AudioSource>();
            laserSound.volume = 0.35f;
            laserSound.Play();
            yield return new WaitForSeconds(4f);

            moon.SetActive(false);
            laser1.gameObject.SetActive(false);
            laser2.SetActive(true);
            laserSound.volume = 1f;
            background.localScale = new Vector3(1f, -1f, 1f);
            yield return new WaitForSeconds(2f);

            showMare = true;
            yield return new WaitForSeconds(5f);

            SceneManager.LoadScene("1 C");
        }
    }
}
