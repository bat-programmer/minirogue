using System.Collections;
using UnityEngine;

namespace Assets.Potions
{
    public class DamageUpPotionEffect : MonoBehaviour
    {
        public void ApplyEffect(Jugador player)
        {
            player.ApplyDamage(2); // Deal 1 full heart of damage
            Debug.Log("Damage potion applied");
        }
    }
}