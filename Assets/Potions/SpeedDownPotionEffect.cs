using Assets.Interfaces;
using UnityEngine;

public class SpeedDownPotionEffect : MonoBehaviour, IPotionEffect
{
    public void ApplyEffect(Jugador player)
    {
        player.baseSpeed -= 20; // adjust as needed
        Debug.Log("Permanent speed down applied");
    }
}
