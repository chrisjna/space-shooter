using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private GameObject _enemyPrefab;
    [SerializeField]
    private GameObject _enemyContainer;
    [SerializeField]
    private GameObject[] powerups;
    [SerializeField]
    private GameObject _ouchPrefab;
    private float _timer = 0;

    private bool _stopSpawning = false;
    private bool _ouchIsDead = true;
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
        if(_ouchIsDead)
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
            GameObject newEnemy = Instantiate(_enemyPrefab, posToSpawn, Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;
            if (_ouchIsDead && _timer > 20)
            {
                Vector3 ouchToSpawn = new Vector3(Random.Range(-8f, 8f), 4, 0);
                Instantiate(_ouchPrefab, ouchToSpawn, Quaternion.identity);
                _ouchIsDead = false;
            }
            yield return new WaitForSeconds(5.0f);
        }
    }

    IEnumerator SpawnPowerUpRoutine()
    {
        while (_stopSpawning == false)
        {
            Vector3 postToSpawn = new Vector3(Random.Range(-.8f, 8f), 7, 0);
            int randomPowerUp = Random.Range(0, 3);
            Instantiate(powerups[randomPowerUp], postToSpawn, Quaternion.identity);
            yield return new WaitForSeconds(Random.Range(3, 8));
        }
    }
    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }

    public void OuchEnemyDied()
    {
        _ouchIsDead = true;
    }
}
