﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Text _scoreText;
    [SerializeField] private Image _livesImg;
    [SerializeField] private Sprite[] _livesSprite;
    [SerializeField] private Text _gameOverText;
    [SerializeField] private Text _restartText;
    [SerializeField] private Text _startText;
    [SerializeField] private Text _ammo;
    [SerializeField] private Image _timer;
    [SerializeField] private Text _wave;
    private GameManager _gameManager;
    
    // Start is called before the first frame update
    void Start()
    {
        _scoreText.text = "Score: " + 0;
        _ammo.text = "Ammo: " + Mathf.Infinity;
        _gameOverText.gameObject.SetActive(false);
        _restartText.gameObject.SetActive(false);
        _startText.gameObject.SetActive(true);
        _timer.gameObject.SetActive(false);
        _wave.gameObject.SetActive(false);

        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        if (_gameManager == null)
        {
            Debug.LogError("Game Manager is Null");
        }
    }

    // Update is called once per frame
    void Update()
    {
        Invoke("StartSequence", 3);
    }

    void StartSequence()
    {
        _startText.gameObject.SetActive(false);
    }
    public void UpdateAmmo(float ammo)
    {
        _ammo.text = "Ammo: " + ammo.ToString();
    }
    public void UpdateScore(int playerScore)
    {
        _scoreText.text = "Score: " + playerScore.ToString();
    }
    
    public void UpdateLives(int currentLives)
    {
        _livesImg.sprite = _livesSprite[currentLives];

        if (currentLives == 0)
        {
            GameOverSequence();
        }
    }

    public void UpdateWave( int wave, float waitTimer)
    {
        _wave.text = "Wave: " + wave.ToString();
        _wave.gameObject.SetActive(true);
    }

    public void WaveIsDone()
    {
        _wave.gameObject.SetActive(false);
    }

    public void PowerUpCollected()
    {
        _timer.gameObject.SetActive(true);
        StartCoroutine(PowerUpEndTimer());
    }
    IEnumerator PowerUpEndTimer()
    {
        yield return new WaitForSeconds(5.0f);
        _timer.gameObject.SetActive(false);
    }

    void GameOverSequence()
    {
        _gameManager.GameOver();
        _gameOverText.gameObject.SetActive(true);
        StartCoroutine(GameOverFlickerRoutine());
        _restartText.gameObject.SetActive(true);
    }
    IEnumerator GameOverFlickerRoutine()
    {
        while (true)
        {
            _gameOverText.text = "GAME OVER";
            yield return new WaitForSeconds(0.5f);
            _gameOverText.text = "";
            yield return new WaitForSeconds(0.5f);
        }
    }
}
