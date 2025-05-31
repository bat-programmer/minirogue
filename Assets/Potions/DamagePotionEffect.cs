using Assets.Interfaces;
using UnityEngine;

public class DamagePotionEffect : MonoBehaviour, IPotionEffect
{
    public void ApplyEffect(Jugador player)
    {
        player.ApplyDamage(2); // Deal 1 full heart of damage
        Debug.Log("Damage potion applied");
    }

    public string GetUILabel()
    {
        return "Pocion Acido";
    }
}