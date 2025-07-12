using Assets.Interfaces;
using System.Collections;
using UnityEngine;

namespace Assets.Potions
{
    public class DamageUpPotionEffect : MonoBehaviour, IPotionEffect
    {
        public void ApplyEffect(Jugador player)
        {
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.ApplyDamage(2); // Deal 1 full heart of damage
        }
            Debug.Log("Damage potion applied");
        }

        public string GetUILabel()
        {
           return "Pocion de Daño Aumentado";
        }
    }
}
