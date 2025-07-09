using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class TemporalEffectSystem : MonoBehaviour 
{
    public static TemporalEffectSystem Instance;
    
    [Header("Bullet Time Settings")]
    public float enemySpeedModifier = 0.5f;
    public float effectDuration = 3f;
    
    private float originalAgentSpeed;
    private bool isEffectActive;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ActivateBulletTime()
    {
        if (isEffectActive) return;
        
        isEffectActive = true;
        var agents = FindObjectsOfType<NavMeshAgent>();
        
        if (agents.Length > 0)
        {
            originalAgentSpeed = agents[0].speed;
            foreach (var agent in agents)
            {
                agent.speed *= enemySpeedModifier;
            }
        }
        
        StartCoroutine(ResetAfterDuration());
    }

    IEnumerator ResetAfterDuration()
    {
        yield return new WaitForSeconds(effectDuration);
        
        var agents = FindObjectsOfType<NavMeshAgent>();
        foreach (var agent in agents)
        {
            agent.speed = originalAgentSpeed;
        }
        
        isEffectActive = false;
    }
}
