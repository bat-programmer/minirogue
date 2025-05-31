using Assets.Interfaces;
using System.Collections;
using UnityEngine;

namespace Assets.Potions
{
    public class FireRateDownPotionEffect : IPotionEffect
    {
        public void ApplyEffect(Jugador player)
        {
            player.fireRate += 0.1f; // adjust as needed
            Debug.Log("Permanent fire up applied");
        }

        public string GetUILabel()
        {
            return "Pocion ralentizadora de disparo";
        }
    }
}