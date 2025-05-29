using UnityEngine;
using System.Collections;
using TMPro;

public class RoundManager : MonoBehaviour
{
    public int totalRounds = 3;
    private int currentRound = 0;
    public float timeBetweenRounds = 2f;
    public PlayerController player;
    public GameObject enemy;
    public Transform enemySpawnPoint;
    public TMP_Text roundUIText;
    private bool roundActive = false;

    void Start()
    {
        ResetEnemy();
        StartCoroutine(StartNextRound());
    }

    IEnumerator StartNextRound()
    {
        if (currentRound > 0)
        {
            roundUIText.text = $"Round {currentRound} over!";
            yield return new WaitForSeconds(timeBetweenRounds);
        }

        if (currentRound < totalRounds)
        {
            currentRound++;
            roundUIText.text = $"Round {currentRound}";
            roundActive = true;
            ResetRound();
            while (roundActive)
            {
                yield return null;
                EnemyController enemyCtrl = enemy.GetComponent<EnemyController>();
                if (enemyCtrl == null)
                {
                    Debug.LogError("EnemyController not available to the enemy");
                    yield break;
                }
                if (enemyCtrl.isDead)
                {
                    roundUIText.text = "You win in this round!";
                    roundActive = false;
                }
                if (player.isDead)
                {
                    roundUIText.text = "You lose in this round";
                    roundActive = false;
                }
            }
            StartCoroutine(StartNextRound());
        }
        else
        {
            roundUIText.text = "All rounds completed! You win!";
        }
    }

    void ResetRound()
    {
        player.currentHealth = player.maxHealth;
        player.healthBar.SetHealth(player.currentHealth, player.maxHealth);
        player.isDead = false;
        player.InitializePlayer();
        player.ResetAnimations();
        player.transform.position = Vector3.zero;
        ResetEnemy();
    }

    void ResetEnemy()
    {
        enemy.transform.position = enemySpawnPoint.position;
        EnemyController enemyCtrl = enemy.GetComponent<EnemyController>();
        if (enemyCtrl != null)
        {
            enemyCtrl.ResetEnemy();
        }
        else
        {
            Debug.LogError("EnemyController is not available to the enemy");
        }
    }
}
