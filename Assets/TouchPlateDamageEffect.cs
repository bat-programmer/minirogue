using UnityEngine;

public class TouchPlateDamageEffect : TouchPlateBaseEffect
{
    [Header("Damage Settings")]
    [SerializeField] private int damageAmount = 1;
    [SerializeField] private bool bypassInvulnerability = false;

    public override void ApplyEffect(Jugador player)
    {
        if (bypassInvulnerability)
        {
            player.ApplyDamage(damageAmount);
        }
        else
        {
            player.ApplyHealth(-damageAmount);
        }
    }
}
