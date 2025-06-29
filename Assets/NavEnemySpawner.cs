using UnityEngine;

public class NavEnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnInterval = 5f;
    public int maxEnemiesToSpawn = 3;

    private int spawnedCount = 0;
    private float timer;

    private void Start()
    {
        timer = spawnInterval;
    }

    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0 && spawnedCount < maxEnemiesToSpawn)
        {
            SpawnEnemy();
            timer = spawnInterval;
        }

        if (spawnedCount >= maxEnemiesToSpawn)
        {
            Destroy(gameObject); // Self destruct
        }
    }

    private void SpawnEnemy()
    {
        //Instantiate the enemy a bit away from the spawner position to avoid overlap
        Vector2 spawnPosition = transform.position + (Vector3)(Random.insideUnitCircle * 0.5f); // Random offset
        spawnPosition = new Vector2(Mathf.Round(spawnPosition.x), Mathf.Round(spawnPosition.y)); // Round to avoid sub-pixel issues
        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        spawnedCount++;

        GameManager.Instance?.RegisterEnemy();
    }
}
