using UnityEngine;
using System.Collections;

public class EnemyAttackController : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackCooldown = 2f;
    private float lastAttackTime = -999f;
    public float attackRange = 2f;

    [Header("Hitboxes")]
    public GameObject punchHitbox;
    public GameObject kickHitbox;
    public GameObject airKickHitbox;
    public GameObject airKickDirectionalHitbox;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    private bool isGrounded;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip attackSound;

    [Header("References")]
    private Animator animator;
    private bool isAttacking = false;
    private bool isDead = false;
    
    [Header("Player")]
    public Transform player;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        DisableAllHitboxes();
    }

    void Update()
    {
        if (isDead) return;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        if (!isAttacking && Time.time - lastAttackTime >= attackCooldown)
        {
            if (player != null)
            {
                float distanceToPlayer = Vector2.Distance(transform.position, player.position);
                if (distanceToPlayer <= attackRange)
                {
                    ChooseAttack();
                }
            }
        }
    }

    void ChooseAttack()
    {
        if (isDead) return;
        isAttacking = true;
        lastAttackTime = Time.time;
        int attackType = Random.Range(0, isGrounded ? 2 : 4);
        if (isGrounded)
        {
            if (attackType == 0)
            {
                animator.SetTrigger("punch");
                StartCoroutine(DelayedHitbox(punchHitbox, 0.2f, 0.3f));
            }
            else
            {
                animator.SetTrigger("kick");
                StartCoroutine(DelayedHitbox(kickHitbox, 0.25f, 0.3f));
            }
        }
        else
        {
            if (attackType == 2)
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

        PlaySound(attackSound);

        Invoke(nameof(EndAttack), 0.6f);
    }

    IEnumerator DelayedHitbox(GameObject hitbox, float delay, float activeDuration)
    {
        yield return new WaitForSeconds(delay);
        if (isDead) yield break;
        hitbox.SetActive(true);
        yield return new WaitForSeconds(activeDuration);
        hitbox.SetActive(false);
    }

    void EndAttack()
    {
        isAttacking = false;
        DisableAllHitboxes();
    }

    void DisableAllHitboxes()
    {
        punchHitbox.SetActive(false);
        kickHitbox.SetActive(false);
        airKickHitbox.SetActive(false);
        airKickDirectionalHitbox.SetActive(false);
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;
        isAttacking = false;
        animator.SetTrigger("isDead");
        DisableAllHitboxes();
        CancelInvoke();
        StopAllCoroutines();
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }
}
