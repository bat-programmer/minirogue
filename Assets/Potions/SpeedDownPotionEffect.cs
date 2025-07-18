using Assets.Interfaces;
using UnityEngine;

public class SpeedDownPotionEffect : MonoBehaviour, IPotionEffect
{
    public void ApplyEffect(Jugador player)
    {
        player.GetComponent<MovementController>().ApplyPermanentSpeedModifier(-20); // adjust as needed
        Debug.Log("Permanent speed down applied");
    }

    public string GetUILabel()
    {
        return "Pocion de Reduccion de Velocidad";
    }
}
