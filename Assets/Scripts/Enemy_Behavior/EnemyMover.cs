using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMover : MonoBehaviour
{
    [SerializeField] private float _speed = 1.0f;
    [SerializeField] private AudioClip _explosionSoundClip;
    [SerializeField] private GameObject _shieldVisualizer;

    private Player _player;
    private Animator _anim;
    private AudioSource _audioSource;
    private Collider2D _collider;

    float frequency = 1.5f; // Speed of sine movement
    float magnitude = 5f; //  Size of sine movement
    Vector3 pos;
    Vector3 axis;

    private float _lives = 2;
    private bool _isAlive = true;

    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        _anim = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
        _collider = GetComponent<Collider2D>();
        _shieldVisualizer.SetActive(true);

        if (_anim == null)
        {
            Debug.LogError("animator is null");
        }
        if (_audioSource == null)
        {
            Debug.Log("Audio is missing");
        }
        else
        {
            _audioSource.clip = _explosionSoundClip;
        }
        pos = transform.position;
        axis = transform.right;
    }

    // Update is called once per frame
    void Update()
    {
        if (_isAlive)
        {
            CalculateMove();
        }
    }

    private void CalculateMove()
    {
        pos += Vector3.down * Time.deltaTime * _speed;
        transform.position = pos + axis * Mathf.Sin(Time.time * frequency) * magnitude;
        if (transform.position.y < -5f)
        {
            Destroy(this.gameObject);
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
            if (_lives < 2)
            {
                this.gameObject.tag = "Untagged";
                _isAlive = false;
                _anim.SetTrigger("OnAsteroidDeath");
                _audioSource.Play();
                Destroy(this.gameObject, 2.1f);
                _collider.enabled = false;
            } else
            {
                _shieldVisualizer.SetActive(false);
                _lives--;
            }
        }

        if (other.tag == "Laser")
        {
            Destroy(other.gameObject);
            if (_player != null)
            {
                if (_lives < 2)
                {
                    this.gameObject.tag = "Untagged";
                    _isAlive = false;
                    _player.AddScore(10);
                    _anim.SetTrigger("OnAsteroidDeath");
                    _audioSource.Play();
                    Destroy(this.gameObject, 2.1f);
                    _collider.enabled = false;
                }
                else
                {
                    _shieldVisualizer.SetActive(false);
                    _lives--;
                }
            }
        }
    }
}