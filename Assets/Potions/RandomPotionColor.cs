using UnityEngine;

public class RandomPotionColor : MonoBehaviour
{
    public SpriteRenderer liquidRenderer; // Assign in Inspector

    void Start()
    {
        // Assign a random color to the liquid (this is where you’ll later link attributes)
        liquidRenderer.color = Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.8f, 1f); // vivid, saturated
    }
}
