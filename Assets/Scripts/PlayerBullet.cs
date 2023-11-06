using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class PlayerBullet : MonoBehaviour, IReturningToPool
{
    public Action<GameObject> thisBulletDestroyed;
    private Rigidbody2D rigidBody;
    [SerializeField] private float bulletSpeed;

    public void OnInitReturningToPool(Action<GameObject> returnToPoolAction)
    {
        thisBulletDestroyed = returnToPoolAction;
    }

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        rigidBody.MovePosition(rigidBody.position + bulletSpeed * Vector2.right * Time.fixedDeltaTime);
    }

    private void OnBecameInvisible()
    {
        if (gameObject.activeSelf)
        {
            thisBulletDestroyed.Invoke(gameObject);
        }
    }

    private void OnTriggerEnter2D()
    {
        thisBulletDestroyed.Invoke(gameObject);
    }
}