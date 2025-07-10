using UnityEngine;

public class TouchPlateSpeedEffect : TouchPlateBaseEffect
{
    [Header("Speed Settings")]
    [SerializeField] private float speedMultiplier = 0.5f;
    [SerializeField] private bool isTemporary = true;
    [SerializeField] private bool affectsEnemies = false;
    [SerializeField] private float enemySpeedModifier = 0.5f;

    public override void ApplyEffect(Jugador player)
    {
        if (isTemporary)
        {
            player.ApplySpeedModifier(speedMultiplier, GetDuration());
            Debug.Log($"Applying temporary speed modifier: {speedMultiplier} for {GetDuration()} seconds.");
        }
        else
        {
            player.ApplyPermanentSpeedModifier(speedMultiplier);
        }

        if (affectsEnemies)
        {
            TemporalEffectSystem.Instance.ActivateBulletTime(GetDuration(), enemySpeedModifier);
        }
    }
}
