using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject playerPrefab;
    public Transform spawnPoint;
    private CameraFollow cameraFollow;

    void Start()
    {
        SpawnPlayer();
    }

    void SpawnPlayer()
    {
        GameObject player = Instantiate(playerPrefab, spawnPoint.position, Quaternion.identity);

        // Assign the player's transform to the GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.playerTransform = player.transform;
        }

        cameraFollow = Camera.main.GetComponent<CameraFollow>();
        if (cameraFollow != null)
        {
            cameraFollow.SetPlayer(player.transform);
        }
    }
}
