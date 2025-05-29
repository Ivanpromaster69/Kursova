using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Stats")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("Components")]
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    [Header("References")]
    public Transform hitboxesParent;
    public Transform groundCheck;

    [Header("Chasing Settings")]
    public Transform player;
    public float chaseSpeed = 3.5f;
    public float slowSpeed = 1.5f;
    public float slowDownRadius = 4f;
    public float stopRadius = 1f;

    [Header("Jump Settings")]
    public float jumpForce = 7f;
    public float jumpCooldown = 3f;
    private float lastJumpTime;

    [Header("Hit Effect")]
    public float hitResetTime = 0.8f;
    private float hitTimer;

    [Header("UI")]
    public HealthBar healthBar;

    [Header("Ground Check")]
    public LayerMask groundLayer;
    public float groundCheckRadius = 0.2f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip jumpSound;

    private bool isGrounded;
    [HideInInspector]
    public bool isDead = false;
    private bool isHit = false;
    private bool movingRight = true;
    private Vector3 originalHitboxScale;
    private bool canBeDamaged = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (hitboxesParent != null)
            originalHitboxScale = hitboxesParent.localScale;
        else
            Debug.LogWarning("hitboxesParent not in EnemyController.");
    }

    void Start()
    {
        currentHealth = maxHealth;
        if (healthBar != null)
            healthBar.SetMaxHealth(maxHealth);
        lastJumpTime = -jumpCooldown;
    }

    void Update()
    {
        if (isDead) return;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        animator.SetBool("isJumping", !isGrounded);
        if (isHit)
        {
            hitTimer -= Time.deltaTime;
            if (hitTimer <= 0f)
            {
                isHit = false;
                animator.SetBool("isHit", false);
                canBeDamaged = true;
            }
            return;
        }
        if (player == null) return;
        HandleMovement();
        HandleJump();
    }

    void HandleMovement()
    {
        if (isHit) return;
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer > slowDownRadius)
        {
            MoveTowardsPlayer(chaseSpeed);
        }
        else if (distanceToPlayer > stopRadius)
        {
            MoveTowardsPlayer(slowSpeed);
        }
        else
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            animator.SetBool("isRunning", false);
        }
    }

    void MoveTowardsPlayer(float speed)
    {
        animator.SetBool("isRunning", true);
        float direction = player.position.x > transform.position.x ? 1f : -1f;
        rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);
        if ((direction > 0 && !movingRight) || (direction < 0 && movingRight))
        {
            Flip();
        }
    }

    void HandleJump()
    {
        if (isGrounded && Time.time - lastJumpTime >= jumpCooldown)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            PlaySound(jumpSound);
            lastJumpTime = Time.time;
        }
    }

    void Flip()
    {
        movingRight = !movingRight;
        if (spriteRenderer != null)
            spriteRenderer.flipX = movingRight;
        if (hitboxesParent != null)
        {
            Vector3 newScale = originalHitboxScale;
            newScale.x *= movingRight ? 1f : -1f;
            hitboxesParent.localScale = newScale;
        }
    }

    public void TakeDamage(float amount)
    {
        if (isDead || !canBeDamaged) return;
        currentHealth -= amount;
        currentHealth = Mathf.Max(0, currentHealth);
        if (healthBar != null)
            healthBar.SetHealth(currentHealth, maxHealth);
        animator.SetBool("isHit", true);
        isHit = true;
        hitTimer = hitResetTime;
        canBeDamaged = false;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void ApplyKnockback(Vector2 direction, float force)
    {
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(direction * force, ForceMode2D.Impulse);
        animator.SetBool("isHit", true);
        isHit = true;
        hitTimer = hitResetTime;
        canBeDamaged = false;
    }

    void Die()
    {
        isDead = true;
        animator.SetTrigger("isDead");
        rb.linearVelocity = Vector2.zero;
        EnemyAttackController attackController = GetComponent<EnemyAttackController>();
        if (attackController != null)
            attackController.enabled = false;
        this.enabled = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerAttack"))
        {
            AttackHitbox attack = other.GetComponent<AttackHitbox>();
            if (attack != null)
            {
                if (!canBeDamaged) return;
                TakeDamage(attack.damage);
                Vector2 direction = (transform.position - attack.transform.position).normalized;
                ApplyKnockback(direction, attack.knockbackForce);
            }
        }
    }

    public void ResetEnemy()
    {
        currentHealth = maxHealth;
        isDead = false;
        this.enabled = true;
        EnemyAttackController attackController = GetComponent<EnemyAttackController>();
        if (attackController != null)
            attackController.enabled = true;
        if (animator != null)
        {
            animator.ResetTrigger("isDead");
            animator.Play("IdleAnimation", 0, 0f);
            animator.SetBool("isHit", false);
            animator.SetBool("isRunning", false);
        }

        canBeDamaged = true;
        isHit = false;
        hitTimer = 0f;
        movingRight = true;
        if (spriteRenderer != null)
            spriteRenderer.flipX = false;
        else
            Debug.LogWarning("spriteRenderer not found in ResetEnemy!");
        if (hitboxesParent != null)
            hitboxesParent.localScale = originalHitboxScale;
        else
            Debug.LogWarning("hitboxesParent not found in ResetEnemy!");
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }
}
