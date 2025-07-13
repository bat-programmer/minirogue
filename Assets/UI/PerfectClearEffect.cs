using UnityEngine;
using System.Collections;

public class PerfectClearEffect : MonoBehaviour
{
    public float timeFreezeScale = 0.1f;
    public float timeFreezeDuration = 0.3f;
    public Vector3 textPositionOffset = new Vector3(0, 1, 0);
    public Color textColor = Color.yellow;

    private void Start()
    {
        StartCoroutine(PlayEffect());
    }

    private IEnumerator PlayEffect()
    {
        // Create floating text at the center of the camera view
        Vector3 cameraCenter = Camera.main.transform.position;
        cameraCenter.z = -1; // Ensure the text is in front of other elements
        FloatingText.Create(cameraCenter, "Perfect Clear", textColor, 30, true);

        // Freeze time
        Time.timeScale = timeFreezeScale;
        yield return new WaitForSecondsRealtime(timeFreezeDuration);
        Time.timeScale = 1f;



        // Destroy this effect object after the animation is done
        Destroy(gameObject, 2f);
    }
}
