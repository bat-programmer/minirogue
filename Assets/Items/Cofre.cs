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

    //called from the animation event
    void ChestOpen()
    {
        SpawnRandomItem();
        Destroy(gameObject);
    }

    void SpawnRandomItem()
    {
        if (itemPrefabs.Length == 0) return;

        int index = Random.Range(0, itemPrefabs.Length);
        GameObject spawnedItem = Instantiate(itemPrefabs[index], transform.position, Quaternion.identity);

        // Decide if it's beneficial or not (could be random or based on level, etc.)
        //bool forceBeneficial = Random.value > 0.5f; // Example
        bool forceBeneficial = true;
        var effectAssigner = spawnedItem.GetComponent<PotionEffectAssigner>();
        if (effectAssigner != null)
        {
            Debug.Log($"Assigning effect to {spawnedItem.name}. Beneficial: {forceBeneficial}");
            effectAssigner.Initialize(forceBeneficial);
        }
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
                
            }
         
           
        }
    }
}
