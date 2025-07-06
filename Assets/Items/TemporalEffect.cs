[System.Serializable]
public class TemporalEffect
{
    public FireballEffectType effectType;
    public float duration;

    public TemporalEffect(FireballEffectType type, float dur)
    {
        effectType = type;
        duration = dur;
    }
}