using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDouble : MonoBehaviour
{
    [SerializeField] private int NumberOfProjectiles = 3;
    [SerializeField] private float BulletForce = 2f;
    [Range(0, 360)]
    [SerializeField] private float SpreadAngle = 20;
   
    [SerializeField] private float hp = 25;
    [SerializeField] private GameObject _ouchPrefab;
    private float _speed = 3.5f;
    
    private float latestDirectionChangeTime;
    private readonly float directionChangeTime = 3f;
    private Vector2 movementDirection;
    private Vector2 movementPerSecond;
    private float _timer = 5;

    private SpawnManager _spawnManager;

    // Start is called before the first frame update
    void Start()
    {
        latestDirectionChangeTime = 0f;
        CalculateMovement();
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        if (_spawnManager == null)
        {
            Debug.LogError("Spawn Manager is NULL");
        }
    }

    // Update is called once per frame
    void Update()
    {
        _timer += Time.deltaTime;
        if (Time.time - latestDirectionChangeTime > directionChangeTime)
        {
            latestDirectionChangeTime = Time.time;
            CalculateMovement();
        }
        if (_timer > 10)
        {
            FireOuch();
            _timer = 0;
        }
        transform.position = new Vector2(Mathf.Clamp(transform.position.x + (movementPerSecond.x * Time.deltaTime), -8, 8),
            Mathf.Clamp(transform.position.y + (movementPerSecond.y * Time.deltaTime), 4, 5));
    }

    void CalculateMovement()
    {
        movementDirection = new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)).normalized;
        movementPerSecond = movementDirection * _speed;
    }

    void FireOuch()
    {
        // reference to https://www.reddit.com/r/Unity2D/comments/gh9rrf/how_can_i_create_a_bullet_spread_in_2d/
        float angleStep = SpreadAngle / NumberOfProjectiles;
        float centeringOffset = (SpreadAngle / 2) - (angleStep / 2);                                                                                                                       //centered on the mouse cursor

        for (int i = 0; i < NumberOfProjectiles; i++)
        {
            float currentBulletAngle = angleStep * i;

            Quaternion rotation = Quaternion.Euler(new Vector3(0, 0,  currentBulletAngle - centeringOffset));
            GameObject bullet = Instantiate(_ouchPrefab, transform.position + new Vector3(0, -0.6f, 0), rotation);

            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.AddForce(bullet.transform.up * -1.0f * BulletForce, ForceMode2D.Impulse);
        // end reference
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.transform.GetComponent<Player>();
            if (player != null)
            {
                Destroy(player);
            }
            Destroy(this.gameObject);
        }

        if (other.CompareTag("Laser"))
        {
            if (hp < 1)
            {
                Destroy(this.gameObject);
                _spawnManager.OuchEnemyDied();
            }
            else
            {
                Destroy(other.gameObject);
                hp--;
            }
        }
    }
}
