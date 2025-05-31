using Assets.Interfaces;
using UnityEngine;

public class SpeedPotion : MonoBehaviour, IPotionEffect
{
    public float speedBoost = 2.0f;  // Multiplier (e.g., double speed)
    public float duration = 10f;      // Duration in seconds

    public void ApplyEffect(Jugador player)
    {
        //player.StartCoroutine(player.ApplySpeedBoost(speedBoost, duration));
    }

    public string GetUILabel()
    {
        return "Pocion de Aumento de Velocidad";
    }
}
