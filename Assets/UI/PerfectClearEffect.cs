using UnityEngine;
using System.Collections;

public class PerfectClearEffect : MonoBehaviour
{
    public float timeFreezeScale = 0.1f;
    public float timeFreezeDuration = 0.3f;
    public Vector3 textPositionOffset = new Vector3(0, 2, 0);
    public Color textColor = Color.yellow;

    private void Start()
    {
        StartCoroutine(PlayEffect());
    }

    private IEnumerator PlayEffect()
    {
        // Freeze time
        Time.timeScale = timeFreezeScale;
        yield return new WaitForSecondsRealtime(timeFreezeDuration);
        Time.timeScale = 1f;

        // Create floating text
        FloatingText.Create(transform.position + textPositionOffset, "Perfect Clear", textColor);

        // Destroy this effect object after the animation is done
        Destroy(gameObject, 2f);
    }
}
