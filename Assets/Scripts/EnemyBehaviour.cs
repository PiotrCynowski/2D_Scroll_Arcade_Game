using System;
using UnityEngine;
using Game.Stats;

[RequireComponent(typeof(BoxCollider2D))]
public class EnemyBehaviour : MonoBehaviour, IReturningToPool
{
    public Action<GameObject> thisEnemyKilled;

    [SerializeField] private int enemyHP; 

    private int currentEnemyHP;


    public void OnInitReturningToPool(Action<GameObject> objectForPoolRelease)
    {
        thisEnemyKilled = objectForPoolRelease;
        currentEnemyHP = enemyHP;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Bullets"))
        {
            TakeDamage();
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Player") && PlayerStats.Instance != null)
        {
            PlayerStats.Instance.PlayerLifes--;
        }
    }

    private void OnDisable()
    {
        currentEnemyHP = enemyHP;
    }

    private void TakeDamage()
    {
        currentEnemyHP--;
        if (currentEnemyHP <= 0)
        {
            if (PlayerStats.Instance != null)
            {
                PlayerStats.Instance.PlayerScore++;
            }

           thisEnemyKilled.Invoke(gameObject);
        }     
    }
}
