using System.Collections.Generic;
using UnityEngine;

public class PoolImpactEffect : MonoBehaviour
{
    public GameObject impactEffectPrefab;
    public int poolSize = 10;

    private Queue<GameObject> impactPool = new Queue<GameObject>();

    void Start()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject effect = Instantiate(impactEffectPrefab, transform);
            effect.SetActive(false);
            impactPool.Enqueue(effect);
        }
    }

    public GameObject GetImpactEffect(Vector3 position)
    {
        if (impactPool.Count == 0)
        {
            ExpandPool();
        }

        GameObject effect = impactPool.Dequeue();
        effect.transform.position = position;
        effect.SetActive(true);

        ParticleSystem ps = effect.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            ps.Clear();
            ps.Play();
            StartCoroutine(ReturnAfterDuration(effect, ps.main.duration + ps.main.startLifetime.constantMax));
        }

        return effect;
    }

    private IEnumerator<WaitForSeconds> ReturnAfterDuration(GameObject effect, float duration)
    {
        yield return new WaitForSeconds(duration);
        effect.SetActive(false);
        impactPool.Enqueue(effect);
    }

    private void ExpandPool()
    {
        GameObject effect = Instantiate(impactEffectPrefab, transform);
        effect.SetActive(false);
        impactPool.Enqueue(effect);
    }
}
