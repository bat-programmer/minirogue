using UnityEngine;

public class LevelExitSpawner : MonoBehaviour
{
    public GameObject changeLevelPrefab;
    public Transform spawnPoint;

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnAllEnemiesDefeated += SpawnExit;
        }
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnAllEnemiesDefeated -= SpawnExit;
        }
    }

    private void SpawnExit()
    {
        Debug.Log("[LevelExitSpawner] Spawning level exit.");
        Instantiate(changeLevelPrefab, spawnPoint.position, Quaternion.identity);
    }
}

