using UnityEngine;

[CreateAssetMenu(fileName = "WandEffectData", menuName = "Wand System/Wand Effect Data")]
public class WandEffectData : ScriptableObject
{
    [Header("Effect Configuration")]
    public FireballEffectType effectType;
    public WandDurationType durationType = WandDurationType.Temporal;
    public float temporalDuration = 10f;
    public string effectName = "Magic Wand";
    public Sprite wandSprite;
    public Color wandColor = Color.white;

    [Header("Visual Feedback")]
    public string permanentPickupMessage = "Permanent {0} ability gained!";
    public string temporalPickupMessage = "Temporary {0} boost for {1} seconds!";

    public float GetDuration()
    {
        return durationType == WandDurationType.Permanent ? -1f : temporalDuration;
    }

    public string GetPickupMessage()
    {
        if (durationType == WandDurationType.Permanent)
            return string.Format(permanentPickupMessage, effectName);
        else
            return string.Format(temporalPickupMessage, effectName, temporalDuration);
    }
}