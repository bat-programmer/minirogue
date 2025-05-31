using Assets.Interfaces;
using UnityEngine;

public class SpeedUpPotionEffect : MonoBehaviour, IPotionEffect
{
    public void ApplyEffect(Jugador player)
    {
        player.baseSpeed += 1; // adjust as needed
        Debug.Log("Permanent speed up applied");
    }

    public string GetUILabel()
    {
        return "Pocion de Aumento de Velocidad";
    }
}