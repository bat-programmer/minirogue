using UnityEngine;
using TouchPlateEffects;

public abstract class TouchPlateBaseEffect : MonoBehaviour, ITouchPlateEffect
{
    [Header("Effect Settings")]
    [SerializeField] protected float duration = 3f;
    [SerializeField] protected Color displayColor = Color.white;
    [SerializeField] protected string description = "Effect description";

    public abstract void ApplyEffect(Jugador player);

    public string GetEffectName() => GetType().Name;
    public float GetDuration() => duration;
    public Color GetDisplayColor() => displayColor;
    public string GetDescription() => description;
}
