using System.Collections;
using UnityEngine;

public class SlotMachine : MonoBehaviour
{
    [Header("Potion Settings")]
    [SerializeField] private GameObject dynamicPotionPrefab;
    [SerializeField, Range(0f, 1f)] private float potionSpawnChance = 0.5f;

    private Animator animator;
    private bool isActive = true;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isActive) return;

        
        Jugador player = other.GetComponent<Jugador>();
        if (player != null)
        {
            // Try to remove 1 coin
            if (GameManager.Instance.RemoveCoins(1))
            {
                isActive = false;                
                animator.SetTrigger("Spin");
                Debug.Log("Slot machine activated!");
                StartCoroutine(HandleSlotMachineResult());
            }
            else
            {
                // Optionally: feedback for not enough coins
                Debug.Log("Not enough coins to use the slot machine.");
            }
        }
    }

    private IEnumerator HandleSlotMachineResult()
    {
        // Wait for animation to finish (replace "Spin" with your animation state name)
        yield return null;
        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
        float animLength = state.length > 0 ? state.length : 1f;
        Debug.Log($"Animation length: {animLength} seconds");        
        yield return new WaitForSeconds(animLength);

        // Chance to spawn potion
        if (Random.value < potionSpawnChance && dynamicPotionPrefab != null)
        {
            Instantiate(dynamicPotionPrefab, transform.position + Vector3.down*2, Quaternion.identity);
        }

        isActive = true; // Allow reuse if desired
    }
}
