using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 12f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Combat")]
    public float maxHealth = 100f;
    public float currentHealth;
    public float attackCooldown = 0.7f;
    public HealthBar healthBar;

    [Header("Collision Check")]
    public LayerMask obstacleLayers;
    public float obstacleCheckDistance = 0.1f;

    [Header("Hitboxes")]
    public GameObject punchHitbox;
    public GameObject kickHitbox;
    public GameObject airKickHitbox;
    public GameObject airKickDirectionalHitbox;

    [Header("Hit Settings")]
    public float hitResetTime = 0.3f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip jumpSound;
    public AudioClip hitSound;

    private Rigidbody2D rb;
    private bool isGrounded;
    private float lastAttackTime = -999f;
    private float hitTimer;
    [HideInInspector]
    public Animator animator;
    private SpriteRenderer spriteRenderer;
    private bool isFacingRight = true;
    private bool isAttacking = false;
    [HideInInspector]
    public bool isDead = false;
    private bool isHit = false;
    private bool canBeDamaged = true;
    private Collider2D playerCollider;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        playerCollider = GetComponent<Collider2D>();
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        DisableAllHitboxes();
    }

    void Update()
    {
        if (isDead) return;
        if (isHit)
        {
            hitTimer -= Time.deltaTime;
            if (hitTimer <= 0f)
            {
                animator.SetBool("isHit", false);
                isHit = false;
                canBeDamaged = true;
            }
            return;
        }
        HandleMovement();
        HandleCombat();
    }

    void HandleMovement()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        animator.SetBool("isJumping", !isGrounded);
        float moveX = 0f;
        if (Input.GetKey(KeyCode.A) && !IsObstacleInDirection(Vector2.left))
        {
            moveX = -1f;
            if (isGrounded && isFacingRight)
                Flip();
        }
        else if (Input.GetKey(KeyCode.D) && !IsObstacleInDirection(Vector2.right))
        {
            moveX = 1f;
            if (isGrounded && !isFacingRight)
                Flip();
        }
        if (isAttacking)
        {
            moveX = 0f;
        }
        rb.linearVelocity = new Vector2(moveX * moveSpeed, rb.linearVelocity.y);
        animator.SetBool("isRunning", moveX != 0);
        if (Input.GetKeyDown(KeyCode.W) && isGrounded && !isAttacking)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            PlaySound(jumpSound);
        }
        animator.SetBool("isFacingRight", isFacingRight);
    }

    bool IsObstacleInDirection(Vector2 direction)
    {
        Vector2 origin = (Vector2)transform.position + Vector2.up * 0.5f;
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, obstacleCheckDistance, obstacleLayers);
        return hit.collider != null;
    }

    void HandleCombat()
    {
        if (isAttacking || Time.time - lastAttackTime < attackCooldown || isDead)
            return;
        if (Input.GetMouseButtonDown(0))
        {
            isAttacking = true;
            animator.SetTrigger("punch");
            PlaySound(hitSound);
            StartCoroutine(DelayedHitbox(punchHitbox, 0.2f, 0.3f));
            Invoke(nameof(EndAttack), 0.6f);
        }
        if (Input.GetMouseButtonDown(1))
        {
            isAttacking = true;
            PlaySound(hitSound);
            if (isGrounded)
            {
                animator.SetTrigger("kick");
                StartCoroutine(DelayedHitbox(kickHitbox, 0.25f, 0.3f));
            }
            else
            {
                if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
                {
                    animator.SetTrigger("airKick");
                    StartCoroutine(DelayedHitbox(airKickHitbox, 0.15f, 0.3f));
                }
                else
                {
                    animator.SetTrigger("airKickDirectional");
                    StartCoroutine(DelayedHitbox(airKickDirectionalHitbox, 0.15f, 0.3f));
                }
            }
            StartCoroutine(EndAttackWithDelay(0.6f));
        }
    }

    IEnumerator EndAttackWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        isAttacking = false;
        lastAttackTime = Time.time;
        DisableAllHitboxes();
    }

    public void ApplyKnockback(Vector2 direction, float force)
    {
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(direction * force, ForceMode2D.Impulse);
        animator.SetBool("isHit", true);
        isHit = true;
        hitTimer = hitResetTime;
        canBeDamaged = false;
        isAttacking = false;
        animator.ResetTrigger("punch");
        animator.ResetTrigger("kick");
        animator.ResetTrigger("airKick");
        animator.ResetTrigger("airKickDirectional");
        DisableAllHitboxes();
    }

    IEnumerator DelayedHitbox(GameObject hitbox, float delay, float activeDuration)
    {
        yield return new WaitForSeconds(delay);
        var hitboxScript = hitbox.GetComponent<AttackHitbox>();
        if (hitboxScript != null)
            hitboxScript.ActivateHitbox();
        yield return new WaitForSeconds(activeDuration);
        if (hitboxScript != null)
            hitboxScript.DeactivateHitbox();
    }

    void EndAttack()
    {
        isAttacking = false;
        lastAttackTime = Time.time;
        DisableAllHitboxes();
    }

    void DisableAllHitboxes()
    {
        punchHitbox.SetActive(false);
        kickHitbox.SetActive(false);
        airKickHitbox.SetActive(false);
        airKickDirectionalHitbox.SetActive(false);
    }

    public void TakeDamage(float amount)
    {
        if (isDead || !canBeDamaged) return;
        currentHealth -= amount;
        if (currentHealth < 0) currentHealth = 0;
        healthBar.SetHealth(currentHealth, maxHealth);
        ApplyKnockback(Vector2.zero, 0f);
        if (currentHealth <= 0)
        {
            animator.SetTrigger("isDead");
            isHit = true;
            isDead = true;
        }
    }

    void UpdateHitboxFacing()
    {
        Vector3 newScale = Vector3.one;
        newScale.x = isFacingRight ? 1 : -1;
        punchHitbox.transform.localScale = newScale;
        kickHitbox.transform.localScale = newScale;
        airKickHitbox.transform.localScale = newScale;
        airKickDirectionalHitbox.transform.localScale = newScale;
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        spriteRenderer.flipX = !spriteRenderer.flipX;
        UpdateHitboxFacing();
    }

    public void InitializePlayer()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    public void ResetAnimations()
    {
        if (animator == null)
        {
            Debug.LogWarning("Animator is null!");
            return;
        }

        animator.ResetTrigger("isDead");
        animator.SetBool("isHit", false);
        animator.Play("IdleAnimation");
    }

    void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }
}
