using System.Collections.Generic;
using UnityEngine;

public class Wolf : Enemy {
    public float speed = 5f;
    public List<AudioClip> attackSounds, growlSounds;
    public AudioClip deathSound;
    
    AudioSource audioSource;
    GameObject attackHitbox;
    bool isChasing = false;
    float growlCooldown;


    Wolf() : base(5) {}

    new void Start() {
        base.Start();
        attackHitbox = transform.Find("Attack").gameObject;
        audioSource = GetComponent<AudioSource>();
        growlCooldown = Random.Range(3f, 6f);
    }

    private void Update() {
        // attack the player when theyre near enough and wolf isnt dead
        if (
            hp > 0 && isChasing &&
            Vector2.Distance(transform.position, Player.instance.transform.position) < 1.2f
        ) {
            animator.SetBool("attacking", true);
            isChasing = false;
        }
    }

    void FixedUpdate() {
        if (hp <= 0 || !isChasing) return;
        
        transform.localScale = new Vector3(Player.instance.transform.position.x < transform.position.x ? -1 : 1, 1, 1);

        // chase player by setting velocity in their direction
        rb.linearVelocity = (
            Player.instance.transform.position - transform.position
        ).normalized * speed;

        if (growlCooldown > 0f)
            growlCooldown -= Time.fixedDeltaTime;
        else {
            audioSource.PlayOneShot(growlSounds[Random.Range(0, growlSounds.Count)]);
            growlCooldown = Random.Range(3f, 6f);
        }
    }

    // start chasing the player when they get near enough
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.transform == Player.instance.transform) {
            isChasing = true;
            animator.SetBool("chasing", true);
            audioSource.PlayOneShot(growlSounds[Random.Range(0, growlSounds.Count)]);
        }
    }

    // called from attack animation to activate the hitbox and play attack sound
    public void EnableAttackBox() {
        attackHitbox.SetActive(true);
        audioSource.PlayOneShot(attackSounds[Random.Range(0, attackSounds.Count)]);
    }

    // called after attack animation to disable the hitbox and start chasing again
    public void EndAttack() {
        attackHitbox.SetActive(false);
        animator.SetBool("attacking", false);
        isChasing = true;
    }

    // called in the animator after death animation finishes to disable colliders and play death sound
    public override void End() {
        attackHitbox.SetActive(false);

        foreach (Collider2D collider in GetComponents<Collider2D>())
            collider.enabled = false;

        audioSource.PlayOneShot(deathSound);
    }
}
