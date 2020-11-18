using UnityEngine;

public class EnemyFast : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private float _speed = 5.0f;
    [SerializeField] private AudioClip _explosionSoundClip;
    [SerializeField] private GameObject _ouchPrefab;

    private Player _player;
    private Animator _anim;
    private AudioSource _audioSource;
    private Collider2D _collider;

    private float _timer = 0;
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        _anim = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
        _collider = GetComponent<Collider2D>();

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
            Destroy(this.gameObject);
        }

        if (_timer > 0.8f)
        {
            if (_collider.isActiveAndEnabled)
            {
                FireFastOuch();
                _timer = 0;
            }
        }
    }

    private void FireFastOuch()
    {
        if (_player != null)
        {
            Vector2 direction = _player.transform.position - transform.position;
            float vecAngle = Vector2.Angle(transform.up, direction);
            if (vecAngle > 125f && vecAngle < 190f)
            {
                GameObject bullet = Instantiate(_ouchPrefab, transform.position + new Vector3(0, -0.6f, 0), Quaternion.identity);
                Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
                direction.Normalize();
                rb.velocity = direction * 6f;
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
            _collider.enabled = false;
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
                _collider.enabled = false;
            }
        }
    }
}
