using Assets.Interfaces;
using UnityEngine;

namespace Assets.Potions
{
    public class DamageUpPotionEffect : MonoBehaviour, IPotionEffect
    {
        public void ApplyEffect(Jugador player)
        {
            var attackController = player.GetComponent<PlayerAttackController>();
            attackController.ApplyFireballDamageBoost(5, float.MaxValue); // Use max value for "permanent" effect
            Debug.Log("Permanent damage up applied");
        }

        public string GetUILabel()
        {
            return "Pocion de Daño Aumentado";
        }
    }
}
