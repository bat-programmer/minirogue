using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class TemporalEffectSystem : MonoBehaviour 
{
    public static TemporalEffectSystem Instance;
    
    [Header("Effect Settings")]
    public float defaultEffectDuration = 3f;
    
    private Dictionary<NavMeshAgent, float> originalAgentSpeeds = new Dictionary<NavMeshAgent, float>();
    private bool isBulletTimeActive;

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

    public void ActivateBulletTime(float duration, float speedModifier = 0.5f)
    {
        if (isBulletTimeActive) return;
        
        isBulletTimeActive = true;
        var agents = FindObjectsOfType<NavMeshAgent>();
        
        originalAgentSpeeds.Clear();
        foreach (var agent in agents)
        {
            originalAgentSpeeds[agent] = agent.speed;
            agent.speed *= speedModifier;
        }
        
        StartCoroutine(ResetAfterDuration(duration, speedModifier));
    }

    IEnumerator ResetAfterDuration(float duration, float speedModifier)
    {
        yield return new WaitForSeconds(duration);
        
        var agents = FindObjectsOfType<NavMeshAgent>();
        foreach (var agent in agents)
        {
            if (originalAgentSpeeds.TryGetValue(agent, out float originalSpeed))
            {
                agent.speed = originalSpeed;
            }
        }
        
        isBulletTimeActive = false;
    }
}
