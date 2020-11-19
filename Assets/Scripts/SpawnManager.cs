using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Threading;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{   
    private float _timerPowerUp = 0;
    private bool _stopSpawning = false;
    private UIManager _uiManager;

    [SerializeField] private GameObject _enemyContainer;
    [SerializeField] private GameObject[] powerups;

    // credit to Brackeys Youtube start
    public enum SpawnState { SPAWNING, WAITING, COUNTING }
    [System.Serializable]
    public class Wave
    {
        public string name;
        public Transform enemy;
        public int count;
        public float rate;
    }
    public Wave[] waves;
    private int nextWave = 0;

    public float timeBetweenWaves = 5f;
    private float waveCountdown;

    private float searchCountdown = 1f;

    private SpawnState state = SpawnState.COUNTING;

    void Start()
    {
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        waveCountdown = timeBetweenWaves;
        Invoke("Spawn", 3);
    }

    void Update()
    {
        if (!_stopSpawning)
        {
            _timerPowerUp += Time.deltaTime;

            if (state == SpawnState.WAITING)
            {
                if (!EnemyIsAlive())
                {
                    WaveCompleted();
                }
                else
                {
                    return;
                }
            }

            if (waveCountdown <= 0)
            {
                if (state != SpawnState.SPAWNING)
                {
                    _uiManager.WaveIsDone();
                    StartCoroutine(SpawnWave(waves[nextWave]));
                }
            }
            else
            {
                waveCountdown -= Time.deltaTime;
            }
        }
    }

    void Spawn()
    {
        StartCoroutine(SpawnPowerUpRoutine());
    }
    void WaveCompleted()
    {
        Debug.Log("Wave Completed");
        state = SpawnState.COUNTING;
        waveCountdown = timeBetweenWaves;

        if (nextWave + 1 > waves.Length - 1)
        {
            nextWave = 0;
            _uiManager.UpdateWave(nextWave, timeBetweenWaves);
            Debug.Log("loop");
        }

        else
        {
            nextWave++;
            _uiManager.UpdateWave(nextWave, timeBetweenWaves);
        }
    }
    bool EnemyIsAlive()
    {
        searchCountdown -= Time.deltaTime;
        if (searchCountdown <= 0f)
        {
            searchCountdown = 1f;
            if (GameObject.FindGameObjectWithTag("Enemy") == null)
            {
                return false;
            }
        }
        return true;
    }

    IEnumerator SpawnWave(Wave _wave)
    {
        state = SpawnState.SPAWNING;

        // Spawn
        for (int i = 0; i < _wave.count; i++)
        {
            SpawnEnemy(_wave.enemy);
            yield return new WaitForSeconds(1f / _wave.rate);
        }
        state = SpawnState.WAITING;

        yield break;
    }

    void SpawnEnemy(Transform _enemy)
    {
        Vector3 posToSpawn = new Vector3(Random.Range(-8f, 8f), 9, 0);
        Instantiate(_enemy, posToSpawn, Quaternion.identity);
    }
    // END
    /*
     * Recommendations for powerups id
     * 1 - 
     * 2 -
     * 3 -
     * 4 - 
     * 5 -
     */
    IEnumerator SpawnPowerUpRoutine()
    {
        while (_stopSpawning == false)
        {
            Vector3 postToSpawn = new Vector3(Random.Range(-.8f, 8f), 7, 0);
            int randomPowerUp = Random.Range(0,7);

            if (randomPowerUp != 4)
            {
                Instantiate(powerups[randomPowerUp], postToSpawn, Quaternion.identity);
                yield return new WaitForSeconds(Random.Range(4, 8));
            }
            else if (randomPowerUp == 4 && _timerPowerUp > 30)
            {
                Instantiate(powerups[4], postToSpawn, Quaternion.identity);
                _timerPowerUp = 0;
                yield return new WaitForSeconds(Random.Range(4, 8));
            } else
            {
                yield return new WaitForSeconds(1);
            }
        }
    }
    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }
}
