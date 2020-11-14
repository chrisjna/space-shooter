using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private float _speed = 3.5f;
    [SerializeField] private GameObject _laserPrefab;
    [SerializeField] private float _fireRate = 0.5f;
    [SerializeField] private int _lives = 3;
    [SerializeField] private GameObject _tripleShotPrefab;
    [SerializeField] private GameObject _shieldVisualizer1, _shieldVisualizer2, _shieldVisualizer3;
    [SerializeField] private GameObject _rightEngine, _leftEngine;
    [SerializeField] private GameObject _thruster;
    [SerializeField] private GameObject _boostOn;
    [SerializeField] private AudioClip _laserSoundClip;

    private bool _missileOn;
    private bool _isTripleShotActive = false;
    private bool _powerUpActive = false;
    private bool _alreadyHit = false;
    private float _ammoCount = Mathf.Infinity;
    private float _speedMultiplier = 2;
    private float _shieldHealth = 0;
    private float _canFire = -1f;
    private float _invincibilityTimer = 0;
    private int _score = 0;

    private SpawnManager _spawnManager;
    private UIManager _uiManager;
    private AudioSource _audioSource;
    private ShakeBehavior _cameraManager;

    void Start()
    {
        transform.position = new Vector3(0, -3, 0);
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _cameraManager = GameObject.Find("Main Camera").GetComponent<ShakeBehavior>();
        _audioSource = GetComponent<AudioSource>();
        
        if (_cameraManager == null)
        {
            Debug.LogError("No camera");
        }
        if (_spawnManager == null)
        {
            Debug.LogError("Spawn Manager is NULL");
        }
        if(_uiManager == null)
        {
            Debug.Log("UI manager missing");
        }

        if(_audioSource == null)
        {
            Debug.Log("Audio is missing");
        } else
        {
            _audioSource.clip = _laserSoundClip;
        }
    }

    // Update is called once per frame
    void Update()
    {
        _invincibilityTimer += Time.deltaTime;
        CalculateMovement();

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
        {
            FireLaser();
        }
    }

    void CalculateMovement()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            _speed = 7f;
            _boostOn.SetActive(true);
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            _speed = 5f;
            _boostOn.SetActive(false);
        }
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);
        transform.Translate(direction * _speed * Time.deltaTime);

        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -3.8f, 0), 0);

        if (transform.position.x > 11.3f)
        {
            transform.position = new Vector3(-11.3f, transform.position.y, 0);
        }
        else if (transform.position.x < -11.3f)
        {
            transform.position = new Vector3(11.3f, transform.position.y, 0);
        }
    }

    void FireLaser()
    {
        if (_ammoCount > 0)
        {
            _ammoCount--;
            _canFire = Time.time + _fireRate;

            if (_isTripleShotActive == true)
            {
                Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);
            }
            else if (_missileOn == true)
            {
                float angleStep = 120 / 5;
                float centeringOffset = (120 / 2) - (angleStep / 2);                                                                                                                       //centered on the mouse cursor
                for (int i = 0; i < 5; i++)
                {
                    float currentBulletAngle = angleStep * i;
                    Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, currentBulletAngle - centeringOffset));
                    GameObject bullet = Instantiate(_laserPrefab, transform.position + new Vector3(0, 0.8f, 0), rotation);
                    Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
                    rb.AddForce(bullet.transform.up * -1.0f * 1f, ForceMode2D.Impulse);
                }
            } else
            {
                Instantiate(_laserPrefab, transform.position + new Vector3(0, 0.8f, 0), Quaternion.identity);
            }
            _uiManager.UpdateAmmo(_ammoCount);
            _audioSource.Play();
        }
    }

    public void Damage()
    {
        if (_alreadyHit)
        {
            if (_invincibilityTimer > 3f)
            {
                _invincibilityTimer = 0;
                _alreadyHit = false;
            }
            return;
        }
        _alreadyHit = true;
        
        if (_shieldHealth >= 1)
        {
            if (_shieldVisualizer3.activeInHierarchy == true)
            {
                _shieldVisualizer2.SetActive(true);
                _shieldVisualizer3.SetActive(false);
            }
            else if (_shieldVisualizer2.activeInHierarchy == true)
            {
               _shieldVisualizer1.SetActive(true);
                _shieldVisualizer2.SetActive(false);
            }
            else
            {
               _shieldVisualizer1.SetActive(false);
            }
            _shieldHealth--;
            return;
        }
        _lives--;
        if (_lives == 2)
        {
            _leftEngine.SetActive(true);
            _cameraManager.TriggerShake();
        }
        if (_lives == 1)
        {
            _rightEngine.SetActive(true);
            _cameraManager.TriggerShake();
        }

        _uiManager.UpdateLives(_lives);

        if (_lives < 1)
        {
            _spawnManager.OnPlayerDeath();
            Destroy(this.gameObject);
        }

    }

    public void TripleShotActive()
    {
        if (!_powerUpActive)
        {
            _powerUpActive = true;
            _uiManager.PowerUpCollected();
            _isTripleShotActive = true;
            StartCoroutine(TripleShotPowerDownRoutine());
        }
    }

    IEnumerator TripleShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _isTripleShotActive = false;
        _powerUpActive = false;
    }

    public void SpeedBoostActive()
    {
        if (!_powerUpActive)
        {
            _powerUpActive = true;
            _uiManager.PowerUpCollected();
            _thruster.SetActive(true);
            _speed = _speed * _speedMultiplier;
            StartCoroutine(SpeedBoostPowerDownRoutine());
        }
    }

    IEnumerator SpeedBoostPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _thruster.SetActive(false);
        _speed = _speed / _speedMultiplier;
        _powerUpActive = false;
    }

    public void ShieldsActive()
    {
        if (_shieldHealth == 2)
        {
            _shieldVisualizer3.SetActive(true);
            _shieldVisualizer2.SetActive(false);
            _shieldHealth++;
        } else 
        if (_shieldHealth == 1)
        {
            _shieldVisualizer2.SetActive(true);
            _shieldVisualizer1.SetActive(false);
            _shieldHealth++;
        } else 
        if (_shieldHealth == 0)
        {
            _shieldVisualizer1.SetActive(true);
            _shieldHealth++;
        }
    }

    public void HealthIncrease()
    {
        if (_lives < 3)
        {
            _lives++;
            _uiManager.UpdateLives(_lives);
            if (_lives == 2){
                _rightEngine.SetActive(false);
            } else
            {
                _leftEngine.SetActive(false);
            }
        }
    }

    public void MissileActive()
    {
        if (!_powerUpActive)
        {
            _uiManager.PowerUpCollected();
            _missileOn = true;
            _powerUpActive = true;
            StartCoroutine(MissilePowerDownRoutine());
        }
    }

    IEnumerator MissilePowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _missileOn = false;
        _powerUpActive = false;
    }
    public void AddScore(int points)
    {
        _score = _score + points;
        _uiManager.UpdateScore(_score);
    }
}