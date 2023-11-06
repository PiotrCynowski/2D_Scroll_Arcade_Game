using System;
using System.Collections;
using UnityEngine;
using Piotr.SpawnWithPool;
using Game.Stats;

public class EnemyManager : MonoBehaviour
{
    [Header("Enemy Settings")]
    public EnemyBehaviour[] enemiesPrefabs;

    [Range(0.5f, 5)]
    [SerializeField] private float timeBetweenEnemyGroupSpawn;
    [Range(0.5f, 5)]
    [SerializeField] private float enemiesTotalTimeForReachingTarget;
    [Range(0.5f, 3)]
    [SerializeField] private float verticalOffsetBetweenEnemies;
    [Range(1, 7)]
    [SerializeField] private int maxEnemiesInYAxis;

    private bool isSpawning;

    private Vector2[] enemyYStartPos;
    private Vector3 enemeiesStart, enemiesTarget;

    private Coroutine SpawnCoroutine;

    private SpawnWithPool enemySpawner;

    private void Start()
    {
        PlayerStats.onGameOver += GameRestart;

        Vector2 screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

        SetupEnemiesStartPositions(screenBounds);

        enemySpawner = new SpawnWithPool();
        foreach(EnemyBehaviour enemy in enemiesPrefabs)
        {
            enemySpawner.toSpawn.Add(enemy.gameObject);

        }
        enemySpawner.Pool.Clear();
    }

    private void GameRestart(bool isGameOver)
    {
        if (isGameOver)
        {
            StopCoroutine(SpawnCoroutine);
        }
        else
        {
            SpawnCoroutine = StartCoroutine(SpawnEnemyGroup());
        }
    }

    private void SetupEnemiesStartPositions(Vector2 _screen)
    {
        enemyYStartPos = new Vector2[maxEnemiesInYAxis];

        enemeiesStart = new Vector2(_screen.x + 1, 0);
        enemiesTarget = new Vector2(-_screen.x - 1, 0);

        float startPoint = ((maxEnemiesInYAxis - 1) * verticalOffsetBetweenEnemies) / 2;

        for (int i = 0; i < enemyYStartPos.Length; i++)
        {
            enemyYStartPos[i].y = startPoint + (-i * verticalOffsetBetweenEnemies);
            enemyYStartPos[i].x = enemeiesStart.y;
        }
    }
    

    private IEnumerator SpawnEnemyGroup()
    {
        isSpawning = true;

        while (isSpawning)
        {
            GameObject containerForEnemies = new("groupOfEnemies");

            for (int i = 0; i < enemyYStartPos.Length; i++)
            {
                if (UnityEngine.Random.value > 0.5f)
                {
                    GameObject plane = enemySpawner.Pool.Get();
                    plane.transform.position = enemyYStartPos[i];
                    plane.transform.parent = containerForEnemies.transform;
                }
            }

            StartCoroutine(MoveGroupOfEnemiesFromStartToEndPoint(containerForEnemies.transform));

            yield return new WaitForSeconds(timeBetweenEnemyGroupSpawn);
        }
    }

    IEnumerator MoveGroupOfEnemiesFromStartToEndPoint(Transform groupToMove)
    {
        float elapsedTime = 0f;
        while (elapsedTime < enemiesTotalTimeForReachingTarget)
        {
            groupToMove.position = Vector3.Lerp(enemeiesStart, enemiesTarget, (elapsedTime / enemiesTotalTimeForReachingTarget));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        foreach(Transform enemy in groupToMove)
        {
            if (enemy.gameObject.activeInHierarchy)
            {
                enemySpawner.Pool.Release(enemy.gameObject);
            }
        }

        yield return new WaitUntil(() => groupToMove.childCount <= 0);
        Destroy(groupToMove.gameObject);

        yield return null;

    }
}
