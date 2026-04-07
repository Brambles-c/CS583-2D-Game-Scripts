using UnityEngine;

public class Loot : MonoBehaviour {
    public enum LootType {
        Mana,
        Snowpity
    }

    public AudioClip pickupSound;
    public LootType type;
    public int count = 1; // should be overridden with the loot entry count in the enemy's loot table
    Vector3 initialPos;

    void Start() {
        initialPos = transform.position;
    }

    void Update() {
        // bobs up and down to look nice
        transform.position = initialPos + new Vector3(0, Mathf.Sin(Time.time * 2f) * 0.25f, 0);
    }

    protected void OnTriggerEnter2D(Collider2D collision) {
        if (!collision.transform.CompareTag("Player")) return;

        // handle collecting the drop based on its type
        switch (type) {
            case LootType.Mana:
                Player.instance.AddMana(count);
                break;
            case LootType.Snowpity:
                Player.instance.AddSnowpity(count);
                break;
        }

        AudioSource.PlayClipAtPoint(pickupSound, transform.position, 1.75f);
        Destroy(gameObject);
    }
}