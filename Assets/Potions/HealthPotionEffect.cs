using Assets.Interfaces;
using UnityEngine;

public class HealthPotionEffect : MonoBehaviour, IPotionEffect
{
    public void ApplyEffect(Jugador player)
    {
        player.ApplyHealth(2); // Add 1 full heart
        Debug.Log("Health potion applied");
    }

    public string GetUILabel()
    {
        return "Pocion de Curacion";
    }
}
