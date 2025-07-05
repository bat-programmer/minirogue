using UnityEngine;

public abstract class FireballEffect : MonoBehaviour
{
    protected BolaDeFuego fireball;
    protected float duration;
    protected bool isActive = true;

    public virtual void Initialize(BolaDeFuego fireballRef, float effectDuration = -1f)
    {
        fireball = fireballRef;
        duration = effectDuration;
    }

    public abstract void ApplyEffect();
    public virtual void UpdateEffect() { }
    public virtual void RemoveEffect() { }

    protected virtual void Update()
    {
        if (!isActive) return;

        if (duration > 0)
        {
            duration -= Time.deltaTime;
            if (duration <= 0)
            {
                RemoveEffect();
                return;
            }
        }

        UpdateEffect();
    }
}