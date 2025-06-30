using UnityEngine;

public class AnimationEventForwarder : MonoBehaviour
{
    public NavEnemyBase parentEnemy;


    void Start()
    {
        parentEnemy = GetComponentInParent<NavEnemyBase>();
        if (parentEnemy == null)
        {
            Debug.LogError($"No NavEnemyBase found in parent of {gameObject.name}");
        }
    }

    // Called via animation event
    public void DealDamageToPlayer()
    {
        if (parentEnemy != null)
        {
            parentEnemy.DealDamageToPlayer();
        }
        else
        {
            Debug.LogWarning($"[AnimationEventForwarder] No enemyScript assigned on {name}");
        }
    }
}
