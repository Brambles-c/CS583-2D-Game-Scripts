using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class HUD : MonoBehaviour {
    public GameObject snowpityPrefab;
    public Transform snowpityAnchor;
    public List<RawImage> snowpityBar = new();
    public Texture2D snowpityIcon;
    public Texture2D nopityIcon;
    public TextMeshProUGUI manaText;

    public void Init(int snowpity) {
        // For adding snowpity to the hp bar
        for (int i = 0; i < snowpity; i++) {
            GameObject snowpityObject = Instantiate(snowpityPrefab, snowpityAnchor.transform);
            snowpityObject.transform.localPosition = new Vector3(i * 115f, 0, 0);
            RawImage snowpityIcon = snowpityObject.GetComponent<RawImage>();
            snowpityIcon.rectTransform.sizeDelta = new Vector2(100, 125);
            snowpityBar.Add(snowpityIcon);
        }
    }

    public void UpdateMana(int newVal, Color color) {
        manaText.text = newVal.ToString();
        manaText.color = color;
    }

    public void Hit(int newSnowpity) {
        snowpityBar[newSnowpity].texture = nopityIcon;
        snowpityBar[newSnowpity].color = new Color(0.5f, 0.5f, 0.5f, 1f);
        snowpityBar[newSnowpity].rectTransform.sizeDelta = new Vector2(125, 100);
    }

    // displaying the right Rainbowshine icons to indicate remaining snowpity
    public void UpdateSnowpity(int newVal) {
        for (int i = 0; i < snowpityBar.Count; i++) {
            if (i < newVal) {
                snowpityBar[i].texture = snowpityIcon;
                snowpityBar[i].color = Color.white;
                snowpityBar[i].rectTransform.sizeDelta = new Vector2(100, 125);
            }
            else {
                snowpityBar[i].texture = nopityIcon;
                snowpityBar[i].color = new Color(0.5f, 0.5f, 0.5f, 1f);
                snowpityBar[i].rectTransform.sizeDelta = new Vector2(125, 100);
            }
        }
    }
}
