using UnityEngine;

public abstract class FireballEffect : MonoBehaviour
{
    protected BolaDeFuego fireball;
    protected bool isActive = true;
    public float RemainingDuration { get; protected set; } = -1f;
    public int Priority { get; protected set; } = 5;
    public bool IsPhysicsEffect { get; protected set; } = false;
    protected EffectState currentState = EffectState.Active;

    public enum EffectState { Active, Disabled, Suspended }

    public virtual void Initialize(BolaDeFuego fireball, float duration = -1f)
    {
        this.fireball = fireball;
        this.RemainingDuration = duration;
    }

    public abstract void ApplyEffect();
    public abstract void UpdateEffect();
    public abstract void RemoveEffect();

    public virtual bool ShouldUpdate()
    {
        return isActive && currentState == EffectState.Active;
    }
}
