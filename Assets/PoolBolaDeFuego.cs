using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolBolaDeFuego : MonoBehaviour
{
    public GameObject fireballPrefab;
    public int poolSize = 10;  // Adjust based on needs

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


    public GameObject GetFireball(Vector3 position, Vector2 direction)
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
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
