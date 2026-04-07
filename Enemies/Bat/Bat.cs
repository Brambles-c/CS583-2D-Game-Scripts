using System.Collections.Generic;
using UnityEngine;

public class Bat : Enemy {
    public List<AudioClip> idleSounds, hurtSounds;
    public AudioClip deathSound;
    public float steeringForce = 25f;
    public float maxSpeed = 8f;
    public float minSpeed = 2.5f;
    public float attackDistance = 1.5f;
    public float attackCooldownTime = 0.5f;
    
    Vector3 targetOffset = new Vector3(0, 0.5f, 0);
    GameObject attackHitbox;
    AudioSource audioSource;
    bool awake = false;
    float attackCooldown;
    float squeekCooldown;

    Bat() : base(3) {}

    private void Awake() {
        attackCooldown = attackCooldownTime;
        attackHitbox = transform.Find("Attack").gameObject;
        audioSource = GetComponent<AudioSource>();
        squeekCooldown = Random.Range(3f, 6f);
    }

    private void FixedUpdate() {
        if (!awake) return;
        // chasing the player is essentially just accelerating
        // towards them instead of setting velocity in their direction
        Vector2 distance = (Player.instance.transform.position + targetOffset - transform.position);
        transform.localScale = new Vector3(distance.x > 0 ? -1 : 1, 1, 1);

        rb.AddForce(distance.normalized * steeringForce);

        if (rb.linearVelocity.magnitude > maxSpeed)
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        else if (rb.linearVelocity.magnitude < minSpeed)
            rb.linearVelocity = rb.linearVelocity.normalized * minSpeed;

        // attack when close enough and cooldown is ready
        if (attackCooldown > 0f)
            attackCooldown -= Time.fixedDeltaTime;
        else if (distance.magnitude < attackDistance) {
            animator.SetInteger("attacking", Random.Range(1, 3));
            attackCooldown = attackCooldownTime;
        }

        // all references to minecraft in sound or assets only happened because of and started here
        // because i saw this again https://www.youtube.com/watch?v=7qkcybWqDPw
        if (squeekCooldown > 0f)
            squeekCooldown -= Time.fixedDeltaTime;
        else {
            audioSource.PlayOneShot(idleSounds[Random.Range(0, idleSounds.Count)]);
            squeekCooldown = Random.Range(3f, 6f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        // wake up when player is near
        if (collision.gameObject.CompareTag("Player"))
            animator.SetTrigger("wake");
    }

    // called in the animator after wake up animation finishes
    public void Awoken() {
        awake = true;
        Vector2 direction = (Player.instance.transform.position - transform.position).normalized;
        audioSource.PlayOneShot(idleSounds[Random.Range(0, idleSounds.Count)]);
        // take off orthogonally from players direction so that it doesn't just start flying straight at them 
        rb.linearVelocity = new Vector2(-direction.y, direction.x) * 5f;
    }

    // called in the animator during attack animation
    public void Attack() {
        animator.SetInteger("attacking", 0);
        attackHitbox.SetActive(true);
        audioSource.PlayOneShot(idleSounds[Random.Range(0, idleSounds.Count)]);
    }

    // called in the animator at the end of attack animation
    public void EndAttack() {
        attackCooldown = attackCooldownTime;
        attackHitbox.SetActive(false);
    }

    // handle getting hit like normal but also play a hurt sound
    public override void Hit(Vector2 direction) {
        audioSource.PlayOneShot(hurtSounds[Random.Range(0, hurtSounds.Count)]);
        base.Hit(direction);
    }

    public override void End() {
        audioSource.PlayOneShot(deathSound);
    }
}
