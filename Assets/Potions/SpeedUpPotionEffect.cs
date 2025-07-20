using Assets.Interfaces;
using UnityEngine;

public class SpeedUpPotionEffect : MonoBehaviour, IPotionEffect
{
    public void ApplyEffect(Jugador player)
    {
        player.GetComponent<MovementController>().ApplyPermanentSpeedModifier(2); // adjust as needed
        Debug.Log("Permanent speed up applied");
    }

    public string GetUILabel()
    {
        return "Pocion de Aumento de Velocidad";
    }
}
