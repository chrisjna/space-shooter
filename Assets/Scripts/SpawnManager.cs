using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Threading;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject _enemyContainer;
    [SerializeField] private GameObject _asteroidPrefab;
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private GameObject _enemyFastPrefab;
    [SerializeField] private GameObject[] powerups;

    private float _timer = 0;
    private float _timer2 = 0;

    private bool _stopSpawning = false;
    private bool _enemyIsDead = true;
    void Start()
    {
        Invoke("Spawn", 3);
    }

    // Update is called once per frame
    void Spawn()
    {
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerUpRoutine());
    }
    void Update()
    {
        _timer2 += Time.deltaTime;
        if (_enemyIsDead)
        {
            _timer += Time.deltaTime;
        } else
        {
            _timer = 0;
        }
    }

    IEnumerator SpawnEnemyRoutine()
    {
        while (_stopSpawning == false)
        {
            Vector3 posToSpawn = new Vector3(Random.Range(-8f, 8f), 7, 0);
            Vector3 posToSpawnFast = new Vector3(Random.Range(-8f, 8f), 8, 0);

            GameObject newEnemy = Instantiate(_asteroidPrefab, posToSpawn, Quaternion.identity);
            GameObject newEnemyFast = Instantiate(_enemyFastPrefab, posToSpawnFast, Quaternion.identity);
            
            newEnemy.transform.parent = _enemyContainer.transform;
            newEnemyFast.transform.parent = _enemyContainer.transform;
            
            if (_enemyIsDead && _timer > 20)
            {
                Vector3 enemyToSpawn = new Vector3(0, 8f, 0);
                Instantiate(_enemyPrefab, enemyToSpawn, Quaternion.identity);
                _enemyIsDead = false;
            }
            yield return new WaitForSeconds(5.0f);
        }
    }

    IEnumerator SpawnPowerUpRoutine()
    {
        while (_stopSpawning == false)
        {
            Vector3 postToSpawn = new Vector3(Random.Range(-.8f, 8f), 7, 0);
            int randomPowerUp = Random.Range(0, 5);
            if (randomPowerUp == 5 && _timer2 > 30)
            {
                Instantiate(powerups[4], postToSpawn, Quaternion.identity);
                _timer = 0;
                yield return new WaitForSeconds(Random.Range(3, 8));
            }
            else
            {
                Instantiate(powerups[randomPowerUp], postToSpawn, Quaternion.identity);
                yield return new WaitForSeconds(Random.Range(3, 8));
            }
        }
    }
    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }

    public void EnemyDied()
    {
        _enemyIsDead = true;
    }
}
