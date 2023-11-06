using UnityEngine;
using Piotr.SpawnWithPool;
using Game.Stats;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(BoxCollider2D))]
public class PlayerBehaviour : MonoBehaviour
{
    [SerializeField] private PlayerBullet bulletPrefab;
    [Range(0.5f, 5)]
    [SerializeField] private float bulletDelay;
    [Range(0.5f, 15)]
    [SerializeField] private float yAxisMoveSpeed;

    private float nextfire;
    private float screenYBound;

    private bool playerActive;

    private Rigidbody2D rigidBody;
    private Transform barrel;

    private SpawnWithPool bulletSpawner;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        barrel = transform.GetChild(0);

        Vector2 screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, 0));
        screenYBound = screenBounds.y;
    }

    private void Start()
    {
        playerActive = true;

        PlayerStats.onGameOver += IsPlayerActive;

        bulletSpawner = new SpawnWithPool();
        bulletSpawner.toSpawn.Add(bulletPrefab.gameObject);
        bulletSpawner.Pool.Clear();
    }

    private void IsPlayerActive(bool isGameOver)
    {
        playerActive = !isGameOver;
        rigidBody.velocity = Vector2.zero;
        
        if (!isGameOver)
        {
            transform.position = new Vector3(transform.position.x, 0, 0);
        }
    }

    private void Update()
    {
        if (playerActive)
        {
            float moveInY = Input.GetAxis("Vertical") * yAxisMoveSpeed; 
            rigidBody.velocity = new Vector2(0, moveInY);

            Vector3 movPos = transform.position; 
            movPos.y = Mathf.Clamp(transform.position.y, -screenYBound, screenYBound);
            transform.position = movPos;

            if (Input.GetButton("Fire1")) 
            {
                Shoot();
            }
        }
    }

    private void Shoot()
    {
        if (Time.time > nextfire)
        {
            nextfire = Time.time + bulletDelay;

            bulletSpawner.Pool.Get().
                GetComponent<Transform>().localPosition = barrel.position;
        }
    }
}