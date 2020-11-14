using UnityEngine;

public class EnemyFast : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private float _speed = 5.0f;
    [SerializeField] private float _rotateSpeed = 20.0f;
    [SerializeField] private AudioClip _explosionSoundClip;
    [SerializeField] private GameObject _ouchPrefab;

    private Player _player;
    private Animator _anim;
    private AudioSource _audioSource;

    private float _timer = 0;
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
        _timer += Time.deltaTime;
        float randomX = Random.Range(-8f, 8f);
        transform.Translate(Vector2.down * _speed * Time.deltaTime, Space.World);

        if (transform.position.y < -5f)
        {
            transform.position = new Vector3(randomX, 7, 0);
        }

        if (_timer > 2)
        {
            FireFastOuch();
            _timer = 0;
        }
    }

    private void FireFastOuch()
    {
        if (_player != null)
        {
            Vector2 direction = _player.transform.position - transform.position;
            if (Vector2.Angle(transform.up, direction) > 90)
            {
                GameObject bullet = Instantiate(_ouchPrefab, transform.position, Quaternion.identity);
                Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
                direction.Normalize();
                rb.velocity = direction * 5f;
            }
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
                _anim.SetTrigger("OnAsteroidDeath");
                _audioSource.Play();
                _speed = 0;
                Destroy(this.gameObject, 2.1f);
                this.gameObject.GetComponent<Collider2D>().enabled = false;
            }
        }
    }
}
