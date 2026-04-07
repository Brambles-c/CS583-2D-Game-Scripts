using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour { // Mares, I love them
    public HUD hud;
    public GameObject basicProjectile;
    public Transform horn;
    public bool controlDisabled = false;
    public float move_speed = 1.8f;
    public float knockbackForce = 5f;
    public bool hasFlintAndSteel = false;
    public AudioClip hurtSound, shootSound, lowShootSound, depletedShootSound;

    public static Player instance { get; private set; }

    AudioSource audioSource;
    InputSystemActions input;
    SpriteRenderer sprite;
    Animator animator;
    Rigidbody2D rb;
    List<GameObject> snowpityBar = new();
    Vector2 movement;
    Vector2 shootDirection;
    int snowpity = 5;
    int mana = 20;
    float invincibilityTime = 0f;
    float magicCooldown = 0f;
    float manaRechargeCooldown = 2.5f;

    void Awake() {
        instance = this;
        rb = GetComponent<Rigidbody2D>();
        input = new InputSystemActions();
        sprite = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        hud.Init(snowpity);
        
        // Setup move and shoot actions for player inputs
        input.Player.Move.performed += ctx => movement = ctx.ReadValue<Vector2>();
        input.Player.Move.canceled += ctx => movement = Vector2.zero;

        input.Player.Shoot.performed += ctx => {
            if (!controlDisabled) {
                Vector2 direction = ctx.ReadValue<Vector2>();
                if ((direction.x != 0 && direction.y != 0))
                    return;

                shootDirection = direction;
            }
        };

        input.Player.Shoot.canceled += ctx => {
            shootDirection = Vector2.zero;
        };

        animator = GetComponent<Animator>();
        input.Enable();
    }

    void Update() {
        Vector2 checkedMovement = controlDisabled ? Vector2.zero : movement;

        animator.SetBool("walking", checkedMovement != Vector2.zero);

        // X movement flipping
        if (checkedMovement.x != 0)
            transform.localScale = new Vector3(checkedMovement.x < 0 ? -1 : 1, 1, 1);

        // Invincibility animation
        if (invincibilityTime > 0f) {
            invincibilityTime -= Time.deltaTime;
            float colorVal = 0.65f + Mathf.PingPong(Time.time * 2.8f, 0.35f);
            sprite.color = new Color(colorVal, colorVal, colorVal, 1f);
        }
        else
            sprite.color = Color.white;

        // Shooting while arrow key held
        if (shootDirection != Vector2.zero && magicCooldown <= 0f && mana > 0) {
            --mana;
            GameObject projectile = Instantiate(basicProjectile, horn.position, Quaternion.identity);

            projectile.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg);
            projectile.GetComponent<Rigidbody2D>().linearVelocity = shootDirection * 12f;

            magicCooldown = 0.35f;
            manaRechargeCooldown = 2.5f;

            // Colors and sounds to indicate mana levels
            hud.UpdateMana(mana, mana <= 3 ? Color.orange: Color.white);
            audioSource.PlayOneShot(
                mana > 3 ? shootSound :
                mana > 0 ? lowShootSound :
                depletedShootSound
            );
        }
        // Doing mana recharge only when mana < 3
        else if (mana < 3 && manaRechargeCooldown <= 0f) {
            AddMana(1);
            manaRechargeCooldown = 2.5f;
        }
        // Mana recharge cooldown reduction
        else if (shootDirection == Vector2.zero || mana == 0) {
            manaRechargeCooldown = Mathf.Max(0f, manaRechargeCooldown - Time.deltaTime);
        }

        // Shot cooldown
        magicCooldown = Mathf.Max(0f, magicCooldown - Time.deltaTime);
    }

    // Handling colliding with a damage source
    private void OnTriggerEnter2D(Collider2D collision) {
        if (invincibilityTime <= 0f && collision.transform.CompareTag("Hazard"))
            TakeDamage(collision.transform);
    }

    private void LateUpdate() {
        sprite.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100);
    }

    void FixedUpdate() {
        if (!controlDisabled)
            rb.linearVelocity = movement.normalized * move_speed;
    }

    public void TakeDamage(Transform source) {
        animator.SetBool("walking", false);
        invincibilityTime = 1.5f;
        Vector2 direction = (transform.position - source.position).normalized;

        if (--snowpity == 0) { // restart when no more snowpity
            SceneManager.LoadScene("Start Menu");
            return;
        }

        hud.Hit(snowpity);
        audioSource.PlayOneShot(hurtSound);

        // Knockback and hitstop to emphasize force behind impact
        StartCoroutine(KnockBack(direction));
        StartCoroutine(HitStop());
    }

    IEnumerator HitStop() {
        // almost freezes the game for a tiny moment when hit
        Time.timeScale = 0.15f;
        yield return new WaitForSecondsRealtime(0.22f);
        Time.timeScale = 1f;
    }

    IEnumerator KnockBack(Vector2 direction) {
        controlDisabled = true;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(direction * knockbackForce, ForceMode2D.Impulse);

        yield return new WaitForSecondsRealtime(0.22f);
        controlDisabled = false;
    }

    public void AddMana(int count) {
        mana = Mathf.Min(20, mana + count);

        hud.UpdateMana(
            mana,
            mana == 20 ? Color.yellow :
            mana > 3 ? Color.white :
            Color.orange
        );
    }

    public void AddSnowpity(int count) {
        snowpity = Mathf.Min(5, snowpity + count);
        hud.UpdateSnowpity(snowpity);
    }
}
