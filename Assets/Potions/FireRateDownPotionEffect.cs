using System.Collections;
using UnityEngine;

namespace Assets.Potions
{
    public class FireRateDownPotionEffect : MonoBehaviour
    {
        public void ApplyEffect(Jugador player)
        {
            player.fireRate += 0.1f; // adjust as needed
            Debug.Log("Permanent fire up applied");
        }

    }
}