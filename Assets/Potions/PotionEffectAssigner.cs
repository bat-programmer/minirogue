using Assets.Potions;
using UnityEngine;

public class PotionEffectAssigner : MonoBehaviour
{
    private bool initialized = false;
    public bool isBeneficial; // still public for reference/debugging
    [Tooltip("If left empty, a random effect will be assigned.")]
    public string specificEffectType = ""; // Example: "HealthPotionEffect
    public void Initialize(bool isBeneficialFlag)
    {
        isBeneficial = isBeneficialFlag;
        AssignEffect();
        initialized = true;
    }
    private void Start()
    {
        if (!initialized)
        {
            isBeneficial = true; // or false if you want
            AssignEffect();
        }
    }

    private void Awake()
    {
        if (initialized)
        {
            //Debug.LogWarning("PotionEffectAssigner initialized via Awake, not ideal.");
            AssignEffect();
        }
    }

    private void AssignEffect()
    {
        // First potion is always beneficial
        bool isFirstPotion = !GameManager.Instance.HasAssignedFirstPotion;
        
        System.Type[] goodEffects = { typeof(HealthPotionEffect), typeof(SpeedUpPotionEffect) };
        System.Type[] badEffects = { typeof(DamagePotionEffect), typeof(SpeedDownPotionEffect) };

        var pool = isFirstPotion || isBeneficial ? goodEffects : badEffects;
        Debug.Log($"Assigning {(isFirstPotion ? "first beneficial" : isBeneficial ? "beneficial" : "harmful")} effect");
        
        int randomIndex = Random.Range(0, pool.Length);
        gameObject.AddComponent(pool[randomIndex]);

        if (isFirstPotion) {
            GameManager.Instance.HasAssignedFirstPotion = true;
        }
    }
}
