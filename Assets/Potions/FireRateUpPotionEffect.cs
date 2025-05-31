using Assets.Interfaces;
using UnityEngine;

public class FireRateUpPotionEffect : MonoBehaviour, IPotionEffect
{
    public void ApplyEffect(Jugador player)
    {
        player.fireRate -= 0.1f; // adjust as needed
        Debug.Log("Permanent fire up applied");
    }

    public string GetUILabel()
    {
        return "Pocion de Aumento de Cadencia de Fuego";
    }
}