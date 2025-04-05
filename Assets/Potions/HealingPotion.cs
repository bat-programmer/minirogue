using Assets.Interfaces;
using UnityEngine;

public class HealingPotion : MonoBehaviour, IPotionEffect
{
    public int healAmount = 20;

    public void ApplyEffect(Jugador player)
    {
        player.cambiarVida(healAmount);
        Debug.Log("Healed " + healAmount + " HP");
    }
}
