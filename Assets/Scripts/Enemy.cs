using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Enemy : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private float _speed = 4.0f;
    [SerializeField] private float _rotateSpeed = 20.0f;
    Player _player;
    private Animator _anim;
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        _anim = GetComponent<Animator>();
        if (_anim == null)
        {
            Debug.LogError("animator is null");
        }
    }

    // Update is called once per frame
    void Update()
    {
        float randomX = Random.Range(-8f, 8f);
        transform.Translate(Vector2.down * _speed * Time.deltaTime, Space.World);
        transform.Rotate(Vector3.forward * _rotateSpeed * Time.deltaTime);
        if (transform.position.y < -5f)
        {
            transform.position = new Vector3(randomX, 7 ,0);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();
            if (player != null)
            {
                player.Damage();
            }
            _anim.SetTrigger("OnAsteroidDeath");
            _speed = 0;
            Destroy(this.gameObject, 2.1f);
            this.gameObject.GetComponent<Collider2D>().enabled = false;
        }

        if (other.tag == "Laser")
        {
            Destroy(other.gameObject);
            if (_player != null)
            {
                _player.AddScore(10);
                _anim.SetTrigger("OnAsteroidDeath");
                _speed = 0;
                Destroy(this.gameObject, 2.1f);
                this.gameObject.GetComponent<Collider2D>().enabled = false;
            }
        }
    }
}
