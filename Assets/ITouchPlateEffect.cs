using UnityEngine;

namespace TouchPlateEffects
{
    public interface ITouchPlateEffect 
    {
        void ApplyEffect(Jugador player);
        string GetEffectName();
        float GetDuration();
        
        // Optional configuration for editor
        Color GetDisplayColor();
        string GetDescription();
    }
}
