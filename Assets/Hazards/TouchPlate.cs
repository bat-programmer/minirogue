using UnityEngine;

public interface ITouchPlateEffect 
{
    void ApplyEffect(Jugador player);
    string GetEffectName();
    float GetDuration();
    Color GetDisplayColor();
    string GetDescription();
}

public class TouchPlate : MonoBehaviour
{
    [Header("Effect Settings")]
    public bool randomEffect = true;
    public float effectDuration = 3f;
    public ITouchPlateEffect[] effects;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        
        if (effects == null || effects.Length == 0)
        {
            effects = GetComponents<ITouchPlateEffect>();
        }

        if (effects.Length == 0) return;

        var player = other.GetComponent<Jugador>();
        if (player == null) return;

        if (randomEffect)
        {
            int randomIndex = Random.Range(0, effects.Length);
            effects[randomIndex].ApplyEffect(player);
        }
        else
        {
            foreach (var effect in effects)
            {
                effect.ApplyEffect(player);
            }
        }
    }
}
