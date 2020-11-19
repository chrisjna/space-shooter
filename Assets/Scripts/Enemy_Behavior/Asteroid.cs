using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Asteroid : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private float _speed = 4.0f;
    [SerializeField] private float _rotateSpeed = 20.0f;
    [SerializeField] private AudioClip _explosionSoundClip;

    private Player _player;
    private Animator _anim;
    private AudioSource _audioSource;
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        _anim = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
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
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector2.down * _speed * Time.deltaTime, Space.World);
        transform.Rotate(Vector3.forward * _rotateSpeed * Time.deltaTime);
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
            this.gameObject.tag = "Untagged";
            _anim.SetTrigger("OnAsteroidDeath");
            _audioSource.Play();
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
                this.gameObject.tag = "Untagged";
                _anim.SetTrigger("OnAsteroidDeath");
                _audioSource.Play();
                _speed = 0;
                Destroy(this.gameObject, 2.1f);
                this.gameObject.GetComponent<Collider2D>().enabled = false;
            }
        }
    }
}
