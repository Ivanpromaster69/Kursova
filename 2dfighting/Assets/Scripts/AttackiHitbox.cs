using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AttackHitbox : MonoBehaviour
{
    [Header("Attack Settings")]
    public float damage = 10f;
    public float hitCooldown = 0.5f;
    public float knockbackForce = 5f;
    public float damageDelay = 0.2f;

    [Header("Sound")]
    public AudioSource audioSource;
    public AudioClip attackSound;

    private Dictionary<GameObject, float> hitTimestamps = new Dictionary<GameObject, float>();
    private bool canDamage = false;
    private bool isActive = false;
    private Coroutine damageDelayCoroutine;

    public void ActivateHitbox()
    {
        isActive = true;
        gameObject.SetActive(true);
        hitTimestamps.Clear();
        damageDelayCoroutine = StartCoroutine(EnableDamageAfterDelay());
        if (audioSource != null && attackSound != null)
        {
            audioSource.PlayOneShot(attackSound);
        }
    }

    public void DeactivateHitbox()
    {
        isActive = false;
        canDamage = false;
        gameObject.SetActive(false);
        if (damageDelayCoroutine != null)
        {
            StopCoroutine(damageDelayCoroutine);
            damageDelayCoroutine = null;
        }
    }

    private IEnumerator EnableDamageAfterDelay()
    {
        yield return new WaitForSeconds(damageDelay);
        canDamage = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isActive || !canDamage) return;
        if (!other.CompareTag("Enemy")) return;
        GameObject target = other.gameObject;
        if (!hitTimestamps.ContainsKey(target) || Time.time - hitTimestamps[target] >= hitCooldown)
        {
            EnemyController enemy = target.GetComponent<EnemyController>();
            if (enemy != null)
            {
                Vector2 direction = (target.transform.position - transform.position).normalized;
                enemy.TakeDamage(damage);
                enemy.ApplyKnockback(direction, knockbackForce);
                hitTimestamps[target] = Time.time;
            }
        }
    }
}
