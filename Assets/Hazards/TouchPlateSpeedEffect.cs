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
        var movementController = player.GetComponent<MovementController>();
        if (isTemporary)
        {
            movementController.ApplySpeedModifier(speedMultiplier, GetDuration());
            Debug.Log($"Applying temporary speed modifier: {speedMultiplier} for {GetDuration()} seconds.");
        }
        else
        {
            movementController.ApplyPermanentSpeedModifier(speedMultiplier);
        }

        if (affectsEnemies)
        {
            TemporalEffectSystem.Instance.ActivateBulletTime(GetDuration(), enemySpeedModifier);
        }
    }
}
