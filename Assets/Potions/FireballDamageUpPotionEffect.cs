using Assets.Interfaces;
using UnityEngine;

public class FireballDamageUpPotionEffect : MonoBehaviour, IPotionEffect
{
    [SerializeField] private int damageIncrease = 500; // Amount to increase
    [SerializeField] private float duration = 5f;      // Duration in seconds

    public void ApplyEffect(Jugador player)
    {
        player.GetComponent<PlayerAttackController>().ApplyFireballDamageBoost(damageIncrease, duration);
        Debug.Log("Fireball damage up applied!");
    }

    public string GetUILabel()
    {
        return "Pocion de Daño de Bola de Fuego";
    }
}
