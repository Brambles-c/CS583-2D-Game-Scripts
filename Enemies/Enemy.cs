using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LootEntry {
    public GameObject item;
    public int count;
}

public abstract class Enemy : MonoBehaviour {
    protected SpriteRenderer sprite;
    protected int hp;
    protected Rigidbody2D rb;
    protected Animator animator;
    public List<LootEntry> loot;

    public Enemy(int hp) {
        this.hp = hp;
    }

    protected void Start() {
        sprite = GetComponent<SpriteRenderer>();
        transform.localScale = new Vector3(Random.value > 0.5f ? -1 : 1, 1, 1);
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void LateUpdate() {
        sprite.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100);
    }

    // handle taking damage to determine when to die
    public virtual void Hit(Vector2 direction) {
        --hp;

        rb.AddForce(direction * 5f, ForceMode2D.Impulse);

        if (hp != 0) return;

        animator.SetTrigger("death");
        End();
    }

    // called when enemy dies just before destroying the game object
    public abstract void End();

    // drop loot on death and destroy the game object
    public void Die() {
        if (Random.value > 0.2f) {
            LootEntry randomLoot = loot[Random.Range(0, loot.Count)];

            GameObject drop = Instantiate(randomLoot.item, transform.position + new Vector3(0f, 0.5f, 0f), Quaternion.identity);
            drop.GetComponent<Loot>().count = randomLoot.count;
        }

        Destroy(gameObject);
    }
}
