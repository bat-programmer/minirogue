using UnityEngine;

public class TouchPlateConfusionEffect : TouchPlateBaseEffect
{
    [Header("Confusion Settings")]
    [SerializeField] private float confusionDuration = 3f;
    [SerializeField] private bool invertControls = true;
    [SerializeField] private bool randomizeControls = false;

    public override void ApplyEffect(Jugador player)
    {
        player.GetComponent<MovementController>().ApplyConfusionEffect(confusionDuration, invertControls, randomizeControls);
    }
}
