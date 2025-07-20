using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolBolaDeFuego : MonoBehaviour
{
    public GameObject fireballPrefab;
    public int poolSize = 50;  // Adjust based on needs

    private Queue<GameObject> fireballPool = new Queue<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject fireball = Instantiate(fireballPrefab);
            fireball.SetActive(false);
            fireballPool.Enqueue(fireball);
        }
    }


    // Add damage parameter
    public GameObject GetFireball(Vector3 position, Vector2 direction, int damage)
    {
        if (fireballPool.Count == 0)
        {
            ExpandPool();
        }

        GameObject fireball = fireballPool.Dequeue();
        fireball.transform.position = position;
        fireball.SetActive(true);

        BolaDeFuego fireballScript = fireball.GetComponent<BolaDeFuego>();
        fireballScript.SetDirection(direction);
        fireballScript.SetDamage(damage); // Set the damage

        return fireball;
    }
    public void ReturnFireball(GameObject fireball)
    {
        fireball.SetActive(false);
        fireballPool.Enqueue(fireball);
    }
    private void ExpandPool()
    {
        GameObject fireball = Instantiate(fireballPrefab);
        fireball.SetActive(false);
        fireballPool.Enqueue(fireball);
        Debug.LogWarning("Fireball pool expanded. Current size: " + fireballPool.Count);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
