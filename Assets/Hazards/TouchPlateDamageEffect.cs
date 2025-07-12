using UnityEngine;

public class TouchPlateDamageEffect : TouchPlateBaseEffect
{
    [Header("Damage Settings")]
    [SerializeField] private int damageAmount = 1;
    [SerializeField] private bool bypassInvulnerability = false;

    public override void ApplyEffect(Jugador player)
    {
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            if (bypassInvulnerability)
            {
                playerHealth.ApplyDamage(damageAmount);
            }
            else
            {
                playerHealth.ApplyDamage(damageAmount);
            }
        }
    }
}
