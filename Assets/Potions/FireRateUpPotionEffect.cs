using Assets.Interfaces;
using UnityEngine;

public class FireRateUpPotionEffect : MonoBehaviour, IPotionEffect
{
    public void ApplyEffect(Jugador player)
    {
        player.GetComponent<PlayerAttackController>().fireRate -= 0.2f; // adjust as needed
        Debug.Log("Permanent fire up applied");
    }

    public string GetUILabel()
    {
        return "Pocion de Aumento de Cadencia de Fuego";
    }
}
