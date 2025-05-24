using UnityEngine;

public class PotionEffectAssigner : MonoBehaviour
{
    public bool isBeneficial; // set from prefab or spawner

    private void Awake()
    {
        System.Type[] goodEffects = { typeof(HealthPotionEffect), typeof(SpeedUpPotionEffect) };
        System.Type[] badEffects = { typeof(DamagePotionEffect), typeof(SpeedDownPotionEffect) };

        var pool = isBeneficial ? goodEffects : badEffects;
        int randomIndex = Random.Range(0, pool.Length);
        gameObject.AddComponent(pool[randomIndex]);
    }
}
