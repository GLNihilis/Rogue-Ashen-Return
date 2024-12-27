using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefabs;
    public int poolSize = 1;
    private List<GameObject> enemyPool;

    public float respawnDelay = 3f;
    private bool isWaitingToRespawn = false;

    public float randomRangeX = 3f;
    public float randomRangeY = 0f;

    //public float spawnInterval = 5f;
    //private float spawnTimer;

    // Start is called before the first frame update
    void Start()
    {
        enemyPool = new List<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject enemy = Instantiate(enemyPrefabs);
            enemy.SetActive(false);
            enemyPool.Add(enemy);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isWaitingToRespawn && !AnyEnemyActive())
        {
            StartCoroutine(RespawnEnemyAfterDelay(respawnDelay));
        }

        // Spawn Following Time
        //spawnTimer += Time.deltaTime;
        //if (spawnTimer > spawnInterval)
        //{
        //    Vector2 spawnPosition = new Vector2(Random.Range(-3f, 3f), 0);
        //    SpawnEnemy(spawnPosition);
        //    spawnTimer = 0;
        //}
    }

    private bool AnyEnemyActive()
    {
        foreach (GameObject enemy in enemyPool)
        {
            if (enemy.activeInHierarchy) return true;
        }
        return false;
    }

    private IEnumerator RespawnEnemyAfterDelay(float delay)
    {
        isWaitingToRespawn = true;
        yield return new WaitForSeconds(delay);
        Vector2 spawnPosition = new Vector2(transform.position.x + Random.Range(-randomRangeX, randomRangeX), transform.position.y + Random.Range(-randomRangeY, randomRangeY));
        SpawnEnemy(spawnPosition);
        isWaitingToRespawn = false;
    }

    public GameObject SpawnEnemy(Vector2 position)
    {
        foreach (GameObject enemy in enemyPool)
        {
            if (!enemy.activeInHierarchy)
            {
                enemy.transform.position = position;
                enemy.SetActive(true);
                
                return enemy;
            }
        }

        GameObject newEnemy = Instantiate(enemyPrefabs);
        newEnemy.transform.position = position;
        enemyPool.Add(newEnemy);
        return newEnemy;
    }

    public void ReturnPool(GameObject enemy)
    {
        enemy.SetActive(false);
    }
}
