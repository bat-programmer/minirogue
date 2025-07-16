using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Cofre : MonoBehaviour
{
    [Header("Item Pool")]
    public GameObject[] itemPrefabs;

    [Header("Wand Prefabs")]
    public GameObject[] wandPrefabs; // Assign these in the inspector

    // Start is called before the first frame update
void Start()
{
    Debug.Log($"[Cofre] Wand prefab count at start: {wandPrefabs.Length}");
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
        if (GameManager.Instance != null)
        {
            var availableWands = System.Enum.GetValues(typeof(FireballEffectType))
                .Cast<FireballEffectType>()
                .Except(GameManager.Instance.PickedUpWands)
                .ToList();

            if (availableWands.Count > 0)
            {
                FireballEffectType wandToSpawn = availableWands[Random.Range(0, availableWands.Count)];
                Debug.Log($"Spawning wand of type: {wandToSpawn}");
                SpawnWand(wandToSpawn);
            }
            else
            {
                // All wands picked up, spawn a dynamic potion
                if (itemPrefabs.Length == 0) return;

                int index = Random.Range(0, itemPrefabs.Length);
                GameObject spawnedItem = Instantiate(itemPrefabs[index], transform.position, Quaternion.identity);

                // Decide if it's beneficial or not (could be random or based on level, etc.)
                bool forceBeneficial = true;
                var effectAssigner = spawnedItem.GetComponent<PotionEffectAssigner>();
                if (effectAssigner != null)
                {
                    Debug.Log($"Assigning effect to {spawnedItem.name}. Beneficial: {forceBeneficial}");
                    effectAssigner.Initialize(forceBeneficial);
                }
            }
        }
    }

    void SpawnWand(FireballEffectType type)
    {
        // Find the wand prefab that matches the given FireballEffectType
        GameObject wandPrefab = wandPrefabs
            .FirstOrDefault(wand => wand.GetComponent<WandPickup>().WandEffectType == type);

        if (wandPrefab != null)
        {
            GameObject spawnedWand = Instantiate(wandPrefab, transform.position, Quaternion.identity);
            // Perform any additional operations on the spawned wand here
        }
        else
        {
            Debug.LogWarning($"No wand prefab found for FireballEffectType: {type}");
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
