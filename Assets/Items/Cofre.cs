using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cofre : MonoBehaviour
{
    [Header("Item Pool")]
    public GameObject[] itemPrefabs;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ChestOpen()
    {
        SpawnRandomItem();
        Destroy(gameObject);
    }

    void SpawnRandomItem()
    {
        if (itemPrefabs.Length == 0) return;
        int index = Random.Range(0, itemPrefabs.Length);
        Instantiate(itemPrefabs[index], transform.position, Quaternion.identity);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //trigger the chest opening animation
            
            Animator animator = GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetTrigger("Open");
                //StartCoroutine(WaitAndSpawnItem(10f));
            }
         
           
        }
    }
}
