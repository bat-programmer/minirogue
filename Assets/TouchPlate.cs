using UnityEngine;

public class TouchPlate : MonoBehaviour
{
    [Header("Effect Settings")]
    public bool activateBulletTime = true;
    public float effectDuration = 3f;
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && activateBulletTime)
        {
            TemporalEffectSystem.Instance.ActivateBulletTime();
        }
    }
}
