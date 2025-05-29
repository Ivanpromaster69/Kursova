using UnityEngine;
using System.Collections.Generic;

public class AttackHitboxEnemy : MonoBehaviour
{
    public float damage = 10f;
    public float hitCooldown = 3f;
    public float knockbackForce = 5f;
    private Dictionary<GameObject, float> hitTimestamps = new Dictionary<GameObject, float>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameObject target = other.gameObject;
            if (!hitTimestamps.ContainsKey(target) || Time.time - hitTimestamps[target] >= hitCooldown)
            {
                PlayerController player = target.GetComponent<PlayerController>();
                if (player != null)
                {
                    player.TakeDamage(damage);
                    Vector2 direction = (target.transform.position - transform.position).normalized;
                    player.ApplyKnockback(direction, knockbackForce);
                    hitTimestamps[target] = Time.time;
                }
            }
        }
    }
    
    private void OnDisable()
    {
        hitTimestamps.Clear();
    }
}
