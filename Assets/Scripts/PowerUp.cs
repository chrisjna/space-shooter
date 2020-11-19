using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    // Start is called before the first frame update
    private float _speed = 3.0f;
    [SerializeField]
    private int powerupID = 0;

    private Player _player;
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
        if (transform.position.y < -4.5f)
        {
            Destroy(this.gameObject);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            Rigidbody2D rb = this.gameObject.GetComponent<Rigidbody2D>();
            Vector2 direction = _player.transform.position - transform.position;
            direction.Normalize();
            rb.velocity = direction * 3f;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();
            if (player != null)
            {
                switch(powerupID)
                {
                    case 0:
                        player.TripleShotActive();
                        break;
                    case 1:
                        player.SpeedBoostActive();
                        break;
                    case 2:
                        player.ShieldsActive();
                        break;
                    case 3:
                        player.HealthIncrease();
                        break;
                    case 4:
                        player.MissileActive();
                        break;
                    case 5:
                        player.JammerActive();
                        break;
                    case 6:
                        player.HomingActive();
                        break;
                    default:
                        Debug.Log("Powerup broke");
                        break;
                }
            }
            Destroy(this.gameObject);
        }
    }
}
